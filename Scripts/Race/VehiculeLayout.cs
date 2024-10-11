using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System.Linq;
using System;

public class VehiculeLayout : MonoBehaviour
{


    [SerializeField] GameObject modelParent;
    [SerializeField] public GameObject vehiculeParent;
    [SerializeField] GameObject cameraLookAt;
    public List<Stat> PodStats;
    public int Index;
    public List<GameObject> reactors;
    public List<GameObject> backWings;
    public List<GameObject> frontWings;
    public GameObject body;
    public GameObject normalModel;
    public GameObject fracturedModel;
    public List<Fx> fxs = new List<Fx>();

    private void Start()
    {
        reactors = new List<GameObject>();
        backWings = new List<GameObject>();
        frontWings = new List<GameObject>();
        InitPartStorage();
        GetFracturedVehicle();
        GetFxs();
        GetComponentInChildren<Vehicle.Animation>().Init();
    }

    private void GetFxs()
    {
        fxs = GetComponentsInChildren<Fx>().ToList();
    }

    private void InitPartStorage()
    {
        GameObject rightReact = null;
        GameObject leftReact = null;
        GameObject rightFront = null;
        GameObject leftFront = null;
        GameObject rightBack = null;
        GameObject leftBack = null;
        List<Part> parts;
        foreach (TypePart typePart in (TypePart[])Enum.GetValues(typeof(TypePart)))
        {
            parts = modelParent.GetComponentsInChildren<Part>().ToList().FindAll(x => x.TypePart == typePart);
            foreach (Part p in parts)
            {
                for (int i = 0; i < p.transform.childCount; i++)
                {
                    Transform transform1 = p.transform.GetChild(i);
                    bool rigth = transform1.parent.localScale.x == -1;
                    if (transform1.name == "Visual")
                    {
                        for (int j = 0; j < transform1.childCount; j++)
                        {
                            GameObject gameObject1 = transform1.transform.GetChild(j).gameObject;
                            if (gameObject1.activeSelf)
                            {
                                switch (typePart)
                                {
                                    case TypePart.BaseFrame:
                                        body = gameObject1; break;
                                    case TypePart.Reactor:
                                        if (rigth)
                                            rightReact = gameObject1;
                                        else
                                            leftReact = gameObject1;
                                        //reactors.Add(gameObject1); 
                                        break;
                                    case TypePart.FrontWings:
                                        if (rigth)
                                            rightFront = gameObject1;
                                        else
                                            leftFront = gameObject1;
                                        //frontWings.Add(gameObject1);
                                        break;
                                    case TypePart.BackWings:
                                        if (rigth)
                                            rightBack = gameObject1;
                                        else
                                            leftBack = gameObject1;
                                        //backWings.Add(gameObject1);
                                        break;
                                    default: break;
                                }
                            }
                        }
                    }
                }
            }
        }
        if (rightReact) reactors.Add(rightReact);
        else Debug.Log("R Reactor null");
        if (leftReact) reactors.Add(leftReact);
        else Debug.Log("L Reactor null");

        if (rightFront) frontWings.Add(rightFront);
        else Debug.Log("R Front null");
        if (leftFront) frontWings.Add(leftFront);
        else Debug.Log("L Front null");

        if (rightBack) backWings.Add(rightBack);
        else Debug.Log("R Back null");
        if (leftBack) backWings.Add(leftBack);
        else Debug.Log("L Back null");

        //Debug.Log(reactors.Count);
        //Debug.Log(frontWings.Count);
        //Debug.Log(backWings.Count);
    }

    private void GetFracturedVehicle()
    {
        for (int i = 0; i < modelParent.transform.childCount; i++)
        {
            if (modelParent.transform.GetChild(i).name.Contains("ok"))
            {
                for (int j = 0; j < modelParent.transform.GetChild(i).childCount; j++)
                {
                    if (modelParent.transform.GetChild(i).GetChild(j).name.Contains("Fractured"))
                    {
                        fracturedModel = modelParent.transform.GetChild(i).GetChild(j).gameObject;
                        fracturedModel.SetActive(false);
                    }
                    else
                    {
                        normalModel = modelParent.transform.GetChild(i).GetChild(j).gameObject;
                    }
                }
            }
        }
    }

    public void AddModel(GameObject _model)
    {
        _model.transform.parent = modelParent.transform;
        _model.transform.localPosition = Vector3.zero;
        _model.transform.localRotation = Quaternion.Euler(Vector3.zero);
    }

    public void AddCamera(GameObject _cam)
    {
        _cam.transform.parent = transform;
        _cam.transform.localPosition = Vector3.zero;
    }

    public void AddCamera(GameObject _cam, Vector3 _pos)
    {
        CinemachineVirtualCamera camera = _cam.GetComponentInChildren<CinemachineVirtualCamera>();
        _cam.transform.parent = transform;
        _cam.transform.localPosition = _pos;
        camera.LookAt = cameraLookAt.transform;
        camera.Follow = vehiculeParent.transform;
    }

    public void AddCharacterLapManager()
    {
        vehiculeParent.AddComponent<CharacterLapManager>();
    }

    public T AddScriptToVehicule<T>() where T : MonoBehaviour 
    {
        return vehiculeParent.AddComponent<T>();
    }
} 
