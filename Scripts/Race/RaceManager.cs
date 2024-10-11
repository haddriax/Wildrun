using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System;
using System.IO;
using Newtonsoft.Json;
using Cinemachine;
using UnityEngine.UI;
using Rewired;

public enum RaceType
{
    Free, Solo, Multi, TimeAttack
}
public class RaceManager : MonoBehaviour
{
    [SerializeField] GameObject playableVehiculePrefab;

    public RaceType RaceType = RaceType.Solo;
    public int LapNumber;

    private StartingBlock[] startingBlocks;
    
    public List<CharacterLapManager> characterLapManagers;
    private List<CharacterLapManager> finishedCharacters;

    private RaceCheckpoint[] raceCheckpoints;
    public RaceCheckpoint[] RaceCheckpoints { get => raceCheckpoints; }

    public static List<PodModel> PodsSelected = null;

    private List<GameObject> podsDefault;

    private AISpawner aISpawner = null;

    RewiredManager rewiredManager;
    SoundManagerFMOD managerFMOD;
    int instanceFmod;

    private void Awake()
    {
        managerFMOD = SoundManagerFMOD.GetInstance();
        instanceFmod = managerFMOD.InitInstance("event:/Soundtrack/SoundTrackRace2D");
    }

    private void Start()
    {
        managerFMOD.PlayInstance(instanceFmod, transform, GameObject.Find("StartingBlocks").GetComponent<Rigidbody>(), true);

        aISpawner = FindObjectOfType<AISpawner>();

        rewiredManager = RewiredManager.Instance;
        podsDefault = SavedDatasManager.DefaultPods;

        startingBlocks = FindObjectsOfType<StartingBlock>()/*.OrderBy(x => x.Index).ToArray()*/;
        raceCheckpoints = FindObjectsOfType<RaceCheckpoint>().OrderBy(x => x.Index).ToArray();
        raceCheckpoints.ToList().ForEach(x => x.SetNeighbors());

        characterLapManagers = new List<CharacterLapManager>();
        InitiateRace();
        if (FindObjectOfType<PodHUDSet>())
            FindObjectOfType<PodHUDSet>().InitHuds(characterLapManagers);
        if (aISpawner != null)
            aISpawner.InitAISpawn(FindObjectsOfType<StartingBlock>().ToList().FindAll(x => !x.AlreadyUsed).Count);
        else
            Debug.Log("nul");

        //characterLapManagers = FindObjectsOfType<CharacterLapManager>().ToList();
        //Debug.Log(characterLapManagers.Count);
        characterLapManagers = FindObjectsOfType<CharacterLapManager>().ToList();
        finishedCharacters = new List<CharacterLapManager>();

        //test
        //finishedCharacters.Add(characterLapManagers[0]);
        //FindObjectOfType<LeaderBoard>().DisplayLeaderBoard(finishedCharacters);

        
    }

    private void LateUpdate()
    {
        //if (RaceType == RaceType.Multi)
        UpdateRank();
    }

    private void UpdateRank()
    {
        List<CharacterLapManager> tmpRanking = new List<CharacterLapManager>();
        List<CharacterLapManager>[] tmpLapRank = new List<CharacterLapManager>[LapNumber];
        for (int i = 0; i < tmpLapRank.Length; i++)
        {
            tmpLapRank[i] = characterLapManagers.FindAll(x => x.CurrentLap == i);
            if (tmpLapRank[i].Count > 0)
            {
                List<CharacterLapManager>[] tmpCheckpointRank = new List<CharacterLapManager>[raceCheckpoints.Length];
                for (int j = 0; j < tmpCheckpointRank.Length; j++)
                {
                    tmpCheckpointRank[j] = tmpLapRank[i].Where(x => x.currentCheckpoint != null).ToList().FindAll(x => x.currentCheckpoint.Index == j);
                    if (tmpCheckpointRank[j].Count > 0)
                        tmpCheckpointRank[j] = tmpCheckpointRank[j].OrderBy(x => Vector3.Distance(x.gameObject.transform.position, x.currentCheckpoint.Next.gameObject.transform.position)).ToList();
                }
                tmpLapRank[i].Clear();
                for (int j = tmpCheckpointRank.Length - 1; j >= 0; j--)
                {
                    tmpLapRank[i].AddRange(tmpCheckpointRank[j]);
                }
            }
        }
        for (int i = tmpLapRank.Length - 1; i >= 0; i--)
        {
            tmpRanking.AddRange(tmpLapRank[i]);
        }
        for (int i = 0; i < tmpRanking.Count; i++)
        {
            tmpRanking[i].SetRank(i + 1);
        }

        tmpRanking = tmpRanking.OrderByDescending(x => x.Rank).ToList();
    }

    public void CharacterFinish(CharacterLapManager _character)
    {
        if (!finishedCharacters.Contains(_character))
        {
            finishedCharacters.Add(_character);
            //switch (finishedCharacters.Count)
            //{
            //    case 1: ActivateFumi(); break;
            //    case 2: ActivateFumi(); break;
            //    case 3: ActivateFumi(); break;
            //    default: break;
            //}
            Ended();
        }
    }

    private void ActivateFumi()
    {
        //throw new NotImplementedException();
    }

    public void Ended()
    {
        int allControllerRewireds = FindObjectsOfType<UserControllerRewired>().ToList().Count;
        int finishedControllerRewireds = finishedCharacters.FindAll(x => x.GetComponentInChildren<UserControllerRewired>()).Count;
        if (finishedCharacters.Count == characterLapManagers.Count || allControllerRewireds == finishedControllerRewireds)
        {
            Debug.Log("Ended");
            FindObjectsOfType<UserControllerRewired>().ToList().ForEach(x => x.Disactiv());
            managerFMOD.StopSound();
            LeaderBoard leaderBoard = FindObjectOfType<LeaderBoard>();
            if (leaderBoard != null) leaderBoard.DisplayLeaderBoard(finishedCharacters);
            SavedDatasManager.SaveRaceDatas(ToRaceData());
            try
            {
                PodsSelected.Clear();
            }
            catch
            {

            }
        }
    }

    private RaceData ToRaceData()
    {
        RaceData raceData = new RaceData()
        {
            //Models = characterLapManagers.Select(x => x.GetComponent<>())
            TotalTimes = characterLapManagers.Select(x => x.TotalTime).ToArray(),
            RaceType = RaceType
        };

        List<TimeAtCheckpoint[]> timeAtCheckpoints = new List<TimeAtCheckpoint[]>();
        List<TimePerLap[]> timePerLaps = new List<TimePerLap[]>();
        for (int i = 0; i < finishedCharacters.Count; i++)
        {
            timeAtCheckpoints.Add(finishedCharacters[i].TimesPerCheckpoint.ToArray());
            timePerLaps.Add(finishedCharacters[i].TimesPerLap.ToArray());
        }

        raceData.timesPerCheckpoint = timeAtCheckpoints;
        raceData.timesPerLap = timePerLaps;

        return raceData;
    }

    private void InitiateRace()
    {
        if (RaceType == RaceType.Solo || RaceType == RaceType.TimeAttack)
        {
            List<PodModel> podModels = SavedDatasManager.PodModels;
            try
            {
                if (RaceType == RaceType.Solo && PodsSelected.Count > 0)
                {
                    GeneratePods(PodsSelected[0], startingBlocks[0].transform);
                    startingBlocks[0].AlreadyUsed = true;
                }
            }
            catch
            {
                if (RaceType == RaceType.Solo)
                {
                    if (podModels.Count > 0)
                        GeneratePods(podModels[1], startingBlocks[0].transform);
                    else
                        GeneratePods(podsDefault[0], startingBlocks[0].transform);
                    startingBlocks[0].AlreadyUsed = true;
                }
            }

            // Ghost Generation
            if (RaceType == RaceType.TimeAttack && SavedDatasManager.Ghosts.Count > 0)
            {
                Ghost best = SavedDatasManager.Ghosts.Find(x => x.totalTime == SavedDatasManager.Ghosts.Min(y => y.totalTime));
                GenerateGhost(podsDefault[1], best);
            }
            
            //if (PodsSelected.Count == 0)
            //    GeneratePods(podsDefault[0], startingBlocks[0].transform);
            //else
            //    GeneratePods(PodsSelected.First(), startingBlocks[0].transform);
            StartCoroutine(DetectGamePad());
        }
        else if (RaceType == RaceType.Multi)
        {
            PodsSelected.Reverse();
            for (int i = 0; i < rewiredManager.PlayersConnected.Count; i++)
            {
                if (i >= PodsSelected.Count)
                    GeneratePods(podsDefault[i], startingBlocks[i].transform, rewiredManager.PlayersConnected[i]);
                else
                    GeneratePods(PodsSelected[i], startingBlocks[i].transform, rewiredManager.PlayersConnected[i]);
                startingBlocks[i].AlreadyUsed = true;
            }
        }
        SetCameraSpliting();
    }

    private void GeneratePods(PodModel _model, Transform _origin, Player _player = null)
    {
        GameObject vehicleModel = VehiculeGenerator.GenerataPodDestroyable(_model, null, 9);
        GameObject podTemplate = InitPodTemplate(_origin);
        podTemplate.tag = "Player";
        InitVehicleLayout(_origin, podTemplate, vehicleModel, _model);
        if (_player != null)
            SetController(_player, podTemplate);
        //podTemplate.GetComponentInChildren<Vehicle.Animation>().Init();
    }

    private void GeneratePods(GameObject _pod, Transform _origin, Player _player = null)
    {
        GameObject podTemplate = InitPodTemplate(_origin);
        podTemplate.tag = "Player";
        GameObject vehicleModel = GameObject.Instantiate(_pod);
        vehicleModel.layer = 9;
        InitVehicleLayout(_origin, podTemplate, vehicleModel);
        if (_player != null)
            SetController(_player, podTemplate);
    }

    private void GenerateGhost(GameObject _pod, Ghost _ghost)
    {
        GhostController ghostController = GameObject.Instantiate(_pod).AddComponent<GhostController>();
        ghostController.SetGhost(_ghost);
    }

    private void GenerateGhost(PodModel _model, Ghost _ghost)
    {
        GameObject vehicleModel = GameObject.Instantiate(SavedDatasManager.GetPartByID(_model.IDBaseFrame).gameObject);
        VehiculeGenerator.GeneratePodGameObject(_model, vehicleModel, 9);
        GhostController ghostController = vehicleModel.AddComponent<GhostController>();
        ghostController.SetGhost(_ghost);
    }

    private GameObject InitPodTemplate(Transform _origin)
    {
        //GameObject podLayout = Instantiate(templatePod);
        GameObject podLayout = GameObject.Instantiate(playableVehiculePrefab);
        podLayout.GetComponentInChildren<VehiculeLayout>().AddCharacterLapManager();
        characterLapManagers.Add(podLayout.GetComponentInChildren<VehiculeLayout>().vehiculeParent.GetComponentInChildren<CharacterLapManager>());
        podLayout.GetComponentInChildren<VehiculeLayout>().Index = FindObjectsOfType<VehiculeLayout>().Length;
        podLayout.transform.position = _origin.position + Vector3.up * 3;
        return podLayout;
    }

    private void InitVehicleLayout(Transform _origin, GameObject podLayout, GameObject vehicleModel, PodModel podModel = null)
    {
        VehiculeLayout vehicule = podLayout.GetComponentInChildren<VehiculeLayout>();
        if (RaceType == RaceType.TimeAttack)
            vehicule.AddScriptToVehicule<GhostRecorder>();
        if (podModel != null)
        {
            vehicule.PodStats = podModel.Stats;
            vehicule.GetComponentInChildren<Vehicle.MainController>().VehicleStatistics = new Vehicle.Statistics()
            {
                acceleration = Mathf.Clamp(podModel.Stats.Find(x => x.StatType == StatType.Acceleration).Value, 100, 1000),
                maniability = Mathf.Clamp(podModel.Stats.Find(x => x.StatType == StatType.Turning).Value, 100, 1000),
                shield = Mathf.Clamp(podModel.Stats.Find(x => x.StatType == StatType.Strength).Value, 100, 1000),
                speed = Mathf.Clamp(podModel.Stats.Find(x => x.StatType == StatType.MaxSpeed).Value, 100, 1000)
            };
        }

        vehicule.AddModel(vehicleModel);
        podLayout.transform.rotation = _origin.rotation;

        //// AI for the end
        //vehicule.gameObject.AddComponent<AI.AIController>();
        //vehicule.GetComponent<AI.AIMovement>().followRoad = false;
    }

    private void SetController(Player _player, GameObject podLayout)
    {
        UserControllerRewired controllerRewired = podLayout.GetComponent<VehiculeLayout>().GetComponentInChildren<UserControllerRewired>();
        Vehicle.MainController mainController = podLayout.GetComponent<VehiculeLayout>().GetComponentInChildren<Vehicle.MainController>();
        if (controllerRewired != null)
            controllerRewired.InitComponent(_player, mainController);
    }

    IEnumerator DetectGamePad()
    {
        while (rewiredManager.GetPlayerAnyButton() == null)
        {
            yield return null;
        }

        UserControllerRewired controllerRewired = FindObjectsOfType<VehiculeLayout>().ToList().Find(x => x.GetComponentInChildren<UserControllerRewired>()).GetComponentInChildren<UserControllerRewired>();
        if (controllerRewired)
            controllerRewired.SetPlayer(rewiredManager.GetPlayerAnyButton());
    }

    private void SetCameraSpliting()
    {
        Camera[] cameras = FindObjectsOfType<CinemachineBrain>().Select(x => x.GetComponent<Camera>()).ToList().Where(x => x.transform.parent.parent.parent.GetComponentInChildren<UserControllerRewired>()).ToArray();
        //if (RaceType == RaceType.Multi)
        //    cameras = cameras.OrderByDescending(x => x.transform.parent.parent.GetComponentInChildren<UserControllerRewired>().player.id).ToArray();
        int nbCam = cameras.Length;

        for (int i = 0; i < nbCam; i++)
        {
            CinemachineVirtualCamera virtualCamera = cameras[i].transform.parent.GetComponentInChildren<CinemachineVirtualCamera>();
            virtualCamera.gameObject.layer = 10 + i;
            cameras[i].cullingMask = -1;
            for (int j = 0; j < nbCam; j++)
                if (j != i)
                {
                    cameras[i].cullingMask &= ~(1 << LayerMask.NameToLayer("P" + (j + 1) + "Cam"));
                }
        }

        switch (nbCam)
        {
            case 1:
                cameras[0].rect = new Rect(0, 0, 1, 1);
                break;
            case 2:
                cameras[0].rect = new Rect(0, 0, 1, 0.5f);
                cameras[1].rect = new Rect(0, 0.5f, 1, 0.5f);
                break;
            case 3:
                cameras[0].rect = new Rect(0.5f, 0, 0.5f, 0.5f);
                cameras[2].rect = new Rect(0, 0.5f, 1, 0.5f);
                cameras[1].rect = new Rect(0, 0, 0.5f, 0.5f);
                break;
            case 4:
                cameras[0].rect = new Rect(0.5f, 0, 0.5f, 0.5f);
                cameras[1].rect = new Rect(0, 0, 0.5f, 0.5f);
                cameras[2].rect = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
                cameras[3].rect = new Rect(0, 0.5f, 0.5f, 0.5f);
                break;
            default: break;
        }
    }

    public void ToLobby()
    {
        Time.timeScale = 1;
        managerFMOD.StopSound();
        LoadingScript.LoadNewScene(Scenes.LobbyMulti);
    }

    public void ToMainMenu()
    {
        Time.timeScale = 1;
        managerFMOD.StopSound();
        LoadingScript.LoadNewScene(Scenes.MainMenu);
    }
}