using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Linq;

public class PartsSelection : MonoBehaviour
{
    public class PartButtonType
    {
        public TypePart typePart;
        public Part part;
        public GameObject button;
    }
    [SerializeField] StatDisplayer PartStatDisplayer = null;

    [SerializeField] GameObject partPannel;
    [SerializeField] GameObject bodyButtonPrefab;
    [SerializeField] GameObject wingButtonPrefab;
    [SerializeField] GameObject backButtonPrefab;
    [SerializeField] GameObject reactorButtonPrefab;
    [SerializeField] PodCustomManager2 podCustomManager;

    [SerializeField] GameObject bodyContent;
    [SerializeField] GameObject reactorContent;
    [SerializeField] GameObject frontContent;
    [SerializeField] GameObject backContent;
    [SerializeField] GameObject engineContent;

    //List<Button> bodyButtons;
    //List<Button> reactorButtons;
    //List<Button> frontButtons;
    //List<Button> backButtons;

    public List<PartButtonType> partButtonTypes = null;

    private void Start()
    {
        List<Button> bodyButtons = bodyContent.GetComponentsInChildren<Button>(true).ToList();
        List<Button> reactorButtons = reactorContent.GetComponentsInChildren<Button>(true).ToList();
        List<Button> frontButtons = frontContent.GetComponentsInChildren<Button>(true).ToList();
        List<Button> backButtons = backContent.GetComponentsInChildren<Button>(true).ToList();
        List<Button> engineButtons = engineContent.GetComponentsInChildren<Button>(true).ToList();
        if (partButtonTypes != null)
            partButtonTypes.ForEach(x => Destroy(x.button));
        partButtonTypes = new List<PartButtonType>();

        int cptR = 0, cptF = 0, cptBo = 0, cptBa = 0, cptE = 0;

        List<Part> unlockedParts = SavedDatasManager.UnlockedParts;
        foreach (Part part in SavedDatasManager.AllParts)
        {
            Button btn;
            switch (part.TypePart)
            {
                case TypePart.BackWings:
                    btn = backButtons[cptBa];
                    cptBa++;
                    break;
                case TypePart.Reactor:
                    btn = reactorButtons[cptR];
                    cptR++;
                    break;
                case TypePart.FrontWings:
                    btn = frontButtons[cptF];
                    cptF++;
                    break;
                case TypePart.BaseFrame:
                    btn = bodyButtons[cptBo];
                    cptBo++;
                    break;
                case TypePart.Engine:
                    btn = engineButtons[cptE];
                    cptE++;
                    break;
                default:
                    btn = backButtons[0];
                    break;
            }

            if (unlockedParts.Contains(part))
                btn.onClick.AddListener(() => podCustomManager.PutElement(part));
            else
            {
                //btn.GetComponentsInChildren<Image>()[0].enabled = false;
                btn.GetComponentsInChildren<Image>(true)[2].enabled = true;
            }
            SelectableButton selectable = btn.gameObject.AddComponent<SelectableButton>();
            selectable.part = part;
            selectable.statDisplayer = PartStatDisplayer;

            btn.GetComponentsInChildren<Image>()[1].sprite = podCustomManager.partButtons.Find(x => x.IdPart == part.ID).ButtonImage;

            partButtonTypes.Add(new PartButtonType() { typePart = part.TypePart, button = btn.gameObject, part = part });
        };

        //// Navigation
        //for (int i = 0; i < partButtonTypes.Count; i++)
        //{
        //    PartButtonType pbt = partButtonTypes[i];
        //    if (pbt.typePart == TypePart.BaseFrame || pbt.typePart == TypePart.FrontWings) // Horizontal
        //    {
        //        Button btn = pbt.button.GetComponentInChildren<Button>();
        //        Button nextbtn = partButtonTypes[(i + 1) % partButtonTypes.Count].button.GetComponent<Button>();
        //        int n = (i - 1) % partButtonTypes.Count;
        //        Button lastbtn = partButtonTypes[(n < 0) ? partButtonTypes.Count + n : n].button.GetComponent<Button>();
        //        Navigation navigation = btn.navigation;

        //        navigation.mode = Navigation.Mode.Explicit;
        //        navigation.selectOnRight = nextbtn;
        //        navigation.selectOnLeft = lastbtn;
        //        btn.navigation = navigation;
        //    }
        //}

        Clear();
    }

    [Obsolete]
    private void InitButtonOLD()
    {
        if (partButtonTypes != null)
            partButtonTypes.ForEach(x => Destroy(x.button));
        partButtonTypes = new List<PartButtonType>();
        foreach (Part part in SavedDatasManager.UnlockedParts)
        {
            GameObject btn;
            //btn = Instantiate(backButtonPrefab, partPannel.transform, false);
            switch (part.TypePart)
            {
                case TypePart.BackWings:
                    btn = Instantiate(backButtonPrefab, backContent.transform, false);
                    break;
                case TypePart.Reactor:
                    btn = Instantiate(reactorButtonPrefab, reactorContent.transform, false);
                    break;
                case TypePart.FrontWings:
                    btn = Instantiate(wingButtonPrefab, frontContent.transform, false);
                    break;
                case TypePart.BaseFrame:
                    btn = Instantiate(bodyButtonPrefab, bodyContent.transform, false);
                    break;
                default: btn = Instantiate(bodyButtonPrefab, partPannel.transform, false); break;
            }
            //btn.GetComponentInChildren<Text>().text = part.name;
            btn.GetComponentInChildren<Button>().onClick.AddListener(() => podCustomManager.PutElement(part));

            btn.GetComponentsInChildren<Image>()[1].sprite = podCustomManager.partButtons.Find(x => x.IdPart == part.ID).ButtonImage;
            //switch (part.TypePart)
            //{
            //    case TypePart.BackWings: btn.GetComponent<Image>().color = Color.blue; break;
            //    case TypePart.Reactor: btn.GetComponent<Image>().color = Color.red; break;
            //    case TypePart.FrontWings: btn.GetComponent<Image>().color = Color.green; break;
            //    case TypePart.BaseFrame: btn.GetComponent<Image>().color = Color.white; break;
            //    default: btn.GetComponent<Image>().color = Color.magenta; break;
            //}

            partButtonTypes.Add(new PartButtonType() { typePart = part.TypePart, button = btn, part = part });
        }

        // Navigation
        for (int i = 0; i < partButtonTypes.Count; i++)
        {
            PartButtonType pbt = partButtonTypes[i];
            Button btn = pbt.button.GetComponentInChildren<Button>();
            Button nextbtn = partButtonTypes[(i + 1) % partButtonTypes.Count].button.GetComponent<Button>();
            int n = (i - 1) % partButtonTypes.Count;
            Button lastbtn = partButtonTypes[(n < 0) ? partButtonTypes.Count + n : n].button.GetComponent<Button>();
            Navigation navigation = btn.navigation;

            navigation.mode = Navigation.Mode.Explicit;
            navigation.selectOnRight = nextbtn;
            navigation.selectOnLeft = lastbtn;
            btn.navigation = navigation;
        }

        Clear();
    }

    public void ShowParts(TypePart _typePart)
    {
        Clear();
        switch (_typePart)
        {
            case TypePart.BaseFrame:
                bodyContent.SetActive(true);
                break;
            case TypePart.Reactor:
                reactorContent.SetActive(true);
                break;
            case TypePart.FrontWings:
                frontContent.SetActive(true);
                break;
            case TypePart.BackWings:
                backContent.SetActive(true);
                break;
            case TypePart.Engine:
                engineContent.SetActive(true);
                break;
            default:
                break;
        }
        partButtonTypes.FindAll(x => x.typePart == _typePart).ForEach(x => x.button.SetActive(true));
    }

    public void Clear()
    {
        partButtonTypes.ForEach(x => x.button.SetActive(false));
        backContent.SetActive(false);
        bodyContent.SetActive(false);
        engineContent.SetActive(false);
        frontContent.SetActive(false);
        reactorContent.SetActive(false);
    }
}
