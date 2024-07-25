using System;
using UnityEngine;

public class EnemyChasingAI : MonoBehaviour
{
    // refernce to the player
    [SerializeField] private GameObject player;
    [SerializeField] private Vector3 ogPos;
    
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
    }

    void Update()
    {   // get distance from the player
        distance = Vector2.Distance(transform.position, player.transform.position);

        // if the player is within the range, chase the player
        if (distance < distanceBetween)
        {
            //Vector2 direction = player.transform.position - transform.position;
            transform.position = Vector2.MoveTowards(this.transform.position,
                player.transform.position, speed * Time.deltaTime);
        }
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // this enemy touches the plater, "Self destruct" (damage the player a lot and destroy the object)
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Touched player");
            gm.Respawn();
            transform.position = ogPos;
        }
    }
}