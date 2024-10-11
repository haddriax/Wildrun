using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class SelectablePartButton : MonoBehaviour, ISelectHandler
{
    [SerializeField] PodCustomManager2 podCustomManager;
    [SerializeField] TypePart typePart;

    public void OnSelect(BaseEventData eventData)
    {
        podCustomManager.partsSelection.Clear();
        podCustomManager.partsSelection.ShowParts(typePart);
    }
}