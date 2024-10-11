using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CustomizationBeta : MonoBehaviour
{
    [Serializable] public class PartButton
    {
        /// <summary>
        /// > -100 -> Backwings Empty 
        /// > -200 -> Reactors Empty 
        /// > -300 -> Frontwings Empty 
        /// </summary>
        public int IdPart;
        public Sprite ButtonImage = null;
    }
    [Serializable] public class PanelToOpen
    {
        public Button button;
        public GameObject panel;
    }
    
    [HideInInspector] public PodOnCustom pod = null;
    [HideInInspector] public PodModel currentModel;
    [HideInInspector] public bool isOnPivotPanel = true;

    [SerializeField] private GameObject notSavedText;
    [SerializeField] private InputField podNameInputField;

    [SerializeField] public StatDisplayer podStatDisplayer;
    [SerializeField] public StatDisplayer partStatDisplayer;

    public List<PartButton> partButtons;
    public GameObject spawn;
    public GameObject btnToSelect;

    private bool saved = true;
    private bool notSavedAndPushB = false;
    private List<PodPivotButtonBeta> podPivotButtons;
    private EventSystem eventSystem = null;
    private SoundManagerFMOD managerFMOD;
    private PanelsStorage panelsStorage;

    private void Awake()
    {
        managerFMOD = SoundManagerFMOD.GetInstance();
    }

    private void Start()
    {
        eventSystem = FindObjectOfType<EventSystem>();
        podPivotButtons = FindObjectsOfType<PodPivotButtonBeta>().ToList();
        panelsStorage = FindObjectOfType<PanelsStorage>();

        eventSystem.SetSelectedGameObject(btnToSelect);
        GenerateCustomPod();

        podStatDisplayer.podOnCustom = pod;
    }

    private void OnEnable()
    {
        if (eventSystem)
            eventSystem.SetSelectedGameObject(btnToSelect);
    }

    private void Update()
    {
        if (Input.GetButtonDown("ButtonStart"))
        {
            NewPod();
        }

        if (Input.GetButtonDown("ButtonB"))
        {
            if (isOnPivotPanel)
            {
                Return();
            }
            else
            {
                isOnPivotPanel = true;
                eventSystem.SetSelectedGameObject(btnToSelect);
            }
            managerFMOD.PlayClickBackwardUI(transform);
        }

        if (Input.GetButtonDown("ButtonY")) SavePod();

        if (Input.GetButtonDown("ButtonRight"))
        {
            NextPod();
        }
        else if (Input.GetButtonDown("ButtonLeft"))
        {
            PreviousPod();
        }

        SetButtonSprites();
    }

    public void Return()
    {
        if (!saved && !notSavedAndPushB)
        {
            notSavedText.SetActive(true);
            StartCoroutine("NotSavedFading");

            notSavedAndPushB = true;
        }
        else if (notSavedAndPushB || saved)
        {
            StopAllCoroutines();
            Text text = notSavedText.GetComponent<Text>();
            text.color = new Color(text.color.r, text.color.g, text.color.b, 1);
            text.gameObject.SetActive(false);

            if (FindObjectOfType<AltMenuCanvasHandler>() != null)
            {
                FindObjectOfType<AltMenuCanvasHandler>().CloseCustomization();
            }
            else
            {
                LoadingScript.LoadNewScene(Scenes.MainMenu);
            }
        }
    }

    public void GenerateCustomPod(int _partID = -1)
    {
        currentModel = _partID <= -1
            ? SavedDatasManager.PodModels.First()
            : SavedDatasManager.PodModels.Find(x => x.ID == _partID);

        if(pod)
            Destroy(pod.gameObject);

        GameObject podGO = VehiculeGenerator.GeneratePodGameObject(currentModel, null, -1, spawn.transform);
        pod = podGO.AddComponent<PodOnCustom>();
        pod.PodName = currentModel.Name; 
        pod.ID = currentModel.ID;
        podNameInputField.text = currentModel.Name;
        SetButtonSprites();
    }

    public void SetButtonSprites()
    {
        foreach (PodPivotButtonBeta pivotButton in podPivotButtons)
        {
            if(!pivotButton.paint)
            {
                int partId;
                switch (pivotButton.typePart)
                {
                    case TypePart.BaseFrame:
                        partId = -1;
                        break;
                    case TypePart.BackWings:
                        partId = -100;
                        break;
                    case TypePart.FrontWings:
                        partId = -200;
                        break;
                    case TypePart.Reactor:
                        partId = -300;
                        break;
                    case TypePart.Engine:
                        partId = -400;
                        break;
                    default:
                        partId = -999;
                        break;
                }
                if (partId != -999)
                    pivotButton.GetComponentsInChildren<Image>()[1].sprite = partButtons.Find(x => x.IdPart == partId).ButtonImage;
            }
        }

        foreach (PodPivotButtonBeta pivotButton in podPivotButtons)
        {
            if (!pivotButton.paint)
            {
                try
                {
                    Part part = pod.GetComponentsInChildren<Part>().ToList().Find(x => x.TypePart == pivotButton.typePart);
                    int partID = part ? part.ID : -999;
                    if (partID != -999)
                        pivotButton.GetComponentsInChildren<Image>()[1].sprite = partButtons.Find(x => x.IdPart == partID).ButtonImage;
                }
                catch { }
            }
        }
    }

    public void PutPart(int _partID)
    {
        Part part = SavedDatasManager.GetPartByID(_partID);
        if (part.TypePart != TypePart.BaseFrame)
        {
            pod.GetComponentsInChildren<PivotSet>().ToList().Find(x => x.PartsAccepted == part.TypePart).PutElement(part);
        }
        else
        {
            SwitchBody(part);
            SwitchBody(part);
        }
        SetButtonSprites();
    }
    
    private void SwitchBody(Part p)
    {
        Part forward = pod.GetComponentsInChildren<Part>().ToList().Find(x => x.TypePart == TypePart.FrontWings);
        Part reactor = pod.GetComponentsInChildren<Part>().ToList().Find(x => x.TypePart == TypePart.Reactor);
        Part backing = pod.GetComponentsInChildren<Part>().ToList().Find(x => x.TypePart == TypePart.BackWings);
        Part engine = pod.GetComponentsInChildren<Part>().ToList().Find(x => x.TypePart == TypePart.Engine);

        int tmpPodId = currentModel.ID;
        string tmpPodName = currentModel.Name;
        NewPod(p.ID);
        pod.ID = tmpPodId;
        pod.PodName = tmpPodName;
        currentModel.ID = tmpPodId;
        currentModel.Name = tmpPodName;

        List<PodPivot> pivots = pod.GetComponentsInChildren<PodPivot>().ToList();
        if (forward != null)
            pivots.Find(x => x.PartsAccepted == TypePart.FrontWings).PutElement(forward);
        if (reactor != null)
            pivots.Find(x => x.PartsAccepted == TypePart.Reactor).PutElement(reactor);
        if (backing != null)
            pivots.Find(x => x.PartsAccepted == TypePart.BackWings).PutElement(backing);
        if (engine != null)
            pivots.Find(x => x.PartsAccepted == TypePart.Engine).PutElement(engine);
    }

    public void NewPod(int _ID = -1)
    {
        Destroy(pod.gameObject);
        GameObject podGO;
        if (_ID == -1)
            podGO = Instantiate(SavedDatasManager.GetPartByTypePart(TypePart.BaseFrame).gameObject, spawn.transform);
        else
            podGO = Instantiate(SavedDatasManager.GetPartByID(_ID).gameObject, spawn.transform);
        pod = podGO.AddComponent<PodOnCustom>();
        currentModel.ID = -999;
        currentModel.Name = "New Pod";
        podNameInputField.text = pod.PodName;
        podStatDisplayer.podOnCustom = pod;
        SetButtonSprites();
    }

    public void PreviousPod()
    {
        int maxId = SavedDatasManager.PodModels.Max(x => x.ID);
        int minId = SavedDatasManager.PodModels.Min(x => x.ID);
        int curentID = pod.ID;

        do
        {
            if (curentID == minId)
                curentID = maxId;
            else
                curentID--;
        }
        while (SavedDatasManager.PodModels.Find(x => x.ID == curentID) == null);
        GenerateCustomPod(curentID);
        podStatDisplayer.podOnCustom = pod;
    }

    public void NextPod()
    {
        int maxId = SavedDatasManager.PodModels.Max(x => x.ID);
        int minId = SavedDatasManager.PodModels.Min(x => x.ID);
        int curentID = pod.ID;

        do
        {
            if (curentID == maxId)
                curentID = minId;
            else
                curentID++;
        }
        while (SavedDatasManager.PodModels.Find(x => x.ID == curentID) == null);
        GenerateCustomPod(curentID);
        podStatDisplayer.podOnCustom = pod;
    }

    public void SavePod()
    {
        PodModel podModel = pod.ToPodModel(spawn);
        if (podModel == null || podModel == new PodModel())
        {
            return;
        }
        //podModel.Name = podName.text;
        SavedDatasManager.SavePodModels(ref podModel);
        pod.ID = podModel.ID;
        pod.name = podModel.Name;
        saved = true;
        notSavedAndPushB = false;
    }
}