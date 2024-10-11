using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

public struct TimeAtCheckpoint
{
    public float time;
    public int lap;
    public int checkpoint;
}

public struct TimePerLap
{
    public float time;
    public int lap;
}

public class CharacterLapManager : MonoBehaviour
{
    private readonly string[] randomNames = { "Ekeo", "G3OLEX", "Hadriax", "Frozz", "Svarder", "Niraël", "Kaeki", "Max", "SwatX18" };

    private RaceManager raceManager;

    [SerializeField] private int currentLap;
    public RaceCheckpoint currentCheckpoint { get; private set; }
    private int rank;
    private bool finished;

    public string Name;
    public int CurrentLap { get => currentLap; }
    public int Rank { get => rank; }
    public bool Finished { get => finished; }
    public float TotalTime { get => totalTime;}

    // Timers
    private float currentLapTime;
    private List<TimeAtCheckpoint> timesPerCheckpoint;
    public List<TimeAtCheckpoint> TimesPerCheckpoint => timesPerCheckpoint;
    private List<TimePerLap> timesPerLap;
    public List<TimePerLap> TimesPerLap => timesPerLap;

    public bool isGoodWay { get; private set; }
    private float totalTime;

    public int checkpointsReached = 0;

    private void Start()
    {
        raceManager = FindObjectOfType<RaceManager>();
        timesPerCheckpoint = new List<TimeAtCheckpoint>();
        timesPerLap = new List<TimePerLap>();
        currentLap = 0;
        isGoodWay = true;
        currentCheckpoint = null;
        rank = FindObjectsOfType<CharacterLapManager>().Length;
        if (GetComponentInChildren<AI.AIController>())
            Name = randomNames[UnityEngine.Random.Range(0, randomNames.Length)] + " (AI)";
        else
            Name = "Joueur";
    }

    private void FixedUpdate()
    {
        if (!finished)
        {
            totalTime += Time.deltaTime;
            currentLapTime += Time.deltaTime;
            if (currentCheckpoint != null)
            {
                //Debug.Log(Vector3.Dot(gameObject.transform.forward, currentCheckpoint.transform.forward));
                if (Vector3.Dot(gameObject.transform.forward, currentCheckpoint.transform.forward) <= -0.8f) WrongWay();
                else GoodWay();
            }
        }
    }

    public void ReachCheckpoint(RaceCheckpoint _checkpoint)
    {
        // Nouveau Tour
        if (_checkpoint == raceManager.RaceCheckpoints.First() && currentCheckpoint == _checkpoint.Last && checkpointsReached >= raceManager.RaceCheckpoints.Length)
        {
            currentCheckpoint = _checkpoint;
            currentLap++;
            checkpointsReached = 1;
            SaveTimePerLap();
            if (currentLap >= raceManager.LapNumber)
                EndRace();
        }
        // Nouveau Checkpoint
        else if (currentCheckpoint == null || currentCheckpoint == _checkpoint.Last || currentCheckpoint == _checkpoint.Last.Last)
        {
            if (currentCheckpoint != null) SaveTimeAtCheckpoint();
            checkpointsReached ++;
            if (currentCheckpoint == _checkpoint.Last.Last)
                checkpointsReached++;
            currentCheckpoint = _checkpoint;
            foreach (RaceCheckpoint check in FindObjectsOfType<RaceCheckpoint>())
            {
                check.ClearPassage(this);
            }
        }
    }

    private void SaveTimeAtCheckpoint()
    {
        timesPerCheckpoint.Add(new TimeAtCheckpoint()
        {
            lap = currentLap,
            checkpoint = currentCheckpoint.Index,
            time = currentLapTime
        });
    }

    private void SaveTimePerLap()
    {
        timesPerLap.Add(new TimePerLap()
        {
            time = currentLapTime,
            lap = currentLap
        });
        currentLapTime = 0;
    }

    private void EndRace()
    {
        finished = true;
        raceManager.CharacterFinish(this);
        if (GetComponent<UserControllerRewired>() != null)
        {
            GetComponent<UserControllerRewired>().Disactiv();
            //GetComponent<AI.AIMovement>().followRoad = true;
            //GetComponent<VehicleBeta.MainController>().enabled = false;
            //GetComponent<Vehicle.MainController>().enabled = true;
        }
        else if (GetComponentInChildren<AI.AIMovement>() != null)
        {
            GetComponentInChildren<AI.AIMovement>().followRoad = false;
        }

        if (GetComponent<GhostRecorder>() != null)
            GetComponent<GhostRecorder>().EndRecord();
    }

    internal void SetRank(int rank)
    {
        this.rank = rank;
    }

    public void GoodWay()
    {
        isGoodWay = true;
    }

    public void WrongWay()
    {
        isGoodWay = false;
    }
}
