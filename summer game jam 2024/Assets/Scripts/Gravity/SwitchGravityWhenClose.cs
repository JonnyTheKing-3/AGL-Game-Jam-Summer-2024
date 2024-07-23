using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchGravityWhenClose : MonoBehaviour
{
    public enum Gravity
    {
        down = 0,
        up = 180,
        right = 90,
        left = 270
    }
    [SerializeField] private Gravity Gravitational_Pull;
    [SerializeField] private float rotationSpeed = 2.0f; // Speed at which the player rotates

    private void OnTriggerEnter2D(Collider2D other)
    {
        // If the player touches this trigger's collider, start the rotation coroutine
        if (other.gameObject.CompareTag("Player"))
        {
            float targetAngle = (float)Gravitational_Pull;
            StartCoroutine(RotatePlayer(other.transform, targetAngle));
        }
    }

    private IEnumerator RotatePlayer(Transform playerTransform, float targetAngle)
    {
        Quaternion initialRotation = playerTransform.rotation;
        Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);
        float elapsedTime = 0;

        while (elapsedTime < 1)
        {
            playerTransform.rotation = Quaternion.Slerp(initialRotation, targetRotation, elapsedTime);
            elapsedTime += Time.deltaTime * rotationSpeed;
            yield return null;
        }

        // Ensure the player ends up at the exact target rotation
        playerTransform.rotation = targetRotation;
    }
}