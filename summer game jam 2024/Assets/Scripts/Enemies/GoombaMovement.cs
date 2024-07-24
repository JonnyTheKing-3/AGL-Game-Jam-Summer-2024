using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoombaMovement : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public float speed = 3f;
    public bool ReachedPointB = false;
    public GameManager gm;
    public float distanceA;
    public float distanceB;
    
    void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        transform.position = pointA.position;
        ReachedPointB = false;
    }
    private void Update()
    {
        if (ReachedPointB)
        {
            //Vector2 direction = player.transform.position - transform.position;
            transform.position = Vector2.MoveTowards(this.transform.position,
                pointA.position, speed * Time.deltaTime);
        }
        else
        {
            //Vector2 direction = player.transform.position - transform.position;
            transform.position = Vector2.MoveTowards(this.transform.position,
                pointB.position, speed * Time.deltaTime);
        }

        // Using a small tolerance for position comparison
        distanceA = Vector2.Distance(transform.position, pointA.position);
        
        if (distanceA < 0.1f)
        {
            ReachedPointB = false;
        }

        distanceB = Vector2.Distance(transform.position, pointB.position);
        if (distanceB < 0.1f)
        {
            ReachedPointB = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            gm.Respawn();
            transform.position = pointA.position;
        }
    }
}