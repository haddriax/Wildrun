using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;

public class Menu : MonoBehaviour
{
    [SerializeField] GameObject canvas;
    [SerializeField] Button resume;
    //[SerializeField] Button lobby;
    [SerializeField] Button main;
    [SerializeField] GameObject objectifs = null;

    SoundManagerFMOD managerFMOD;

    private void Awake()
    {
        managerFMOD = SoundManagerFMOD.GetInstance();
    }

    private void Start()
    {
        resume.onClick.AddListener(() => Resume());
        main.onClick.AddListener(() => MainMenu());
        //lobby.onClick.AddListener(() => Lobby());
        RaceManager raceManager = FindObjectOfType<RaceManager>();
        if (raceManager)
            if (raceManager.RaceType == RaceType.Multi && objectifs) objectifs.SetActive(false);
        //if (raceManager != null)
        //    if (raceManager.RaceType == RaceType.Solo || raceManager.RaceType == RaceType.TimeAttack)
        //        lobby.gameObject.SetActive(false);
    }

    private void Update()
    {
        if(!canvas.activeSelf)
        {
            if (Input.GetButtonDown("ButtonStart") && !FindObjectOfType<LeaderBoard>().canvas.gameObject.activeSelf)
            {
                Pause();
            }
        }
        else
        {
            if (Input.GetButtonDown("ButtonStart") || Input.GetButtonDown("ButtonB"))
            {
                Resume();
            }
            else if (Input.GetButtonDown("ButtonY"))
            {
                MainMenu();
            }
        }
    }

    private void Resume()
    {
        Time.timeScale = 1;
        canvas.SetActive(false);
        managerFMOD.PlaySound();
    }

    private void Pause()
    {
        Debug.Log("pause");
        Time.timeScale = 0;
        canvas.SetActive(true);
        FindObjectOfType<EventSystem>().SetSelectedGameObject(GetComponentsInChildren<Button>().First().gameObject);
        managerFMOD.PauseSound();
    }

    public void Lobby()
    {
        Time.timeScale = 1;
        managerFMOD.StopSound();
        LoadingScript.LoadNewScene(Scenes.LobbyMulti);
    }

    public void MainMenu()
    {
        Time.timeScale = 1;
        managerFMOD.StopSound();
        LoadingScript.LoadNewScene(Scenes.MainMenu);
    }
}
