using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PodOnCustom : MonoBehaviour
{
    public int ID = -999;
    public string PodName = "New Pod";

    public PodModel ToPodModel(GameObject _podSpawnPos = null)
    {
        Vector3 spawnPodPos = (_podSpawnPos != null) ? _podSpawnPos.transform.position : Vector3.zero;
        Part[] parts = GetComponentsInChildren<Part>();
        // -1 because the BaseFrame doesn't have Pivot.
        //if (parts.Length - 1 != GetComponentsInChildren<PodPivot>().Length) return null;
        GetComponentsInChildren<PodPivot>().Select(x => x.elementPut).ToList().ForEach(x => { if (x == null) return; });
        PodModel result = new PodModel();
        result.Name = PodName;
        result.ID = ID;
        result.IDBaseFrame = parts.ToList().Find(x => x.TypePart == TypePart.BaseFrame).ID;
        foreach (Part p in parts)
        {
            if (p.TypePart != TypePart.BaseFrame)
            {
                if (!result.Parts.ContainsKey(p.ID))
                {
                    result.Parts.Add(
                        p.ID, new List<float[]> { new float[] {
                            p.transform.position.x - spawnPodPos.x, p.transform.position.y + 1 - spawnPodPos.y, p.transform.position.z - spawnPodPos.z,
                            p.transform.localScale.x, p.transform.localScale.y, p.transform.localScale.z
                        } }
                    );
                }
                else
                    result.Parts[p.ID].Add(new float[] {
                        p.transform.position.x - spawnPodPos.x, p.transform.position.y + 1 - spawnPodPos.y, p.transform.position.z - spawnPodPos.z,
                        p.transform.localScale.x, p.transform.localScale.y, p.transform.localScale.z
                    });
            }
        }

        //List<Stat> tmpSats = new List<Stat>()
        //{
        //    new Stat(){ StatType = StatType.Acceleration,    Value = 0 },
        //    new Stat(){ StatType = StatType.MaxSpeed,        Value = 0 },
        //    new Stat(){ StatType = StatType.Turning,         Value = 0 },
        //    new Stat(){ StatType = StatType.Strength,        Value = 0 },
        //    new Stat(){ StatType = StatType.TurboPower,      Value = 0 },
        //    new Stat(){ StatType = StatType.TurboDuration, Value = 0 },
        //    new Stat(){ StatType = StatType.TurboCooldown,   Value = 0 }
        //};
        //foreach (Part p in parts)
        //{
        //    foreach(Stat stat in p.Stats)
        //    {
        //        tmpSats.Find(x => x.StatType == stat.StatType).Value += stat.Value;
        //    }
        //}
        //result.Stats = tmpSats;
        result.Stats = ToPodModelStats();
        return result;
    }

    internal List<Stat> ToPodModelStats()
    {
        Part[] parts = GetComponentsInChildren<Part>();
        List<Stat> result = new List<Stat>()
        {
            new Stat() { StatType = StatType.Acceleration, Value = 0 },
            new Stat() { StatType = StatType.MaxSpeed, Value = 0 },
            new Stat() { StatType = StatType.Turning, Value = 0 },
            new Stat() { StatType = StatType.Strength, Value = 0 },
            new Stat() { StatType = StatType.TurboPower, Value = 0 },
            new Stat() { StatType = StatType.TurboDuration, Value = 0 },
            new Stat() { StatType = StatType.TurboCooldown, Value = 0 }
        };
        foreach (Part p in parts)
        {
            foreach (Stat stat in p.Stats)
            {
                result.Find(x => x.StatType == stat.StatType).Value += !p.IsPair && p.TypePart != TypePart.BaseFrame ? stat.Value / 2f : stat.Value;
            }
        }
        return result;
    }
}
