using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Ghost
{
    public int raceID;
    public float timerOffsetSave;
    public float totalTime;
    public List<float[]> positions;
    public List<float[]> quaternions;
}

public class GhostRecorder : MonoBehaviour
{
    private float timer = 0.0f;
    private float timerOffsetSave = 0.015f;
    Ghost ghost;
    CharacterLapManager lapManager;

    private void Start()
    {
        ghost = new Ghost();
        ghost.raceID = 0;
        ghost.timerOffsetSave = timerOffsetSave;
        ghost.positions = new List<float[]>();
        ghost.quaternions = new List<float[]>();
    lapManager = GetComponent<CharacterLapManager>();
    }

    private void FixedUpdate()
    {
        if (!lapManager.Finished)
        {
            timer += Time.deltaTime;
            if (timer >= timerOffsetSave)
            {
                RecordCurrentDatas();
                timer -= timerOffsetSave;
            }
        }
    }

    private void RecordCurrentDatas()
    {
        ghost.totalTime = GetComponent<CharacterLapManager>().TotalTime;
        Vector3 pos = gameObject.transform.position;
        Quaternion quaternion = gameObject.transform.rotation;
        ghost.positions.Add(new float[3] { pos.x, pos.y, pos.z });
        ghost.quaternions.Add(new float[4] { quaternion.x, quaternion.y, quaternion.z, quaternion.w });
    }

    internal void EndRecord()
    {
        RecordCurrentDatas();
        SavedDatasManager.SaveGhost(ghost);
    }
}
