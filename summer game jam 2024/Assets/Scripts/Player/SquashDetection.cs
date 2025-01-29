using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquashDetection : MonoBehaviour
{
    public bool MovingPlatformTouching = false;
    public bool PlatformDestinationTouching = false;
    public GameManager gm;

    void Start()
    {
        MovingPlatformTouching = false;
        PlatformDestinationTouching = false;
        gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    // If a moving platform squashes the player with the ground, call squash
    private void Update() { if (MovingPlatformTouching && PlatformDestinationTouching) { Squash(); } }

    // For squash effect. Works with platform SquashAreaTrigger script
    private void OnCollisionEnter2D(Collision2D other)
    {
        // Check if it's the platform and if it's not done moving
        if (other.gameObject.CompareTag("Platform"))
        {
            if (other.transform.GetChild(0).TryGetComponent<MovingPlatformManaging>(out MovingPlatformManaging mpm))
            {
                MovingPlatformTouching = !mpm.finished;
            }
            else
            {
                MovingPlatformTouching = false;
            }
        }
    }
    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Platform"))
        {
            MovingPlatformTouching = false;
        }
    }

    // Respawn because player got squashed
    private void Squash()
    {
        // Debug.Log("Squash");
        gm.Respawn();
    }
}
