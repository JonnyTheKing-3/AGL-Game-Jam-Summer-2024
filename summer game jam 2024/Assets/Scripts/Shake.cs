using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Shake : MonoBehaviour
{
    // Regulate shake effect strength. May add more to give more type of shake effects (constant, rapid, etc)
    [SerializeField] private AnimationCurve curve;

    [SerializeField] private float duration = 1f;
    [SerializeField] private float intensity = 1f;
    
    // Only call one of the two methods at a time. If you call one method before 
    [SerializeField] private bool StartStationaryShake = false;
    [SerializeField] private bool StartFollowShake = false;
    

    // If either bool turns true, start the respective camera shake method
    private void Update()
    {
        if (StartStationaryShake) { StartStationaryShake = false; StartCoroutine(StationaryShaking(duration, intensity)); }
        if (StartFollowShake) { StartFollowShake = false; StartCoroutine(FollowingWhileShaking(duration, intensity)); }
    }
    

    // Use when camera is going to be stationary
    IEnumerator StationaryShaking(float duration, float intesnsity)
    {
        // Remembers the starting position before shake
        Vector3 StartingPos = transform.position;
        float elapsedTime = 0f;

        // Keep "shaking camera" for desired duration
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            
            // Strength variable is used as a regulator for the intensity of the shake
            float strength = curve.Evaluate(elapsedTime / duration) * intensity;
            // Move camera in a random direction within a small 3D radius every frame
            transform.position = StartingPos + Random.insideUnitSphere * strength;
            
            yield return null;
        }
        // Return camera to original spot
        transform.position = StartingPos;
        
        // If this is for a moving platform, start the moveplatform coroutine
        if (transform.GetChild(0).TryGetComponent(out MovingPlatformManaging mpm))
        {
            mpm.StartMoving = true;
        }
    }
    
    
    // Use when camera is going to be following something (like the player).
    // Going to have have to give less intensity to achieve a similar effect as it's stationary counter part (around 1/4ish to 1/3ish of stationary counterpart)
    IEnumerator FollowingWhileShaking(float duration, float intesnsity)
    {
        // Remembers the starting position before shake
        Vector3 StartingPos = transform.position;
        // Remembers the starting position before shake
        float elapsedTime = 0f;

        // Keep "shaking camera" for desired duration
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            
            // Strength variable is used as a regulator for the intensity of the shake
            float strength = curve.Evaluate(elapsedTime / duration) * intensity;
            // Move camera in a random direction within a small 3D radius every frame
            transform.position += Random.insideUnitSphere * strength; // May use insideUnitCircle method which (only affect x and y axis, no z)
            
            yield return null;
        }

        // Return camera to original spot
        transform.position = StartingPos;
        
        // If this is for a moving platform, start the moveplatform coroutine
        if (transform.GetChild(0).TryGetComponent(out MovingPlatformManaging mpm))
        {
            mpm.StartMoving = true;
        }
    }
}
