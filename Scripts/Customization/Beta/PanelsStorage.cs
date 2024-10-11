using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PanelsStorage : MonoBehaviour
{
    [SerializeField] List<GameObject> panels;

    public void Clear()
    {
        panels.ForEach(x => x.SetActive(false));
    }
}
