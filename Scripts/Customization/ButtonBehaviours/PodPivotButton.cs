using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PodPivotButton : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public PositionType camPosition;
    Customization.CameraManager camManager;

    private void Start()
    {
        camManager = FindObjectOfType<Customization.CameraManager>();
    }

    public void OnSelect(BaseEventData eventData)
    {
        try
        {
            camManager.SetCurrentCamera(camPosition);
        }
        catch { }
    }

    public void OnDeselect(BaseEventData eventData)
    {

    }
}
