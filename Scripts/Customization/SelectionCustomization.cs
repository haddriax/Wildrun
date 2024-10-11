using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SelectionCustomization : MonoBehaviour
{
    [SerializeField] GameObject container;
    [SerializeField] GameObject prefabPanel;
    [SerializeField] GameObject listBaseFrame;
    [SerializeField] GameObject listframeContainer;
    Dictionary<int, GameObject> panels = new Dictionary<int, GameObject>();
    GameObject lastSelected = null;

    SoundManagerFMOD managerFMOD;

    List<GameObject> podButtons = new List<GameObject>();

    private void Awake()
    {
        managerFMOD = SoundManagerFMOD.GetInstance();
    }

    void Start()
    {
        List<PodModel> podModels = SavedDatasManager.PodModels;
        for (int i = 0; i < podModels.Count; i++)
            podButtons.Add(Instantiate(prefabPanel, container.transform, false));
        for (int i = 0; i < podButtons.Count; i++)
        {
            GameObject podButton = podButtons[i];
            PodModel podModel = podModels[i];
            podButton.GetComponentInChildren<Text>().text = podModel.Name/* + " - " + podModel.ID*/;
            Button[] btns = podButton.GetComponentsInChildren<Button>();
            btns[0].onClick.AddListener(() => OpenGarage(podModel.ID));

            // Navigation
            Navigation nav1 = btns[0].navigation;
            nav1.mode = Navigation.Mode.Explicit;
            if (i < podButtons.Count - 1)
                nav1.selectOnDown = podButtons[(i + 1) % podButtons.Count].GetComponent<Button>();
            int n = (i - 1) % podButtons.Count;
            nav1.selectOnUp = podButtons[(n < 0) ? podButtons.Count + n : n].GetComponent<Button>();
                
            // Deletable
            if (i == 0) FindObjectOfType<EventSystem>().firstSelectedGameObject = btns[0].gameObject;
            if (podModel.Deletable)
            {
                Navigation nav2 = btns[1].navigation;
                nav2.mode = Navigation.Mode.Explicit;
                nav1.selectOnRight = btns[1];
                nav2.selectOnLeft = btns[0];
                nav2.selectOnDown = podButtons[(i + 1) % podButtons.Count].GetComponent<Button>();
                n = (i - 1) % podButtons.Count;
                nav2.selectOnUp = podButtons[(n < 0) ? podButtons.Count + n : n].GetComponent<Button>();

                btns[1].onClick.AddListener(() => DeletePod(podModel.ID));
                btns[1].navigation = nav2;
            }
            else Destroy(btns[1]);

            panels.Add(podModel.ID, podButton);
            btns[0].navigation = nav1;
        }
        GameObject newPodButton = Instantiate(prefabPanel, container.transform, false);
        newPodButton.GetComponentInChildren<Text>().text = "New Pod";
        Navigation nav = newPodButton.GetComponent<Button>().navigation;
        nav.mode = Navigation.Mode.Explicit;
        if(podButtons.Count > 0)
        {
            nav.selectOnUp = podButtons.Last().GetComponent<Button>();
            nav.selectOnDown = podButtons.First().GetComponent<Button>();

            Navigation nav2 = podButtons.Last().GetComponent<Button>().navigation;
            nav2.mode = Navigation.Mode.Explicit;
            nav2.selectOnDown = newPodButton.GetComponent<Button>();
            podButtons.Last().GetComponent<Button>().navigation = nav2;

            nav2 = podButtons.First().GetComponent<Button>().navigation;
            nav2.mode = Navigation.Mode.Explicit;
            nav2.selectOnUp = newPodButton.GetComponent<Button>();
            podButtons.First().GetComponent<Button>().navigation = nav2;
        }
        else
            FindObjectOfType<EventSystem>().firstSelectedGameObject = newPodButton;
        newPodButton.GetComponent<Button>().navigation = nav;
        Button[] buttons = newPodButton.GetComponentsInChildren<Button>();
        buttons[0].onClick.AddListener(() => ShowBaseFrames());
        Destroy(buttons[1].gameObject);
        GenerateBaseFrameButtons();
    }

    private void Update()
    {
        if (Input.GetButtonDown("ButtonB"))
        {
            if (listBaseFrame.gameObject.activeSelf)
            {
                listBaseFrame.gameObject.SetActive(false);
                FindObjectOfType<EventSystem>().SetSelectedGameObject(FrameBtns.First());
                managerFMOD.PlayClickBackwardUI(transform);
            }
            else
            {
                managerFMOD.PlayClickBackwardUI(transform);
                LoadingScript.LoadNewScene(Scenes.MainMenu);
            }
        }
    }

    List<GameObject> FrameBtns = new List<GameObject>();
    private void GenerateBaseFrameButtons()
    {
        List<Part> frames = SavedDatasManager.GetPartsByTypePart(TypePart.BaseFrame);
        foreach (Part p in frames)
        {
            GameObject newFrameBtn = Instantiate(prefabPanel, listframeContainer.transform, false);
            FrameBtns.Add(newFrameBtn);
            newFrameBtn.GetComponentInChildren<Text>().text = p.Name;
            Button[] btns = newFrameBtn.GetComponentsInChildren<Button>();
            btns[0].onClick.AddListener(() => SetBase(p.ID));
            Destroy(btns[1].gameObject);
        }
    }

    private void SetBase(int _id)
    {
        PodCustomManager.CurrentBaseCustomized = _id;
        OpenGarage(-999);

        managerFMOD.PlayClickForwardUI(transform);
    }

    private void OpenGarage(int id)
    {
        PodCustomManager.CurrentPodCustomized = id;

        managerFMOD.PlayClickForwardUI(transform);
        LoadingScript.LoadNewScene(Scenes.Customization);
    }

    private void DeletePod(int id)
    {
        SavedDatasManager.DeletePodModel(id);
        Destroy(panels[id]);
        panels.Remove(id);

        managerFMOD.PlayClickForwardUI(transform);
    }

    public void Return()
    {
        managerFMOD.PlayClickBackwardUI(transform);
        LoadingScript.LoadNewScene(Scenes.MainMenu);
    }

    private void ShowBaseFrames()
    {
        lastSelected = FindObjectOfType<EventSystem>().currentSelectedGameObject;
        //FrameBtns applique l'EventSystem sur les premiers boutons, mais pas sur les BaseFrames 
        FindObjectOfType<EventSystem>().SetSelectedGameObject(FrameBtns.First());
        //Debug.Log("Bug avec la manette: SlectionCustomization 164");
        listBaseFrame.SetActive(true);
    }

    public void CloseBaseFrameWindow()
    {
        FindObjectOfType<EventSystem>().SetSelectedGameObject(lastSelected);
        lastSelected = null;
        listBaseFrame.gameObject.SetActive(false);
    }

    private void OnGUI()
    {
        //GUI.color = Color.red;
        //GUI.Label(new Rect(0, 0, 1000, 1000), DatasSavingManager.tmpJson);
    }
}
