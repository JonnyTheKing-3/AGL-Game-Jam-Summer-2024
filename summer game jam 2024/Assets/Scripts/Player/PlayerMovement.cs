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

    [Header("PLAYER SETTINGS")] 
    public KeyCode JumpKey;
    public int NumberOfJumpsForPlayer;
    [SerializeField] private float JumpDelayReset = 0.2f;

    [Header("CHARACTER SETTINGS")] 
    public float gravity;
    public float CappedSpeed;

    [Header("MOVEMENT VARIABLES")]
    [SerializeField] private float WalkSpeed;
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
    public bool Grounded;
    public int AvailableJumps;
    public bool CoyoteAvailable;

    [Header("GROUNDING")]
    private LayerMask groundLayer;
    public float RayForGroundCheck = 1.3f; // Might need to change this variable depending on object. EDIT: It's now .87f
    private bool CanJump = true;

    [Header("EXTRA")]
    public float lastmoveX;                  // Keeps track of if the player was moving
    public float xForCalculateMovement = 0;

    // Keeps track of the appropriate jump direction based on gravity pull
    public Vector2 jumpDir;
    [SerializeField] private float hitpointIncrement;
    public enum Direction
    {
        up, down, left, right
    }

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
        FMODbanks.Instance.PlayMusic();
    }

    private void Update()
    {
        PlayerInput();
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
        if ( Input.GetKeyDown(JumpKey) && (AvailableJumps > 0 || coyoteTimer > 0) && CanJump)
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
            // If the player had multiple jumps, let's default to one, but if they had only one jump, then do 0 jumps
            if (NumberOfJumpsForPlayer == 1) { AvailableJumps = 0; }
            else { AvailableJumps = 1; }
        }
    }

    private void FixedUpdate()
    {
        // Check if player is grounded and if their on a slope
        Grounded = IsGrounded();

        // Stick player to the ground
        if (Grounded && CanJump) { StickPlayerToGround(); }
        
        // if player is falling, limit the fall speed
        if (!Grounded) { CapFallSpeed(); }
        
        // Grounded Movement
        rb.AddForce(CalculateMovement() * transform.right);
    }

    public void PlayerInput()
    {
        moveX = Input.GetAxisRaw("Horizontal");
        moveY = Input.GetAxisRaw("Vertical");

        if (moveX == 0 && Mathf.Abs(lastmoveX) == 1)
        { 
            FMODbanks.Instance.StopHoverSFX();
        }
        // If the player stopped and was moving, stop the hover sound
        else if (Mathf.Abs(moveX) == 1 && lastmoveX == 0)
        {
            FMODbanks.Instance.PlayHoverSFX();
        }  
        // When the player first moves, play the hover sound
        lastmoveX = moveX; // Update lastmoveX
        
    }

    public void Jump()
    {
        // Add velocity to the direction gravity is pulling 
        rb.velocity += jumpDir*jumpForce;
    }
    
    private float CalculateMovement()
    {
        // Speed of player
        float targetSpeed = moveX * WalkSpeed;
        
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

    public bool IsGrounded()
    {
        // This portion is to avoid extra impulse player gets when they jump on to the platform when they reach a platform from under AND to help with platform script
        // if (!Grounded && rb.velocity.y > 0) { return false;}
        Grounded = Physics2D.Raycast(transform.position, -transform.up, RayForGroundCheck, groundLayer);
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

    private void StickPlayerToGround()
    {
        // Shoot ray to the ground and stick player to the ground they are standing on
        RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, -jumpDir, RayForGroundCheck, groundLayer);
        if (jumpDir == Vector2.up)
        {
            if (transform.position.y > hitInfo.point.y + hitpointIncrement)
            {
                transform.position = new Vector2(transform.position.x, hitInfo.point.y + hitpointIncrement);
            } 
        }
        else if (jumpDir == Vector2.down)
        {
            if (transform.position.y < hitInfo.point.y - hitpointIncrement)
            {
                transform.position = new Vector2(transform.position.x, hitInfo.point.y - hitpointIncrement);
            } 
        }
        else if (jumpDir == Vector2.right)
        {
            if (transform.position.x < hitInfo.point.x - hitpointIncrement)
            {
                transform.position = new Vector2(hitInfo.point.x - hitpointIncrement, transform.position.y);
            } 
        }
        else if (jumpDir == Vector2.left)
        {
            if (transform.position.x > hitInfo.point.x + hitpointIncrement)
            {
                transform.position = new Vector2(hitInfo.point.x + hitpointIncrement, transform.position.y);
            } 
        }
    }

    private void CapFallSpeed()
    {
        // Check jump direction to know the gravity direction. Then cap the fall speed if necessary
        if (jumpDir == Vector2.up)
        {
            if (rb.velocity.y < -CappedSpeed) { rb.velocity= new Vector2(rb.velocity.x, -CappedSpeed); }
        }
        else if (jumpDir == Vector2.down)
        {
            if (rb.velocity.y > CappedSpeed) { rb.velocity = new Vector2(rb.velocity.x, CappedSpeed); }
        }
        else if (jumpDir == Vector2.left)
        {
            if (rb.velocity.x < -CappedSpeed) { rb.velocity = new Vector2(-CappedSpeed, rb.velocity.y); }
        }
        else if (jumpDir == Vector2.right)
        {
            if (rb.velocity.x > CappedSpeed) { rb.velocity = new Vector2(CappedSpeed, rb.velocity.y); }
        }
    }
    
}
