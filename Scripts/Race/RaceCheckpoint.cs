using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class RaceCheckpoint : MonoBehaviour
{
    private RaceManager raceManager;
    public int Index = -1;
    public RaceCheckpoint Next;
    public RaceCheckpoint Last;
    private List<CharacterLapManager> characterLaps = new List<CharacterLapManager>();

    public List<GameObject> m_respawn;
    
    private void Awake()
    {
        raceManager = FindObjectOfType<RaceManager>();
        //Index++;
        //Debug.Log(Index);
    }

    public void SetNeighbors()
    {
        Next = (Index + 1 == raceManager.RaceCheckpoints.Length) ? raceManager.RaceCheckpoints[0] : raceManager.RaceCheckpoints[Index + 1];
        Last = (Index - 1 < 0) ? raceManager.RaceCheckpoints.Last() : raceManager.RaceCheckpoints[Index - 1];
    }

    private void OnTriggerEnter(Collider other)
    {
        //CharacterLapManager clm = other.transform.parent.GetComponentInChildren<CharacterLapManager>();
        CharacterLapManager clm = other.transform.GetComponentInParent<CharacterLapManager>();
        if (clm != null)
        {
            if (!characterLaps.Contains(clm) || (characterLaps.Contains(clm) && this == raceManager.RaceCheckpoints.First()))
            {
                clm.ReachCheckpoint(this);
                characterLaps.Add(clm);
            }
        }
    }

    internal void ClearPassage(CharacterLapManager _clm)
    {
        characterLaps.RemoveAll(x => x == _clm);
    }
}
