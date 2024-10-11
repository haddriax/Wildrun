using Rewired;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ConnectionHandler : MonoBehaviour
{
    float timeBeforeStart = 5;
    Coroutine timer = null;
    //[SerializeField] Text timerText;
    RewiredManager rewiredManager = null;
    SoundManagerFMOD managerFMOD;
    [SerializeField] GameObject lobbySetUp;

    private void Awake()
    {
        managerFMOD = SoundManagerFMOD.GetInstance();
    }

    void Start()
    {
        //timerText.enabled = false;
        rewiredManager = RewiredManager.Instance;
        rewiredManager.RemoveAllPlayers();
    }

    private void OnEnable()
    {
        FindObjectsOfType<VehiculeSelecter>().ToList().ForEach(x => x.ReInit());
        if (rewiredManager != null)
            rewiredManager.RemoveAllPlayers();
        lobbySetUp.SetActive(true);
    }

    private void OnDisable()
    {
        lobbySetUp.SetActive(false);
    }

    void Update()
    {
        if (rewiredManager == null)
            rewiredManager = RewiredManager.Instance;
        foreach (Player p in rewiredManager.PotentialPlayers)
        {
            if (p.GetButtonDown("Connect"))
            {
                if (rewiredManager.AddPlayer(p))
                {
                    managerFMOD.PlayClickBackwardUI(transform);
                    //Connect();
                }
                else if (rewiredManager.PlayersConnected.Contains(p) && !FindObjectOfType<MapSelectionHandler>().GetCurrentMap().isLock)
                {
                    managerFMOD.PlayClickBackwardUI(transform);
                    LauchGame();
                }

            }

            if (p.GetButtonDown("Disconnect"))
            {
                List<VehiculeSelecter> selecters = FindObjectsOfType<VehiculeSelecter>().OrderBy(x => x.playerNumber).ToList();
                bool pPassed = false;
                for (int i = 0; i < selecters.Count; i++)
                {
                    if (selecters[i].player == p) pPassed = true;
                    if (pPassed && i + 1 < selecters.Count) selecters[i].currentVehicule = selecters[i + 1].currentVehicule;
                }
                managerFMOD.PlayClickErrorUI(transform);
                rewiredManager.RemovePlayer(p);
                //Disconnect();
            }
        }

        if (Input.GetButtonDown("ButtonB"))
        {
            if (FindObjectOfType<AltMenuCanvasHandler>())
                FindObjectOfType<AltMenuCanvasHandler>().CloseLobby();
            else
                ReturnMainMenu();
        }
    }

    private static void LauchGame()
    {
        SelectionVehiculeRace svr = FindObjectOfType<SelectionVehiculeRace>();
        List<PodModel> podModelsSelected = FindObjectsOfType<VehiculeSelecter>().Where(x => x.player != null).Select(x => svr.podmodels[x.currentVehicule]).ToList();
        RaceManager.PodsSelected = podModelsSelected;

        SoundManagerFMOD manager = SoundManagerFMOD.GetInstance();
        manager.StopSound();
        
        if (podModelsSelected.Count == 1)
            LoadingScript.LoadNewScene(Scenes.RaceSolo);
        else if (podModelsSelected.Count > 1)
            LoadingScript.LoadNewScene(Scenes.RaceMulti);
    }

    //private void Connect()
    //{
    //    //timerText.enabled = true;
    //    //timer = timer ?? StartCoroutine("Timer");
    //    //timeBeforeStart = 5;
    //}

    //private void Disconnect()
    //{
    //    //timeBeforeStart = 5;
    //    //if (rewiredManager.PlayersConnected.ToList().Count() == 0)
    //    //{
    //    //    StopCoroutine("Timer");
    //    //    timer = null;
    //    //    timerText.enabled = false;
    //    //}
    //}

    //IEnumerator Timer()
    //{
    //    while (timeBeforeStart > 0)
    //    {
    //        timeBeforeStart -= Time.deltaTime;
    //        timerText.text = Mathf.Ceil(timeBeforeStart) + " Seconds Left";
    //        yield return null;
    //    }
    //    timer = null;
    //    LoadingScript.LoadNewScene(Scenes.RaceMulti);
    //}

    public void ReturnMainMenu()
    {
        managerFMOD.PlayClickBackwardUI(transform);
        LoadingScript.LoadNewScene(Scenes.MainMenu);
    }
}
