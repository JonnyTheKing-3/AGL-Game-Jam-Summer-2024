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
    
    // Regulate the speed of the platform throughout the travel
    [SerializeField] private AnimationCurve curve;
    public Transform OriginalSpot;
    public bool IWantThisPlatformToMove = false;
    public bool StartMoving = false;
    public float duration = 0.5f;

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
        // Remembers the starting position before shake
        Vector3 initialPos = OriginalSpot.position;
        Vector3 targetPos = transform.position;
        float elapsedTime = 0f;

        // Keep "shaking camera" for desired duration
        while (elapsedTime < duration)
        {
            // Strength variable is used as a regulator for the intensity of the shake
            float strength = curve.Evaluate(elapsedTime / duration);
            OriginalSpot.position = Vector3.Lerp(initialPos, targetPos, elapsedTime * strength);

            elapsedTime += Time.deltaTime;
            
            yield return null;
        }

        OriginalSpot.position = targetPos;
    }
}
