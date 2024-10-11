using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class SelectableButton : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public StatDisplayer statDisplayer = null;
    public Part part = null;

    public void OnSelect(BaseEventData eventData)
    {
        statDisplayer.Display(part.Stats);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        statDisplayer.DisactivateDisplay();
    }
}
