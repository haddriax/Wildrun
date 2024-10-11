using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniMapPodsManager : MonoBehaviour
{
    private Terrain[] terrain;
    private float sizeMap;
    private Vector2 ratio;
    [SerializeField]
    public GameObject miniMapImg;
    private Vector2 sizeMiniMapImg;
    private GameObject[] vehicles;
    private Vector2Int sizeBlits;
    [SerializeField]
    public GameObject parentBlits;
    private GameObject blitGO;
    private List<GameObject> otherBlitsGO;
    [SerializeField]
    private bool useRotation = true;
    public bool UseRotation { get => useRotation; }

    string[] tags = { "AIVehicle", "Player" };

    private void Start()
    {
        terrain = GameObject.FindGameObjectWithTag("Terrain").GetComponentsInChildren<Terrain>();
        for (int i = 0; i < 4; i++)
        {
            sizeMap += terrain[i].terrainData.size.x;
        }
        //sizeMap = 8192;
        sizeMiniMapImg = miniMapImg.GetComponent<RectTransform>().sizeDelta;
        ratio = new Vector2(sizeMiniMapImg.x / sizeMap, sizeMiniMapImg.y / sizeMap);
        sizeBlits = new Vector2Int(15, 15);
        otherBlitsGO = new List<GameObject>();

        LoadVehicles();
        AddOtherBlits();
        AddPersonalBlits();  
        ManageMiniMap();
    }

    private void Update()
    {
        // A SUPP APRES
       // ManageMiniMap();
        /////////////////

        MoveMiniMap();
        MoveOthersBlits();
    }

    private void LoadVehicles()
    {
        // Create temporal list of gameobject
        List<GameObject> tempList = new List<GameObject>();

        // Travel all tags
        foreach (string tag in tags)
        {
            GameObject[] go = GameObject.FindGameObjectsWithTag(tag);

            // Add find gameobject to the temporal list
            for(byte i = 0; i < go.Length; i++)
            {
                tempList.Add(go[i]);
            }
        }

        // Define vehicles array
        vehicles = new GameObject[tempList.Count];

        // Copy the list to vehicles array
        tempList.CopyTo(vehicles);
    }

    private void AddPersonalBlits()
    {
        blitGO = new GameObject();
        blitGO.name = "PlayerBlit " + this.transform.parent.name;
        blitGO.transform.parent = parentBlits.transform;
        blitGO.transform.position = Vector3.zero;
        blitGO.AddComponent<RectTransform>();
        blitGO.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
        blitGO.GetComponent<RectTransform>().sizeDelta = new Vector2(sizeBlits.x, sizeBlits.y);
        blitGO.GetComponent<RectTransform>().rotation = Quaternion.identity;
        blitGO.GetComponent<RectTransform>().localScale = new Vector2(1.0f, 1.0f);
        blitGO.AddComponent<CanvasRenderer>();
        blitGO.AddComponent<Image>();
        blitGO.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/PlayerBlit");
    }

    private void AddOtherBlits()
    {
        for (byte i = 0; i < vehicles.Length; i++)
        {
            if (this.transform.parent.gameObject != vehicles[i])
            {
                GameObject go = new GameObject();
                go.name = "PlayerBlit " + vehicles[i].gameObject.name;
                go.transform.parent = parentBlits.transform;
                go.transform.position = Vector3.zero;
                go.AddComponent<RectTransform>();
                go.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
                go.GetComponent<RectTransform>().sizeDelta = new Vector2(sizeBlits.x, sizeBlits.y);
                go.GetComponent<RectTransform>().rotation = Quaternion.identity;
                go.GetComponent<RectTransform>().localScale = new Vector2(1.0f, 1.0f);
                go.AddComponent<CanvasRenderer>();
                go.AddComponent<Image>();
                go.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/OthersBlit");

                otherBlitsGO.Add(go);
            }
        }
    }

    private void ManageMiniMap()
    {
        if (this.transform.parent.GetComponentInChildren<Camera>().enabled)
        {
            foreach (Transform tr in this.transform.parent)
            {
                if (tr.tag == "MiniMap")
                {
                    tr.gameObject.SetActive(true);
                }
            }
        }
        else
        {
            foreach (Transform tr in this.transform.parent)
            {
                if (tr.tag == "MiniMap")
                {
                    tr.gameObject.SetActive(false);
                }
            }
        }
    }

    private void MoveMiniMap()
    {
        Vector2 tempPos = new Vector2(-this.transform.position.x * ratio.x, -this.transform.position.z * ratio.y);

        if (useRotation)
        {
            tempPos = Quaternion.Euler(0, 0, this.transform.eulerAngles.y) * tempPos;
            blitGO.GetComponent<RectTransform>().eulerAngles = Vector3.zero;
            miniMapImg.GetComponent<RectTransform>().eulerAngles = new Vector3(0.0f, 0.0f, this.transform.eulerAngles.y);
        }
        else
        {
            blitGO.GetComponent<RectTransform>().eulerAngles = new Vector3(0.0f, 0.0f, -this.transform.eulerAngles.y);
            miniMapImg.GetComponent<RectTransform>().eulerAngles = Vector3.zero;
        }

        miniMapImg.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(tempPos.x, tempPos.y, 0.0f);
    }

    private void MoveOthersBlits()
    {
        for (byte i = 0; i < vehicles.Length; i++)
        {
            if (this.transform.parent.name != vehicles[i].name)
            {
                foreach (GameObject go in otherBlitsGO)
                {
                    if (go.name == "PlayerBlit " + vehicles[i].gameObject.name)
                    {
                        for(byte j = 0; j < vehicles[i].transform.childCount; j++)
                        {
                            if(vehicles[i].transform.GetChild(j).name.Contains("Layout"))
                            {
                                if (useRotation)
                                {
                                    Vector3 localPos = transform.InverseTransformPoint(vehicles[i].transform.GetChild(j).position);
                                    Vector2 tempPos = new Vector2(localPos.x * ratio.x, localPos.z * ratio.y);
                                    Vector3 tempRot = new Vector3(0.0f, 0.0f, -vehicles[i].transform.GetChild(j).eulerAngles.y + this.transform.eulerAngles.y);
                                    go.GetComponent<RectTransform>().eulerAngles = tempRot;
                                    go.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(tempPos.x, tempPos.y, 0.0f);
                                }
                                else
                                {
                                    Vector2 tempPos = new Vector2((vehicles[i].transform.GetChild(j).position.x - this.transform.position.x) * ratio.x,
                                        ((vehicles[i].transform.GetChild(1).position.z - this.transform.position.z)) * ratio.y);
                                    Vector3 tempRot = new Vector3(0.0f, 0.0f, -vehicles[i].transform.GetChild(j).eulerAngles.y);
                                    go.GetComponent<RectTransform>().eulerAngles = tempRot;
                                    go.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(tempPos.x, tempPos.y, 0.0f);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}