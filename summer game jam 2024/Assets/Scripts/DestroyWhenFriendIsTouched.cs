using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyWhenFriendIsTouched : MonoBehaviour
{
    public GameObject []listOfObjectsToDestroy;
    void Update()
    {
        if (GameManager.GotFriend)
        {
            foreach (GameObject obj in listOfObjectsToDestroy)
            {
                Destroy(obj);
            }
            Destroy(gameObject);
        }
    }
}
