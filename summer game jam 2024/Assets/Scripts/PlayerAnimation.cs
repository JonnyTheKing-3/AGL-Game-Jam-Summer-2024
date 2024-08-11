using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    public PlayerMovement player;

    public Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<PlayerMovement>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // If player is moving to the right
        if (player.moveX > 0.1f)
        {
            
            animator.SetBool("RunningR", true);
            animator.SetBool("RunningL", false);
        }
        // If player is moving to the left
        else if (player.moveX < -0.1f)
        {
            animator.SetBool("RunningR", false);
            animator.SetBool("RunningL", true);
        }
        // If player is moving to the left
        else if (Mathf.Abs(player.moveX) <= 0.1f)
        {
            animator.SetBool("RunningR", false);
            animator.SetBool("RunningL", false);
        }
        // Death
        // else if ()
        // {
        //     animator.SetBool("Die", true);
        // }
        
    }
}
