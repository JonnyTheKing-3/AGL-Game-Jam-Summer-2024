using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform player;
    public bool ChasePlayer = false;
    public float PlayerCloseness = 5f;
    public float speed = 5f;
    public Animator animator;
    public bool Facing = false;
    
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
        Facing = false;
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = player.rotation;
        if (ChasePlayer && Vector3.Distance(gameObject.transform.position, player.position) > PlayerCloseness)
        {
            //Vector2 direction = player.transform.position - transform.position;
            transform.position = Vector2.Lerp(this.transform.position, player.transform.position, speed * Time.deltaTime);
        }
        Vector3 direction = transform.position - player.position;
        if (direction.x > 0 && Facing)
        {
            Facing = false; 
            Flip();
        }
        else if (direction.x < 0 && !Facing)
        {
            Facing = true; 
            Flip();
        }

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            FMODbanks.Instance.SetParameterOfMusicToOne();
            animator.SetBool("Player Touched", true);
            ChasePlayer = true;
            GameManager.GotFriend = true;
        }
    }
    
    private void Flip()
    {
        Vector3 localScale = transform.localScale; 
        localScale.x *= -1f; 
        transform.localScale = localScale;
    }
}
