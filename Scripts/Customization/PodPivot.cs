using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PodPivot : MonoBehaviour
{
    [SerializeField] PositionType position;
    public PositionType Position => position;

    TypePart partsAccepted;
    //public TypePart PartsAccepted { get => partsAccepted; }
    public TypePart PartsAccepted { get => PivotSet.PartsAccepted; }
    public PivotSet PivotSet { get => GetComponentInParent<PivotSet>(); }
    public GameObject elementPut = null;

    private void Start()
    {
        //PivotSet = GetComponentInParent<PivotSet>();
    }

    public void PutElement(Part podPart)
    {
        if (PartsAccepted == podPart.TypePart)
        {
            PivotSet.PutElement(podPart);
        }
    }

    public void SetPartsAccepted(TypePart partsAccepted)
    {
        this.partsAccepted = partsAccepted;
    }
}
