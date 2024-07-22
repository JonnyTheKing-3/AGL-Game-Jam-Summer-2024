using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

public class FMODtest : MonoBehaviour
{
    [SerializeField] private EventReference sound;
    
    // Start is called before the first frame update
    void Start()
    {
        RuntimeManager.PlayOneShotAttached(sound, gameObject);
    }
}
