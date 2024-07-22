using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class FMODbanks : MonoBehaviour
{
    public static FMODbanks Instance { get; private set; }
    public EventReference jumpSFX;
    public EventReference hoverSFX;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }
    
    public void PlayJumpSFX(GameObject OriginOfSound)
    {
        RuntimeManager.PlayOneShotAttached(jumpSFX, OriginOfSound);
    }

    private EventInstance hoverInstance;
    public void PlayHoverSFX()
    {
        // Create the EventInstance for hover sound and start it
        hoverInstance = RuntimeManager.CreateInstance(hoverSFX);
        hoverInstance.start();
    }
    public void StopHoverSFX()
    {
        // If the hover sound is playing, stop it, and release the instance
        if (hoverInstance.isValid())
        {
            hoverInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            hoverInstance.release();
        }
    }
}
