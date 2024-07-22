using System;
using UnityEngine;

public class SmoothCameraFollow : MonoBehaviour
{
    // Target for the camera to follow. USUALLY IS PLAYER, SO DRAG PLAYER INTO THIS SPOT
    [SerializeField] private Transform target;
    
    // How fast the camera should reach the character
    public float TimeToGetToTarget = 2;
    
    //Variables to manipulate the center of the camera
    public float xOffset = 0f;
    public float yOffset = 0f;
    public float zOffset = 0f;
    
    // Velocity variable for SmoothDamp
    private Vector3 velocity = Vector3.zero;

    private void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void LateUpdate()
    {   // The position of the object to reach
        Vector3 desiredPos = new Vector3(target.position.x + xOffset, target.position.y + yOffset, (-10 +zOffset));
        
        // Get camera follow 
        Vector3 newPos = Vector3.SmoothDamp(transform.position, desiredPos, ref velocity, TimeToGetToTarget);

        // Implement camera follow
        transform.position = newPos;
    }
}