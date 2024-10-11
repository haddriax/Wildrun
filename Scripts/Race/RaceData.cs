using System;
using System.Collections.Generic;

public class RaceData
{
    public int ID = -99;
    public PodModel[] Models;
    public float[] TotalTimes;
    public List<TimeAtCheckpoint[]> timesPerCheckpoint;
    public List<TimePerLap[]> timesPerLap;
    public RaceType RaceType = RaceType.Free;
    public DateTime Date = DateTime.Now;
}
