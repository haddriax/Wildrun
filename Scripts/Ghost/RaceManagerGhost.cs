using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceManagerGhost : MonoBehaviour
{
    public bool raceFinished = true;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        raceFinished = false;
    }
}
