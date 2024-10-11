using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PodCustomManager2 : MonoBehaviour
{
    [Serializable]
    public class PartButton
    {
        /// <summary>
        /// > -100 -> Backwings Empty 
        /// > -200 -> Reactors Empty 
        /// > -300 -> Frontwings Empty 
        /// </summary>
        public int IdPart;
        public Sprite ButtonImage = null;
    }

    SoundManagerFMOD managerFMOD;
    Customization.CameraManager camManager;

    [SerializeField] InputField podName;
    [SerializeField] GameObject NotSaved;
    [SerializeField] StatDisplayer statDisplayer = null;
    [SerializeField] PivotSelection pivotSelection;
    [SerializeField] public PartsSelection partsSelection;
    [SerializeField] GameObject podSpawnPos = null;
    public PodPainter podPainter = null;
    public List<PartButton> partButtons;

    PodPivot selectedPodPivot;
    public PodOnCustom podOnCustom;
    static public int CurrentPodCustomized = -999;
    static public int CurrentBaseCustomized = -999;

    public bool saved = true;
    private bool notSavedAndPushB = true;
    GameObject lastSelected;
    EventSystem eventSystem;

    private void Awake()
    {
        managerFMOD = SoundManagerFMOD.GetInstance();
    }

    private void Start()
    {
        //camManager = FindObjectOfType<Customization.CameraManager>();

        if (CurrentPodCustomized == -999)
            CurrentPodCustomized = SavedDatasManager.PodModels.First().ID;
        if (CurrentBaseCustomized == -999)
            CurrentBaseCustomized = SavedDatasManager.PodModels.First().IDBaseFrame;
        selectedPodPivot = null;
        GenerateCustomPod();
        eventSystem = FindObjectOfType<EventSystem>();
        //podPainter.GetMaterials();
    }

    private void Update()
    {
        GamePadCommand();
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
                    //camManager.SetCurrentCamera(PositionType.Orbital);
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
            PivotSelection.PivotButton selectedPiv = pivotSelection.Pivots.Last();
            eventSystem.SetSelectedGameObject(selectedPiv.partPodButton);
            selectedPodPivot = podOnCustom.GetComponentsInChildren<PodPivot>().ToList().Find(x => x.PartsAccepted == selectedPiv.typePart);
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
    public void GenerateCustomPod(bool _regenPod = true)
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
            if (podSpawnPos == null)
            {
                baseFrame.transform.SetParent(podSpawnPos.transform);
                baseFrame.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            }
                //baseFrame.transform.SetPositionAndRotation(Vector3.up, Quaternion.identity);
            else
                baseFrame.transform.SetPositionAndRotation(podSpawnPos.transform.position, podSpawnPos.transform.rotation);
        }
        else // When you create new Pod
        {
            if (podSpawnPos == null)
                baseFrame = Instantiate(SavedDatasManager.GetPartByID(CurrentBaseCustomized).gameObject, podSpawnPos.transform);
                //baseFrame = Instantiate(SavedDatasManager.GetPartByID(CurrentBaseCustomized).gameObject, Vector3.up, Quaternion.identity);
            else
                baseFrame = Instantiate(SavedDatasManager.GetPartByID(CurrentBaseCustomized).gameObject, podSpawnPos.transform.position, podSpawnPos.transform.rotation);
            baseFrame.AddComponent<PodOnCustom>();
            baseFrame.GetComponent<PodOnCustom>().ID = CurrentPodCustomized;
            baseFrame.GetComponent<PodOnCustom>().PodName = "NewPod";
            podName.text = "NewPod";
        }

        //camManager.SetCamerasTarget(PositionType.Orbital, baseFrame);
        foreach (PivotSet pivotSet in baseFrame.GetComponentsInChildren<PivotSet>())
        {
            //camManager.SetCamerasTarget(pivotSet.Position, pivotSet.gameObject);
        }
        podOnCustom = baseFrame.GetComponent<PodOnCustom>();
        if (statDisplayer != null)
            statDisplayer.podOnCustom = podOnCustom;
        /*if (_regenPod)*/ pivotSelection.GenerateButtonActions();
    }

    public void NewPod(bool _regenPod = true)
    {
        //Debug.Log("New!");
        SavePod();
        CurrentPodCustomized = -999;
        ReGeneratePod(_regenPod);
    }

    private void SwitchPod()
    {
        //Debug.Log("Switch!");
        Destroy(podOnCustom.gameObject);
        ReGeneratePod();
    }

    private void ReGeneratePod(bool _regenPod = true)
    {
        Destroy(podOnCustom.gameObject);
        podOnCustom = null;
        GenerateCustomPod(_regenPod);
    }

    private void SwitchBody(Part p)
    {
        Part forward = podOnCustom.GetComponentsInChildren<Part>().ToList().Find(x => x.TypePart == TypePart.FrontWings);
        Part reactor = podOnCustom.GetComponentsInChildren<Part>().ToList().Find(x => x.TypePart == TypePart.Reactor);
        Part backing = podOnCustom.GetComponentsInChildren<Part>().ToList().Find(x => x.TypePart == TypePart.BackWings);

        CurrentBaseCustomized = p.ID;
        int tmpPodId = CurrentPodCustomized;
        string tmpPodName = podName.text;
        NewPod(false);
        CurrentPodCustomized = tmpPodId;
        podOnCustom.ID = tmpPodId;
        podOnCustom.PodName = tmpPodName;
        podName.text = tmpPodName;

        List<PodPivot> list = podOnCustom.GetComponentsInChildren<PodPivot>().ToList()/*.FindAll(x => x.Position == PositionType.Right)*/;
        //Debug.Log(list.Find(x => x.PartsAccepted == TypePart.FrontWings));
        if (forward != null)
            list.Find(x => x.PartsAccepted == TypePart.FrontWings).PutElement(forward);
        if (reactor != null)
            list.Find(x => x.PartsAccepted == TypePart.Reactor).PutElement(reactor);
        if (backing != null)
            list.Find(x => x.PartsAccepted == TypePart.BackWings).PutElement(backing);
    }

    public void PutElement(Part p)
    {
        if (p.TypePart != TypePart.BaseFrame)
        {
            //if (p.TypePart == TypePart.Engine)
                //Debug.Log(p.TypePart);
            selectedPodPivot.PutElement(p);
            pivotSelection.SetButtonSprites();
        }
        else
        {
            // 1 fois sur 2 bugué :-/
            SwitchBody(p);
            SwitchBody(p);
        }
        saved = false;
        notSavedAndPushB = false;
    }

    bool selectBody = false;

    public void Select(PivotSet _ps, Button _btn)
    {
        //selectBody = false;
        selectedPodPivot = _ps.PodPivots.First() ?? selectedPodPivot;
        lastSelected = _btn.gameObject;
        partsSelection.ShowParts(_ps.PartsAccepted);
        eventSystem.SetSelectedGameObject(partsSelection.partButtonTypes.Find(x => x.button.activeSelf).button);
    }

    public void Select(TypePart typePart, Button _btn)
    {
        //selectBody = true;
        lastSelected = _btn.gameObject;
        partsSelection.ShowParts(typePart);
        eventSystem.SetSelectedGameObject(partsSelection.partButtonTypes.Find(x => x.button.activeSelf).button);
    }

    public void SavePod()
    {
        PodModel podModel = podOnCustom.ToPodModel(podSpawnPos);
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
        {
            StopAllCoroutines();
            Text text = NotSaved.GetComponent<Text>();
            text.color = new Color(text.color.r, text.color.g, text.color.b, 1);
            text.gameObject.SetActive(false);

            if (FindObjectOfType<AltMenuCanvasHandler>() != null)
                FindObjectOfType<AltMenuCanvasHandler>().CloseCustomization();
            else
                LoadingScript.LoadNewScene(Scenes.MainMenu);
        }
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
    }

    public void BugMillstone()
    {
        SavedDatasManager.bugMillestone = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
