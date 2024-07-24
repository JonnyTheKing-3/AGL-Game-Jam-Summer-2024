using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReachedpointA : MonoBehaviour
{
    public GameObject parent;

    private void Start()
    {
        parent = transform.parent.gameObject;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Touched");
        if (other.gameObject == parent)
        {
            Debug.Log("Touched by parent");
            GetComponentInParent<GoombaMovement>().ReachedPointB = false;
        }
    }
}