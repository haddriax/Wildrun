using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PositionType { Forward, Middle, Back, Right, Left, Orbital, None }

public class PivotSet : MonoBehaviour
{
    [SerializeField] PositionType position;
    public PositionType Position => position;
    [SerializeField] TypePart partsAccepted;
    public TypePart PartsAccepted { get => partsAccepted; }
    //PodPivot[] PodPivots;
    public PodPivot[] PodPivots { get => GetComponentsInChildren<PodPivot>(); }
    //public PodPivot[] PodPivots { get => podPivots; }

    private void Start()
    {
        //PodPivots = GetComponentsInChildren<PodPivot>();
        foreach (PodPivot pp in PodPivots)
        {
            pp.SetPartsAccepted(partsAccepted);
        }
    }

    public void PutElement(Part podPart)
    {
        if (podPart.IsPair)
        {
            for (int i = 0; i < PodPivots.Length; i++)
            {
                if (PodPivots[i].elementPut != null)
                    Destroy(PodPivots[i].elementPut);
                GameObject g = Instantiate(podPart.gameObject, transform);
                g.transform.position = PodPivots[i].transform.position;
                if (i == 0)  
                    g.transform.localScale = new Vector3(-g.transform.localScale.x, g.transform.localScale.y, g.transform.localScale.z);
                PodPivots[i].elementPut = g;
            }
        }
        else
        {
            GameObject g = Instantiate(podPart.gameObject, transform);
            for (int i = 0; i < PodPivots.Length; i++)
            {
                if (PodPivots[i].elementPut != null)
                    Destroy(PodPivots[i].elementPut);
                PodPivots[i].elementPut = g;
            }
        }
    }
}
