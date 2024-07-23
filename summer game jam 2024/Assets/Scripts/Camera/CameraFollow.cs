using System;
using UnityEngine;

public class SmoothCameraFollow : MonoBehaviour
{
    // Target for the camera to follow. USUALLY IS PLAYER, SO DRAG PLAYER INTO THIS SPOT
    [SerializeField] private Transform target;
    
    // How fast the camera should reach the character
    public float TimeToGetToTarget = .1f;
    public float RotationSpeed = 4f;
    
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
    {
        Vector3 desiredPos = new Vector3(target.position.x + xOffset, target.position.y + yOffset, (-10 +zOffset));;
        
        // The position and rotation of the object to reach
        switch (target.rotation.eulerAngles.z)
        {
            case 0f:
                desiredPos = new Vector3(target.position.x + xOffset, target.position.y + yOffset, (-10 +zOffset));
                break;
            case 180f:
                desiredPos = new Vector3(target.position.x - xOffset, target.position.y - yOffset, (-10 +zOffset));
                break;
            case 90f:
                desiredPos = new Vector3(target.position.x - yOffset, target.position.y + xOffset, (-10 +zOffset));
                break;
            case 270f:
                desiredPos = new Vector3(target.position.x + yOffset, target.position.y - xOffset, (-10 +zOffset));
                break;
        }
        
        Quaternion desiredRot = target.rotation;

        // Get camera follow 
        Vector3 newPos = Vector3.SmoothDamp(transform.position, desiredPos, ref velocity, TimeToGetToTarget);
        Quaternion newRot = Quaternion.Slerp(transform.rotation, desiredRot, RotationSpeed * Time.deltaTime);

        // Implement camera follow and rotation
        transform.position = newPos;
        transform.rotation = newRot;
    }
}