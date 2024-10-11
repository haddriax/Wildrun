using System;
using System.Collections.Generic;
using UnityEngine;

public class PodModel
{
    public int ID = -999;
    public string Name = "None";
    public int IDBaseFrame = 0;
    public List<Stat> Stats = new List<Stat>();
    public Dictionary<int, List<float[]>> Parts = new Dictionary<int, List<float[]>>();
    public bool Deletable = true;
}
