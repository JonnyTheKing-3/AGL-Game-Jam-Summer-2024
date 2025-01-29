using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquashAreaTrigger : MonoBehaviour
{
    private Vector3 StartingPos;

    private void Start()
    {
        StartingPos = transform.position;
    }

    private void Update()
    {
        transform.position = StartingPos;
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // Debug.Log("Enter");
            other.GetComponent<SquashDetection>().PlatformDestinationTouching = true;
        }
    }
    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // Debug.Log("Exit");
            other.GetComponent<SquashDetection>().PlatformDestinationTouching = false;
        }
    }
}
