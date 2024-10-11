using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.EventSystems;

public class PartButtonBeta : MonoBehaviour, ISelectHandler
{
    public int partID;
    CustomizationBeta customization;

    Part part => SavedDatasManager.AllParts.Find(x => x.ID == partID);

    private void Start()
    {
        customization = FindObjectOfType<CustomizationBeta>();
        GetComponentsInChildren<Image>()[1].sprite = customization.partButtons.Find(x => x.IdPart == partID).ButtonImage;
        //if (SavedDatasManager.UnlockedParts.Find(x => x.ID == partID))
        {
            GetComponentInChildren<Button>().onClick.AddListener(() => customization.PutPart(partID));
            GetComponentInChildren<Button>().onClick.AddListener(() => customization.SetButtonSprites());
            GetComponentsInChildren<Image>()[2].enabled = false;
        }
        //else
        //{
        //    GetComponentsInChildren<Image>()[2].enabled = true;
        //}
    }

    public void OnSelect(BaseEventData eventData)
    {
        try
        {
            customization.partStatDisplayer.Display(part.Stats);
        }
        catch { }
    }
}
