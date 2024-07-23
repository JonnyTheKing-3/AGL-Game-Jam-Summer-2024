using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PassthroughPlatform : MonoBehaviour
{
    // Choices for platform type. GoingDown: Can only be accessed going down. GoingUp: Opposite of GoingDown. Both: Can be accessed from up and down
    public enum OneWayPlatform
    {
        GoingDown,
        GoingUp,
        Both
    };
    public OneWayPlatform Type = OneWayPlatform.Both;
    
    // For reactivating platform
    [SerializeField] private float delay = .5f;
    private float forceAfterDeactivation = 8f;
    
    private Collider2D col;               // Platform collider
    private Collider2D playerCollider;    // Player Collider
    private GameObject player;

    // Keeps track of when falling through a platform
    private bool isOnDelay = false;
    
    private void Start()
    {
        // Initializing references
        col = GetComponent<Collider2D>();
        player = FindObjectOfType<PlayerMovement>().gameObject;
        playerCollider = player.GetComponent<Collider2D>();
    }

    // This handles reaching the platform from below
    private void Update()
    {
        // If the player not close enough, ignore the rest  
        if (Vector3.Distance(player.transform.position, gameObject.transform.position) > 5f) { return; }

        if (Type != OneWayPlatform.GoingDown && !isOnDelay)
        {
            // If the player's position is above the platform, activate the interaction between player and platform. If it's below the platform, deactivate it
            if (playerCollider.bounds.min.y > col.bounds.center.y)
            {
                Physics2D.IgnoreCollision(playerCollider, col, false);
            }
            else
            {
                Physics2D.IgnoreCollision(playerCollider, col, true);
            }
        }
    }
    
    // This handles falling through the platform
    private void OnCollisionStay2D(Collision2D collision)
    {
        // Checks to see if the colliding object is the player
        if (collision.gameObject == player)
        {
            // if the player pressed down, let the player fall through
            if (player.GetComponent<PlayerMovement>().moveY < -0.5f && playerCollider.bounds.min.y > col.bounds.center.y && Type != OneWayPlatform.GoingUp)
            {
                isOnDelay = true;
                player.GetComponent<PlayerMovement>().OnDelay = true;           // So player doesn't become grounded during this state
                Physics2D.IgnoreCollision(playerCollider, col, true);
                player.GetComponent<Rigidbody2D>().AddForce(Vector2.down * forceAfterDeactivation, ForceMode2D.Impulse);    // Impulse to make falling through the platform feel better
                StartCoroutine(StopIgnoring());
            }
        }
    }

    // Coroutine that toggles the collider on the platform to allow the player to collide with it again... plus extra to make other things work
    private IEnumerator StopIgnoring()
    {
        yield return new WaitForSeconds(delay);
        isOnDelay = false;
        Physics2D.IgnoreCollision(playerCollider, col, false);
        player.GetComponent<PlayerMovement>().OnDelay = false;
    }
    
}
