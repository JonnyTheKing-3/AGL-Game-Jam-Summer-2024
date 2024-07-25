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
    public EventReference DeathByCrushinSFX;
    public EventReference GravitySwitchSFX;
    public EventReference platformFlyingSFX;
    public EventReference Music;
    public EventReference Victory;

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
    
    public void PlayGravitySwitchSFX(GameObject OriginOfSound)
    {
        RuntimeManager.PlayOneShotAttached(GravitySwitchSFX, OriginOfSound);
    }
    
    public void PlayPlatformFlyingSFX(GameObject OriginOfSound)
    {
        RuntimeManager.PlayOneShotAttached(platformFlyingSFX, OriginOfSound);
    }

    public void PlayVictoryTheme()
    {
        RuntimeManager.PlayOneShotAttached(Victory, gameObject);
    }
    
    private EventInstance musicInstance;
    public void PlayMusic()
    {
        // Create the EventInstance for hover sound and start it
        musicInstance = RuntimeManager.CreateInstance(Music);
        musicInstance.start();
    }
    
    public void SetParameterOfMusicToOne()
    {
        // Set the parameter value
        musicInstance.setParameterByName("Music", 1);
    }

    public void OnSceneSwitch()
    {
        StopHoverSFX();
        hoverInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        hoverInstance.release();
        musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        musicInstance.release();
    }
    
    public void OnDestroy()
    {
        // Make sure to release the instance when it's no longer needed
        musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        musicInstance.release();
    }

    public void DeathSound()
    {
        RuntimeManager.PlayOneShotAttached(jumpSFX, gameObject);
    }
    
    
}
