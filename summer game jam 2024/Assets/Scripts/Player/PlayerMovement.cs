using System;
using System.Timers;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using FMODUnity;

public class PlayerMovement : MonoBehaviour
{
    [Header("REFERENCES")]
    private Rigidbody2D rb;
    private ConstantForce2D myconstantForce;
    private FMODbanks FmodBanks;

    [Header("PLAYER SETTINGS")] 
    public KeyCode JumpKey;
    public KeyCode DashKey;
    public int NumberOfJumpsForPlayer;
    [SerializeField] private float JumpDelayReset = 0.2f;

    [Header("CHARACTER SETTINGS")] 
    public float gravity;

    [Header("SLOPE STUFF")]
    public float maxSlopeAngle = 45f;
    private RaycastHit2D slopeHit;

    [Header("MOVEMENT VARIABLES")]
    [SerializeField] private float WalkSpeed;
    [SerializeField] private float DashSpeed;
    [SerializeField] private float jumpForce;
    
    [Header("MOVEMENT SMOOTHING")]
    [SerializeField] private float acceleration;
    [SerializeField] private float deceleration;
    private float velPower = .9f;    // > 1: exponential --- = 1: linear --- < 1: logarithmic
                                                // velPower is how fast Accel/Deccel is approached
                                                // if = 1, accel/deccel will be approached naturally
    [Header("COYOTE TIME")] 
    [SerializeField] private float coyoteTime;
    private float coyoteTimer;
    
    [Header("STATUS ON PLAYER")] 
    public float moveX;
    public float moveY;
    public float DesiredSpeed;
    public bool Grounded;
    public int AvailableJumps;
    public bool CoyoteAvailable;
    public float SurfaceAngle;
    public bool IsOnSlope = false;
    
    // Ground checks for jumping
    private LayerMask groundLayer;
    public float RayForGroundCheck = 1.3f; // Might need to change this variable depending on object
    private bool CanJump = true;
    // Used for passthrough platform script
    public bool OnDelay = false;
    
    // flip variable to see what direction the player is facing
    private bool isFacingRight = true;
    
    // Keeps track of if the player was moving
    public float lastmoveX;

    // Keeps track of the appropriate jump direction based on gravity pull
    public Vector2 jumpDir;

    public float xForCalculateMovement = 0;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        myconstantForce = GetComponent<ConstantForce2D>();
        groundLayer = LayerMask.GetMask("Walkable");

        AvailableJumps = NumberOfJumpsForPlayer;
        CanJump = true;
        velPower = 0.9f;
        moveX = 0f;
        moveY = 0f;
        lastmoveX = 0f;
    }

    private void Update()
    {
        PlayerInput();
        FaceInputDirection();
        SetUpAppropriateGravityAndJumpDirectionAndVelocity();

        // For debugging coyote time
        if (coyoteTimer > 0)
        {
            CoyoteAvailable = true;
        }
        else
        {
            CoyoteAvailable = false;
        }
        
        // Can jump is so explained in comment after the jump is called inside the jump check
        if (Grounded && CanJump) { AvailableJumps = NumberOfJumpsForPlayer; }

        // If the player is not grounded, start the coyote timer. Otherwise, reset
        if (Grounded) { coyoteTimer = coyoteTime; }
        else { coyoteTimer -= Time.deltaTime; }
        
        // Jump if the player pressed jump button AND still has available jumps
        if ( Input.GetKeyDown(JumpKey) && (AvailableJumps > 0 || coyoteTimer > 0))
        {
            CanJump = false;
            FMODbanks.Instance.PlayJumpSFX(gameObject);     // Play jump sfx
            // Decrease the number of jumps and remove coyote time before jump
            --AvailableJumps;
            coyoteTimer = 0;
            
            // Jump
            Jump();
                
            /*
             * When jumping, sometimes the player will travel such small distance from the ground by the time the next frame hits
             * that the player will still be considered grounded. This will reset the jumps, making the frist jump free
             * 
             * We don't want this
             * 
             * So to address this, we'll have a little artificial buffer
             * so that when we jump, we have a bit of time before we are able to reset the jump count
             * 
             * PS if buffer is too small, make it larger lol
             */
            
            Invoke("EnableJump", JumpDelayReset);
        }

        // If the player is past the coyote time and didn't jump, then make the player have only one jump
        if (coyoteTimer <= 0 && AvailableJumps == NumberOfJumpsForPlayer && NumberOfJumpsForPlayer < 2)
        {
            AvailableJumps = 1;
        }
    }

    private void FixedUpdate()
    {
        // Check if player is grounded and if their on a slope
        Grounded = IsGrounded();
        IsOnSlope = OnSlope();

        // MOVEMENT
        //---------
        // Grounded Movement
        if (!IsOnSlope)
        {
            rb.AddForce(CalculateMovement() * transform.right);
        }
        
        // Slope Movement
        else if (IsOnSlope)
        {
            rb.AddForce(CalculateMovement() * GetSlopeDirection());
        }
        
        // Air Movement
        // else if (!Grounded)
        // {
        //     rb.AddForce((CalculateMovement() * AirSpeed) * Vector2.right);
        // }
    }

    public void PlayerInput()
    {
        moveX = Input.GetAxisRaw("Horizontal");
        moveY = Input.GetAxisRaw("Vertical");

        if (moveX == 0 && Mathf.Abs(lastmoveX) == 1) { FMODbanks.Instance.StopHoverSFX(); }       // If the player stopped and was moving, stop the hover sound
        else if (Mathf.Abs(moveX) == 1 && lastmoveX == 0) { FMODbanks.Instance.PlayHoverSFX(); }  // When the player first moves, play the hover sound
        lastmoveX = moveX; // Update lastmoveX

        // If player presses the dash key, make the target speed be the dash speed
        if (Input.GetKey(DashKey))
        {
            DesiredSpeed = DashSpeed;
        }
        else
        {
            DesiredSpeed = WalkSpeed;
        }
    }

    public void Jump()
    {
        // Add velocity to the direction gravity is pulling 
        rb.velocity += jumpDir*jumpForce;
    }
    
    private float CalculateMovement()
    {
        // Speed of player
        float targetSpeed = moveX * DesiredSpeed;
        
        // Calculate the difference of the speed the player wants to go by
        // how fast the player is already going
        float speedDif = targetSpeed - xForCalculateMovement;
        
        // If the player is going the same direction as the one they pressed, accelerate. Else, decelerate
        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : deceleration;
        
        // Applied acceleration to speed difference, AND THEN, raised to something so that
        // speed increases and decreases depending on the current movement status of the player.
        // Lastly, preserve the direction
        return Mathf.Pow(Mathf.Abs(speedDif) * accelRate, velPower) * Math.Sign(speedDif);
    }
    
    private void FaceInputDirection()
    {
        if (isFacingRight && moveX < 0 || !isFacingRight && moveX > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    public bool IsGrounded()
    {
        // This portion is to avoid extra impulse player gets when they jump on to the platform when they reach a platform from under AND to help with platform script
        if (!Grounded && rb.velocity.y > 0 || OnDelay) { return false;}
        
        Grounded = Physics2D.Raycast(transform.position, -transform.up, RayForGroundCheck, groundLayer);
        
        // Quick check to make sure the player isn't able to go up slopes that are too steep
        if (Grounded && !IsOnSlope && SurfaceAngle != 0) { Grounded = false;}
        
        return Grounded;
    }

    private void EnableJump()
    {
        CanJump = true;
    }

    private void SetUpAppropriateGravityAndJumpDirectionAndVelocity()
    {
        // Set up appropriate gravity based on player's rotation (the player will always be rotated towards the gravity pull)
        if (transform.rotation.eulerAngles.z == 0f) { myconstantForce.force = Vector2.down * gravity; jumpDir = Vector2.up; xForCalculateMovement = rb.velocity.x; }
        else if (transform.rotation.eulerAngles.z == 180f) { myconstantForce.force = Vector2.up * gravity; jumpDir = Vector2.down; xForCalculateMovement = -rb.velocity.x; }
        else if (transform.rotation.eulerAngles.z == 90f) { myconstantForce.force = Vector2.right * gravity; jumpDir = Vector2.left; xForCalculateMovement = rb.velocity.y; }
        else if (transform.rotation.eulerAngles.z == 270f) { myconstantForce.force = Vector2.left * gravity; jumpDir = Vector2.right; xForCalculateMovement = -rb.velocity.y; }
        
        // If the player is in the air, enable extra gravity. Otherwise, disable extra gravity
        myconstantForce.enabled = !Grounded;
    }

    public bool OnSlope()
    {
        slopeHit = Physics2D.Raycast(transform.position, -transform.up, RayForGroundCheck, groundLayer);
        if (slopeHit.collider != null)
        {
            SurfaceAngle = Mathf.Abs(Vector2.Angle(transform.up, slopeHit.normal));
            return SurfaceAngle < maxSlopeAngle && SurfaceAngle != 0;
        }
        
        return false;
        
    }

    public Vector3 GetSlopeDirection()
    {
        return Vector3.ProjectOnPlane(Vector2.right, slopeHit.normal).normalized;
    }
}
