using System;
using UnityEngine;

public class EnemyChasingAI : MonoBehaviour
{
    // refernce to the player
    [SerializeField] private GameObject player;
    [SerializeField] private Vector3 ogPos;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float speedCap;
    
    // stats for this enemy
    public float speed;

    // distance to see if player is in range
    private float distance;
    [SerializeField] private float distanceBetween;

    public GameManager gm;

    private void Start()
    {
        ogPos = this.transform.position;
        player = GameObject.FindGameObjectWithTag("Player");
        gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {   // get distance from the player
        distance = Vector2.Distance(transform.position, player.transform.position);

        // if the player is within the range, chase the player
        if (distance < distanceBetween)
        {
            Vector2 direction = player.transform.position - transform.position;
            rb.AddForce(direction * speed);
        }

        // Make sure enemy doesn't move too fast
        rb.velocity = Vector2.ClampMagnitude(rb.velocity, speedCap);

    }
    

    private void OnTriggerEnter2D(Collider2D other)
    {
        // this enemy touches the plater, "Self destruct" (damage the player a lot and destroy the object)
        if (other.gameObject.CompareTag("Player"))
        {
            gm.Respawn();
            transform.position = ogPos;
        }
    }
}