using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyWhenFriendIsTouched : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
       if (GameManager.GotFriend) { Destroy(gameObject);}
    }
}
