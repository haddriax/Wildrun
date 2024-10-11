using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Linq;

public class PivotSelection : MonoBehaviour
{
    [Serializable]
    public class PivotButton
    {
        public TypePart typePart;
        public GameObject partPodButton;
    }

    [SerializeField] List<PivotButton> pivots;
    [SerializeField] PodCustomManager2 podCustomManager;

    public List<PivotButton> Pivots
    {
        get => pivots;
    }

    public void GenerateButtonActions()
    {
        PodOnCustom podOnCustom = podCustomManager.podOnCustom;
        foreach (PivotButton pivot in pivots)
        {
            Button button = pivot.partPodButton.GetComponentInChildren<Button>();
            button.onClick.RemoveAllListeners();
            if (pivot.typePart != TypePart.BaseFrame)
            {
                PivotSet ps = podOnCustom.GetComponentsInChildren<PivotSet>().ToList().Find(x => x.PartsAccepted == pivot.typePart);
                button.onClick.AddListener(() => podCustomManager.Select(ps, button));
            }
            else
            {
                button.onClick.AddListener(() => podCustomManager.Select(pivot.typePart, button));
            }
        }
        SetButtonSprites();
    }

    public void SetButtonSprites()
    {
        PodOnCustom podOnCustom = podCustomManager.podOnCustom;
        foreach (PivotButton pivot in pivots)
        {
            Image buttonImage = pivot.partPodButton.GetComponentInChildren<Button>().GetComponentsInChildren<Image>()[1];
            Part part = null;
            try
            {
                part = pivot.typePart == TypePart.BaseFrame
                    ? podOnCustom.GetComponentsInChildren<Part>().ToList().Find(x => x.TypePart == TypePart.BaseFrame)
                    : podOnCustom.GetComponentsInChildren<PodPivot>().ToList().Find(x => x.PartsAccepted == pivot.typePart).elementPut.GetComponent<Part>();

                int id = part ? part.ID : -1;
                if (id > -1)
                {
                    //Debug.Log(part.TypePart + ", id " + id);
                    buttonImage.sprite = podCustomManager.partButtons.Find(x => x.IdPart == id).ButtonImage;
                }
                else
                {
                    buttonImage.sprite = null;
                    switch (pivot.typePart)
                    {
                        case TypePart.BaseFrame:
                            buttonImage.sprite = podCustomManager.partButtons.Find(x => x.IdPart == -1).ButtonImage;
                            break;
                        case TypePart.Reactor:
                            buttonImage.sprite = podCustomManager.partButtons.Find(x => x.IdPart == -100).ButtonImage;
                            break;
                        case TypePart.FrontWings:
                            buttonImage.sprite = podCustomManager.partButtons.Find(x => x.IdPart == -200).ButtonImage;
                            break;
                        case TypePart.BackWings:
                            buttonImage.sprite = podCustomManager.partButtons.Find(x => x.IdPart == -300).ButtonImage;
                            break;
                        case TypePart.Engine:
                            buttonImage.sprite = podCustomManager.partButtons.Find(x => x.IdPart == -400).ButtonImage;
                            break;
                        default:
                            break;
                    }
                }
            }
            catch
            {

            }
        }
    }
}
