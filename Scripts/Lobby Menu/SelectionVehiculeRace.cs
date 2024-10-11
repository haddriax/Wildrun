using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Cinemachine;
using Rewired;

public class SelectionVehiculeRace : MonoBehaviour
{
    [SerializeField] GameObject pivotCam;

    public List<PodModel> podmodels = null;
    public List<GameObject> pods = new List<GameObject>();
    public List<GameObject> pivotsCam = new List<GameObject>();
    //public Dictionary<Player, Camera> playersCams = new Dictionary<Player, Camera>();

    private void Start()
    {
        podmodels = new List<PodModel>();
        foreach (PodModel p in SavedDatasManager.PodModels)
        {
            GeneratePod(p);
            podmodels.Add(p);
        }
        foreach (GameObject pod in SavedDatasManager.DefaultPods)
        {
            GeneratePod(pod);
        }
    }

    public void OnEnable()
    {
        //pods.ForEach(x => Destroy(x));
        //pods = 
        //podmodels = new List<PodModel>();
        //foreach (PodModel p in SavedDatasManager.PodModels)
        //{
        //    GeneratePod(p);
        //    podmodels.Add(p);
        //}
        //foreach (GameObject pod in SavedDatasManager.DefaultPods)
        //{
        //    GeneratePod(pod);
        //}
    }

    Vector3 pos = Vector3.zero;
    private void GeneratePod(PodModel _p)
    {
        PodModel podM = _p;
        GameObject baseFrame = Instantiate(SavedDatasManager.GetPartByID(podM.IDBaseFrame).gameObject);
        baseFrame.layer = 9;
        pivotsCam.Add(Instantiate(pivotCam, pos + pivotCam.transform.position, pivotCam.transform.rotation));

        VehiculeGenerator.GeneratePodGameObject(_p, baseFrame, 9);
        baseFrame.transform.SetPositionAndRotation(Vector3.up + pos, Quaternion.Euler(0, 180, 0));
        pos += Vector3.right * 15;
        pods.Add(baseFrame);
    }
    private void GeneratePod(GameObject _pod)
    {
        GameObject pod = Instantiate(_pod, Vector3.up + pos, Quaternion.Euler(0, 180, 0));
        pivotsCam.Add(Instantiate(pivotCam, pos + pivotCam.transform.position, pivotCam.transform.rotation));
        pods.Add(pod);
        pos += Vector3.right * 15;
    }
}
