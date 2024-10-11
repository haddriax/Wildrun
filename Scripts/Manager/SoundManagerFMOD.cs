using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

//Path
/*Example path will be contain in this commentary:
 event:/Soundtrack/SoundTrackMenu2D (called for the soundtracks, set in 2D to skip the sound spatialization)
 event:/FX/InGame/Vehicle (this one has to be call in the Start, look at AI controller)
 event:/Choc (this one has to be call through a Function OneShot already present in the Manager)
 event:/FX/UI/UI_Accept (this one has to be call through a Function OneShot already present in the Manager*/

//FMOD Listener
/*To create an Studio Listener, just go on the prefab or the gameobject 
 and do Add Component, and write Studio Event Listener and add. This is 
 a C# script ready to use. Better use on the camera for the player.*/
public class SoundManagerFMOD
{
    private static SoundManagerFMOD instance;

    public static SoundManagerFMOD GetInstance()
    {
        if (instance == null)
        {
            instance = new SoundManagerFMOD();
        }
        return instance;
    }

    //Summary
    /*Declaration of the all long Event called in the game,
     * in a List of EventInstance,
     * stack in the memory
     * and declaration of their FMOD parameter*/

    #region Variables
    List<FMOD.Studio.EventInstance> eventInstance = new List<FMOD.Studio.EventInstance>();

    FMOD.Studio.System system;
    FMOD.Studio.ParameterInstance acceleration;
    FMOD.Studio.ParameterInstance burst;
    FMOD.Studio.ParameterInstance accelerationAI;
    FMOD.Studio.ParameterInstance burstAI;

    FMOD.ATTRIBUTES_3D attributes;


    public bool alreadyUsing = false;
    public bool motorUsing = false;
    public bool AImotorUsing = false;
    public bool startRace = false;
    public bool collisionIsPlaying = false;
    #endregion

    //Summary
    /*Creation of the Instance
     * and return the index*/
    public int InitInstance(string path)
    {
        if (path == "event:/Soundtrack/SoundTrackMenu2D") alreadyUsing = true;

        if (path == "event:/FX/InGame/Vehicle2") motorUsing = true;

        if (path == "event:/FX/InGame/Vehicle2AI") AImotorUsing = true;

        eventInstance.Add(RuntimeManager.CreateInstance(path));

        return eventInstance.Count - 1;
    }

    //Summary
    /*Activation of the sound needed at start,
     * and create the 3D Attributes
     * Only use .start() here*/
    public void PlayInstance(int currentInstance, Transform instanceTR, Rigidbody instanceRB, bool Is2D = false)
    {
        if (!Is2D)
        {
            eventInstance[currentInstance].set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(instanceTR.gameObject));
        }

        RuntimeManager.AttachInstanceToGameObject(eventInstance[currentInstance], instanceTR, instanceRB);
        eventInstance[currentInstance].start();
    }

    //Summary
    /*Modification of the sound, or calling sound which is playing once
     * float value needed for a graduate changement of the sound
     * int value needed as boolean for an instant effect
     * PlayOneShot need the path of the event played and the position where to play it*/

    #region Parameter
    public void SetAcceleration(float value, int current)
    {
        eventInstance[current].getParameter("Acceleration", out acceleration);

        acceleration.setValue(value);
    }

    public void SetBurst(int value, int current)
    {
        eventInstance[current].getParameter("Burst", out burst);

        burst.setValue(value);
    }

    public void SetAccelerationAI(float value, int current)
    {
        eventInstance[current].getParameter("AccelerationAI", out accelerationAI);

        acceleration.setValue(value);
    }

    public void SetBurstAI(int value, int current)
    {
        eventInstance[current].getParameter("BurstAI", out burstAI);

        burst.setValue(value);
    }
    #endregion

    #region SetVolume
    public void SetVolumeMaster(float value)
    {
        string currentBus = "Bus:/";
        FMOD.Studio.Bus bus;

        bus = RuntimeManager.GetBus(currentBus);
        bus.setVolume(value);
    }

    public void SetVolumeSoundTrack(float value)
    {
        string currentBus = "Bus:/Soundtrack";
        FMOD.Studio.Bus bus;

        bus = RuntimeManager.GetBus(currentBus);
        bus.setVolume(value);
    }

    public void SetVolumeFX(float value)
    {
        string currentBus = "Bus:/FX";
        FMOD.Studio.Bus bus;

        bus = RuntimeManager.GetBus(currentBus);
        bus.setVolume(value);
    }

    public void SetVolumeAmbiant(float value)
    {
        string currentBus = "Bus:/Ambiant";
        FMOD.Studio.Bus bus;

        bus = RuntimeManager.GetBus(currentBus);
        bus.setVolume(value);
    }
    #endregion

    #region GetVolume
    public float GetVolumeMaster()
    {
        float value, finalVolume;
        string currentBus = "Bus:/";
        FMOD.Studio.Bus bus;

        bus = RuntimeManager.GetBus(currentBus);
        bus.getVolume(out value, out finalVolume);
        return finalVolume;
    }

    public float GetVolumeSoundTrack()
    {
        float value, finalVolume;
        string currentBus = "Bus:/Soundtrack";
        FMOD.Studio.Bus bus;

        bus = RuntimeManager.GetBus(currentBus);
        bus.getVolume(out value, out finalVolume);
        return finalVolume;
    }

    public float GetVolumeFX()
    {
        float value, finalVolume;
        string currentBus = "Bus:/FX";
        FMOD.Studio.Bus bus;

        bus = RuntimeManager.GetBus(currentBus);
        bus.getVolume(out value, out finalVolume);
        return finalVolume;
    }

    public float GetVolumeAmbiant()
    {
        float value, finalVolume;
        string currentBus = "Bus:/Ambiant";
        FMOD.Studio.Bus bus;

        bus = RuntimeManager.GetBus(currentBus);
        bus.getVolume(out value, out finalVolume);
        return finalVolume;
    }
    #endregion

    #region OneShot
    public void PlayRaceLaunch(Transform tr)
    {
        RuntimeManager.PlayOneShot("event:/FX/InGame/RaceLaunch", tr.position);
    }

    public void PlayImpact(Transform tr)
    {
        RuntimeManager.PlayOneShot("event:/FX/InGame/Choc3", tr.position);
    }

    public void PlayBush(Transform tr)
    {
        RuntimeManager.PlayOneShot("event:/FX/InGame/BushThrough", tr.position);
    }

    public void PlayGetFireworksLoot(Transform tr)
    {
        RuntimeManager.PlayOneShot("event:/FX/InGame/FireworksLoot", tr.position);
    }

    public void PlayGetBoxLoot(Transform tr)
    {
        RuntimeManager.PlayOneShot("event:/FX/InGame/BoxLoot", tr.position);
    }

    public void PlaySandstorm(Transform tr)
    {
        RuntimeManager.PlayOneShot("event:/Ambiant/Sandstorm", tr.position);
    }

    public void PlayClickForwardUI(Transform tr)
    {
        RuntimeManager.PlayOneShot("event:/FX/UI/UI_Accept", tr.position);
    }

    public void PlayClickBackwardUI(Transform tr)
    {
        RuntimeManager.PlayOneShot("event:/FX/UI/UI_Deny", tr.position);
    }

    public void PlayClickErrorUI(Transform tr)
    {
        RuntimeManager.PlayOneShot("event:/FX/UI/UI_Error", tr.position);
    }

    public void PlayMovesUI(Transform tr)
    {
        RuntimeManager.PlayOneShot("event:/FX/UI/UI_Moves", tr.position);
    }
    #endregion

    #region Effect
    public void StopSound()
    {
        for (int i = 0; i < eventInstance.Count; i++)
        {
            eventInstance[i].stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
        alreadyUsing = false;
        startRace = false;
        motorUsing = false;
        AImotorUsing = false;
    }

    public void PauseSound()
    {
        for (int i = 0; i < eventInstance.Count; i++)
        {
            eventInstance[i].setPaused(true);
        }
    }

    public void PlaySound()
    {
        for (int i = 0; i < eventInstance.Count; i++)
        {
            eventInstance[i].setPaused(false);
        }
    }
    #endregion
}