using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformManaging : MonoBehaviour
{
    /* TO MOVE A PLATFORM
     *      1) PLACE THIS SCRIPT ON AN EMPTY OBJECT (let's call it destination)
     *      2) MAKE destination OBJECT A CHILD OF THE PLATFORM YOU WANT TO MOVE
     *      3) SET THE POSITION OF destination TO WHEREVER YOU WANT THE PLATFORM TO MOVE TO
     */
    
    public Transform OriginalSpot;
    public bool IWantThisPlatformToMove = false;
    public bool StartMoving = false;
    public float travelSpeed = 0.5f;

    void Start()
    {
        // Get the transform of the parent
        OriginalSpot = transform.parent.transform;
    }

    void Update()
    {
        if (!StartMoving) { return; }
        
        if (StartMoving && IWantThisPlatformToMove)
        {
            StartMoving = false;
            IWantThisPlatformToMove = false;
            StartCoroutine(MovePlatform());
        }
    }

    public IEnumerator MovePlatform()
    {
        // initializing temporary placeholders for lerp
        Vector3 initialPos = OriginalSpot.position;
        Vector3 targetPos = transform.position;
        float elapsedTime = 0;

        while (elapsedTime < 1)
        {
            OriginalSpot.position = Vector3.Lerp(initialPos, targetPos, elapsedTime);
            elapsedTime += Time.deltaTime * travelSpeed;
            yield return null;
        }

        // Ensure the player ends up at the exact target rotation
        OriginalSpot.position = targetPos;
    }
}
