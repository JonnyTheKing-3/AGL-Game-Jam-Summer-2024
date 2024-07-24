using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public Transform respawn_point = null;
    public Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }
    
    public void Respawn()
    {
        // If the player hasn't reached a checkpoint, reload the scene. Otherwise, spawn in the checkpoint
        if (respawn_point == null) {Debug.Log("Reload"); SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); }
        else                       {Debug.Log("Respawn"); player.position = respawn_point.position; }
    }

    public void CheckpointReached(Transform checkpoint)
    {
        Debug.Log("Updating Checkpoint");
        // Update checkpoint
        respawn_point = checkpoint;
    }
}
