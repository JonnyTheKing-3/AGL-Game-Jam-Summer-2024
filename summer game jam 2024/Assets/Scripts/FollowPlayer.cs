using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform player;
    public bool ChasePlayer = false;
    public float PlayerCloseness = 5f;
    public float speed = 5f;
    
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform; 
    }

    // Update is called once per frame
    void Update()
    {
        if (ChasePlayer && Vector3.Distance(gameObject.transform.position, player.position) > PlayerCloseness)
        {
            //Vector2 direction = player.transform.position - transform.position;
            transform.position = Vector2.Lerp(this.transform.position, player.transform.position, speed * Time.deltaTime);
        }
    }
}
