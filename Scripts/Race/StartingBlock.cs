using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartingBlock : MonoBehaviour
{
    [SerializeField] int index = 0;
    public int podId = -1;
    public int Index => index;
    public bool AlreadyUsed = false;
}
