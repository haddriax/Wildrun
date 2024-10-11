using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Runtime.InteropServices;
using UnityEngine.EventSystems;

public class PodCustomManager : MonoBehaviour
{
    SoundManagerFMOD managerFMOD;
    Customization.CameraManager camManager;

    [SerializeField] InputField podName;
    [SerializeField] GameObject inventoryContainer;
    [SerializeField] GameObject podPartContainer;
    [SerializeField] GameObject prefabPanel;
    [SerializeField] GameObject statPanel;
    [SerializeField] GameObject statBarPrefab;
    [SerializeField] GameObject NotSaved;

    PodPivot selectedPodPivot;
    PodOnCustom podOnCustom;
    static public int CurrentPodCustomized = -999;
    static public int CurrentBaseCustomized = -999;

    public bool saved = true;
    private bool notSavedAndPushB = true;
    GameObject lastSelected;
    EventSystem eventSystem;
    Dictionary<StatType, GameObject> statsBars = new Dictionary<StatType, GameObject>();

    private void Awake()
    {
        managerFMOD = SoundManagerFMOD.GetInstance();
    }

    private void Start()
    {
        camManager = FindObjectOfType<Customization.CameraManager>();

        //if (CurrentPodCustomized == -999)
        //    CurrentPodCustomized = SavedDatasManager.PodModels.First().ID;
        if (CurrentBaseCustomized == -999)
            CurrentBaseCustomized = SavedDatasManager.PodModels.First().IDBaseFrame;
            //CurrentBaseCustomized = SavedDatasManager.GetPartsByTypePart(TypePart.BaseFrame).First().ID;
        selectedPodPivot = null;
        GenerateCustomPod();
        GeneratePodPivotsPanel();
        GenerateStatsPanel();
        //podOnCustom = FindObjectOfType<PodOnCustom>();
        eventSystem = FindObjectOfType<EventSystem>();
    }

    private void Update()
    {
        GamePadCommand();
        UpdateStatsPanel();
    }
    
    // Controls
    private void GamePadCommand()
    {
        if (Input.GetButtonDown("ButtonStart"))
        {
            NewPod();
        }
        if (Input.GetButtonDown("ButtonB"))
        {
            if (lastSelected == null)
            {
                if (selectedPodPivot == null)
                    Return();
                else
                {
                    selectedPodPivot = null;
                    eventSystem.SetSelectedGameObject(null);
                    camManager.SetCurrentCamera(PositionType.Orbital);
                }
            }
            else
            {
                eventSystem.SetSelectedGameObject(lastSelected);
                lastSelected = null;
            }
            managerFMOD.PlayClickBackwardUI(transform);
        }
        if (Input.GetButtonDown("ButtonY")) SavePod();
        if (Input.GetButtonDown("ButtonX")) GoToRace();
        if ((Input.GetAxis("VerticalCross") != 0 || Input.GetAxis("HorizontalCross") != 0) && selectedPodPivot == null)
        {
            eventSystem.SetSelectedGameObject(partPodButtons.First());
            selectedPodPivot = pivotSets.First().PodPivots.First();
        }

        if (Input.GetButtonDown("ButtonRight"))
        {
            int maxId = SavedDatasManager.PodModels.Max(x => x.ID);
            int minId = SavedDatasManager.PodModels.Min(x => x.ID);

            do
            {
                if (CurrentPodCustomized == maxId)
                    CurrentPodCustomized = minId;
                else
                    CurrentPodCustomized++;
            }
            while (SavedDatasManager.PodModels.Find(x => x.ID == CurrentPodCustomized) == null);
            ReGeneratePod();
        }
        else if (Input.GetButtonDown("ButtonLeft"))
        {
            int maxId = SavedDatasManager.PodModels.Max(x => x.ID);
            int minId = SavedDatasManager.PodModels.Min(x => x.ID);

            do
            {
                if (CurrentPodCustomized == minId)
                    CurrentPodCustomized = maxId;
                else
                    CurrentPodCustomized--;
            }
            while (SavedDatasManager.PodModels.Find(x => x.ID == CurrentPodCustomized) == null);
            ReGeneratePod();
        }
    }

    // Generation
    public void GenerateCustomPod()
    {
        // When you choose an existant Pod
        GameObject baseFrame;
        if (CurrentPodCustomized != -999)
        {
            PodModel podM;
            podM = SavedDatasManager.GetPodModelById(CurrentPodCustomized);
            baseFrame = Instantiate(SavedDatasManager.GetPartByID(podM.IDBaseFrame).gameObject);
            baseFrame.AddComponent<PodOnCustom>();
            baseFrame.GetComponent<PodOnCustom>().ID = CurrentPodCustomized;
            podName.text = podM.Name;
            VehiculeGenerator.GeneratePodGameObject(podM, baseFrame);
            baseFrame.transform.SetPositionAndRotation(Vector3.up, Quaternion.identity);
        }
        else // When you create new Pod
        {
            baseFrame = GameObject.Instantiate(SavedDatasManager.GetPartByID(CurrentBaseCustomized).gameObject, Vector3.up, Quaternion.identity);
            baseFrame.AddComponent<PodOnCustom>();
            baseFrame.GetComponent<PodOnCustom>().ID = CurrentPodCustomized;
            baseFrame.GetComponent<PodOnCustom>().PodName = "NewPod";
            podName.text = "NewPod";
        }

        camManager.SetCamerasTarget(PositionType.Orbital, baseFrame);
        foreach (PivotSet pivotSet in baseFrame.GetComponentsInChildren<PivotSet>())
        {
            camManager.SetCamerasTarget(pivotSet.Position, pivotSet.gameObject);
        }
        podOnCustom = baseFrame.GetComponent<PodOnCustom>();
    }

    private void NewPod()
    {
        Debug.Log("New!");
        SavePod();
        CurrentPodCustomized = -999;
        ReGeneratePod();
    }
    private void SwitchPod()
    {
        Debug.Log("Switch!");
        Destroy(podOnCustom.gameObject);
        ReGeneratePod();
    }
    private void ReGeneratePod()
    {
        Destroy(podOnCustom.gameObject);
        podOnCustom = null;
        GenerateCustomPod();
        ReGeneratePodPivotsPanel();
    }

    // PodPivots Panel
    GameObject[] partPodButtons;
    PivotSet[] pivotSets;
    private void GeneratePodPivotsPanel()
    {
        PodOnCustom pod = FindObjectOfType<PodOnCustom>();
        pivotSets = pod.GetComponentsInChildren<PivotSet>();
        partPodButtons = new GameObject[pivotSets.Length/* + 1*/];
        for (int i = 0; i < partPodButtons.Length; i++)
            partPodButtons[i] = Instantiate(prefabPanel, podPartContainer.transform, false);
        for (int i = 0; i < partPodButtons.Length; i++)
        {
            GameObject partPodButton = partPodButtons[i];
            PivotSet ps = /*i == 0 ? null :*/ pivotSets[i];
            string text = "";

            // Properties
            Button btn = partPodButton.GetComponent<Button>();
            if (ps != null)
                text = ps.PartsAccepted.ToString();
                //ps.PartsAccepted.ToList().ForEach(x => text += x.ToString() + " ");
            partPodButton.GetComponentInChildren<Text>().text = ps != null ? text : "Baseframe";
            btn.onClick.AddListener(() => Select(ps, btn));

            // OnSelect
            PodPivotButton podPivotButton = btn.gameObject.AddComponent<PodPivotButton>();
            podPivotButton.camPosition = ps != null ? ps.Position : PositionType.Orbital;

            // Navigation
            Navigation navigation = btn.navigation;
            navigation.mode = Navigation.Mode.Explicit;
            navigation.selectOnDown = partPodButtons[(i + 1) % partPodButtons.Length].GetComponent<Button>();
            int n = (i - 1) % partPodButtons.Length;
            navigation.selectOnUp = partPodButtons[(n < 0) ? partPodButtons.Length + n : n].GetComponent<Button>();
            btn.navigation = navigation;
        }
    }
    private void ReGeneratePodPivotsPanel()
    {
        partPodButtons.ToList().ForEach(x => Destroy(x));
        partPodButtons.ToList().Clear();
        pivotSets.ToList().Clear();
        GeneratePodPivotsPanel();
    }

    // Parts Panel
    public void GeneratePartInventoryList()
    {
        Debug.Log("Generates Parts");
        List<Part> parts = new List<Part>();
        if (selectedPodPivot != null)
        {
            //foreach (TypePart typePart in selectedPodPivot.PartsAccepted)
            //{
            //    parts.AddRange(SavedDatasManager.GetPartsByTypePart(typePart));
            //}
            parts.AddRange(SavedDatasManager.GetPartsByTypePart(selectedPodPivot.PartsAccepted));
        }
        List<RectTransform> panels = inventoryContainer.GetComponentsInChildren<RectTransform>().ToList();
        panels.RemoveAll(x => x.gameObject == inventoryContainer.gameObject);
        panels.ForEach(x => Destroy(x.gameObject));
        
        GameObject[] partButtons = new GameObject[parts.Count];
        for (int i = 0; i < partButtons.Length; i++)
                partButtons[i] = Instantiate(prefabPanel, inventoryContainer.transform, false);
        for (int i = 0; i < parts.Count; i++)
        {
            Part p = parts[i];
            GameObject partButton = partButtons[i];
            partButton.GetComponentInChildren<Text>().text = p.Name + " - " + p.ID;
            Button btn = partButton.GetComponent<Button>();
            btn.onClick.AddListener(() => PutElement(p));
            switch (p.TypePart)
            {
                case TypePart.BackWings: partButton.GetComponent<Image>().color = Color.blue; break;
                case TypePart.Reactor: partButton.GetComponent<Image>().color = Color.red; break;
                case TypePart.FrontWings: partButton.GetComponent<Image>().color = Color.green; break;
                default: partButton.GetComponent<Image>().color = Color.magenta; break;
            }
            //partsPodPanels.Add(partButton);

            // Navigation
            Navigation navigation = btn.navigation;
            navigation.mode = Navigation.Mode.Explicit;
            navigation.selectOnRight = partButtons[(i + 1) % partButtons.Length].GetComponent<Button>();
            int n = (i - 1) % partButtons.Length;
            navigation.selectOnLeft = partButtons[(n < 0) ? partButtons.Length + n : n].GetComponent<Button>();
            btn.navigation = navigation;
            if (i == 0) eventSystem.SetSelectedGameObject(btn.gameObject);
        }
    }

    private void PutElement(Part p)
    {
        selectedPodPivot.PutElement(p);
        saved = false;
        notSavedAndPushB = false;
    }

    // Stat Panel 
    private void GenerateStatsPanel()
    {
        foreach (StatType statType in (StatType[])Enum.GetValues(typeof(StatType)))
        {
            GameObject bar = Instantiate(statBarPrefab, statPanel.transform);
            bar.GetComponentInChildren<Slider>().value = 0;
            bar.GetComponentsInChildren<Text>()[0].text = statType.ToString();
            statsBars.Add(statType, bar);
        }
        UpdateStatsPanel();
    }
    public void UpdateStatsPanel()
    {
        if (podOnCustom != null)
        {
            List<Stat> stats = podOnCustom.ToPodModelStats();
            foreach (Stat stat in stats)
            {
                statsBars[stat.StatType].GetComponentInChildren<Slider>().value = stat.Value / 999;
                statsBars[stat.StatType].GetComponentsInChildren<Text>()[1].text = stat.Value.ToString();
            }
        }
    }

    private void Select(PivotSet _ps, Button _btn)
    {
        if (_ps == null)
            Debug.Log("Base Frame");
        selectedPodPivot = _ps == null ? null : _ps.PodPivots.First() ?? selectedPodPivot;
        lastSelected = _btn.gameObject;
        GeneratePartInventoryList();
    }

    public void SavePod()
    {
        PodModel podModel = podOnCustom.ToPodModel();
        if (podModel == null || podModel == new PodModel())
        {
            Debug.Log("UnSavable");
            return;
        }
        podModel.Name = podName.text;
        SavedDatasManager.SavePodModels(ref podModel);
        podOnCustom.ID = podModel.ID;
        podOnCustom.name = podModel.Name;
        saved = true;
        notSavedAndPushB = false;
    }

    public void Return()
    {
        if (!saved && !notSavedAndPushB)
        {
            NotSaved.SetActive(true);
            StartCoroutine("NotSavedFading");
            notSavedAndPushB = true;
        }
        else if (notSavedAndPushB || saved)
            LoadingScript.LoadNewScene(Scenes.VehiculeSelection);
    }

    IEnumerator NotSavedFading()
    {
        float timeToDisapear = 1.5f;
        float timer = timeToDisapear;
        Text text = NotSaved.GetComponent<Text>();
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            text.color = new Color(text.color.r, text.color.g, text.color.b, timer / timeToDisapear);
            yield return null;
        }
        text.color = new Color(text.color.r, text.color.g, text.color.b, 1);
        text.gameObject.SetActive(false);
        yield return null;
    }

    public void GoToRace()
    {
        if (saved) LoadingScript.LoadNewScene(Scenes.LobbyMulti);
        else Debug.Log("not saved");
    }
}