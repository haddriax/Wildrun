using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AISpawner : MonoBehaviour
{
    private GameObject car;
    private GameObject[] spawn;

    private void Awake()
    {
        // Load car gameObject from Resources file
        car = Resources.Load<GameObject>("AI/SandSkimmer_VF_AI");
    }

    private void Start()
    {
        spawn = GameObject.FindGameObjectsWithTag("Spawn");
        if (FindObjectOfType<RaceManager>() || FindObjectsOfType<StartingBlock>().Length > 0)
            spawn = FindObjectsOfType<StartingBlock>().ToList().FindAll(x => !x.AlreadyUsed).Select(x => x.gameObject).ToArray();


        // Init cars on the spawn points
        if (!FindObjectOfType<RaceManager>())
            InitAISpawn();
    }

    public void InitAISpawn(int _nbAI = -1)
    {
        if (FindObjectOfType<RaceManager>())
            spawn = GameObject.FindGameObjectsWithTag("Spawn").ToList().FindAll(x => !x.GetComponent<StartingBlock>().AlreadyUsed).ToArray();

        //Debug.Log(spawn.Length);
        for (byte i = 0; i < (spawn.Length); i++)
        {
            car.name = "AIPod (" + (i + 1).ToString() + ")";
            GameObject pod = Instantiate(car, spawn[i].transform.position, Quaternion.identity, this.transform);

            StartingBlock startingBlock = spawn[i].GetComponentInChildren<StartingBlock>();
            PodModel podModel = SavedDatasManager.PodModels[Random.Range(0, SavedDatasManager.PodModels.Count - 1)];
            //if (startingBlock)
            //    if (startingBlock.podId != -1 && SavedDatasManager.PodModels.Select(x => x.ID).Contains(startingBlock.podId))
            //    {
            //        podModel = SavedDatasManager.GetPodModelById(startingBlock.podId);
            //    }

            //pod.GetComponentInChildren<VehiculeLayout>().AddModel(VehiculeGenerator.GeneratePodGameObject(podModel, null, 9));
            pod.GetComponentInChildren<VehiculeLayout>().AddModel(VehiculeGenerator.GenerataPodDestroyable(podModel, null, 9));
            pod.GetComponentInChildren<Vehicle.MainController>().VehicleStatistics = new Vehicle.Statistics()
            {
                acceleration = Mathf.Clamp(podModel.Stats.Find(x => x.StatType == StatType.Acceleration).Value, 100, 1000),
                maniability = Mathf.Clamp(podModel.Stats.Find(x => x.StatType == StatType.Turning).Value, 100, 1000),
                shield = Mathf.Clamp(podModel.Stats.Find(x => x.StatType == StatType.Strength).Value, 100, 1000),
                speed = Mathf.Clamp(podModel.Stats.Find(x => x.StatType == StatType.MaxSpeed).Value, 100, 1000)
            };
            //pod.GetComponentInChildren<Vehicle.Animation>().Init();

            pod.transform.rotation = spawn[i].transform.rotation;

            if (FindObjectOfType<RaceManager>())
            {
                pod.GetComponentInChildren<AI.AIMovement>().followRoad = false;
                pod.GetComponentInChildren<Vehicle.MainController>().gameObject.AddComponent<CharacterLapManager>();
            }
        }
    }
}
