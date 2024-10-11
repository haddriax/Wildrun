using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using System;

public enum StatType
{
    Acceleration,
    MaxSpeed,
    Turning,
    Strength,
    TurboPower,
    TurboDuration,
    TurboCooldown,
}

public enum TypePart
{
    BaseFrame,
    Reactor,
    FrontWings,
    BackWings,
    Engine
}

[Serializable]
public class Stat
{
    public StatType StatType;
    public float Value;
}

public class Part : MonoBehaviour
{
    public int ID = 999;
    public string Name;
    public TypePart TypePart;
    //public bool IsUnlocked = false;
    public int Price = 0;
    public bool IsPair = false;
    public List<Stat> Stats;
}