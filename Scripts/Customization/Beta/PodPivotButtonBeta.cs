using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PodPivotButtonBeta : MonoBehaviour, ISelectHandler/*, IDeselectHandler*/
{
    public TypePart typePart;
    public bool paint;
    public GameObject panel;

    private EventSystem eventSystem;
    private PanelsStorage panelsStorage;
    private CustomizationBeta customization;

    private void Start()
    {
        GetComponentInChildren<Button>().onClick.AddListener(() => OpenPanel());
        eventSystem = FindObjectOfType<EventSystem>();
        panelsStorage = GetComponentInParent<PanelsStorage>();
        customization = FindObjectOfType<CustomizationBeta>();
    }

    private void OpenPanel()
    {
        panel.SetActive(true);
        if (!paint)
            eventSystem.SetSelectedGameObject(panel.GetComponentInChildren<Button>().gameObject);
        customization.isOnPivotPanel = false;
        customization.btnToSelect = gameObject;
    }

    public void OnSelect(BaseEventData eventData)
    {

        try
        {
            panelsStorage.Clear();
        }
        catch { }
        panel.SetActive(true);
    }
}
