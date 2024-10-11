using UnityEngine;
using System.Collections;

public class Fx : MonoBehaviour
{
    public enum FxUsing
    {
        Event, Continuous
    }

    public enum FxType
    {
        HeatDistortion, Turbo
    }

    public FxType fxType;
    public FxUsing fxUsing;

    private void Start()
    {
        gameObject.SetActive(fxType == FxType.HeatDistortion && FindObjectOfType<RaceManager>());
    }
}
