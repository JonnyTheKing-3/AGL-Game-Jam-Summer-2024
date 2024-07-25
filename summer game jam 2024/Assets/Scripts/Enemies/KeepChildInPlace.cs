using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepChildInPlace : MonoBehaviour
{
    public Vector3 OGpos;
    void Start()
    {
        OGpos = transform.position;
    }

    void Update()
    {
        transform.position = OGpos;
    }
}
