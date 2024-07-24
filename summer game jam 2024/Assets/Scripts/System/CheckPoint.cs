using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    
    public GameManager gm;

    void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        // If the player touched the pitfall, respawn
        if (other.CompareTag("Player"))
        {
            Debug.Log("Entered Checkpoint");
            gm.CheckpointReached(gameObject.transform, other.transform.eulerAngles.z);
        }
    }
}
