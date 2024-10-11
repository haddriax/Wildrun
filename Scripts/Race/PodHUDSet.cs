using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PodHUDSet : MonoBehaviour
{
    public PodHUD podHudsPrefabs;
    private List<PodHUD> podHUDs;

    private void Start()
    {
        podHUDs = new List<PodHUD>();
    }

    public void InitHuds(List<CharacterLapManager> _clm)
    {
        int nbCLM = _clm.Count;
        podHUDs = new List<PodHUD>();
        for (int i = 0; i < nbCLM; i++)
        {
            podHUDs.Add(Instantiate(podHudsPrefabs, transform));
            podHUDs[i].gameObject.SetActive(true);
        }

        if (nbCLM == 1)
        {
            //podHUDs[0].lapManager = FindObjectOfType<RaceManager>().characterLapManagers[0];
            podHUDs[0].lapManager = _clm[0];
            _clm[0].GetComponentInChildren<MiniMapPodsManager>().parentBlits = podHUDs[0].miniMapMask;
            _clm[0].GetComponentInChildren<MiniMapPodsManager>().miniMapImg = podHUDs[0].miniMapMap;
            podHUDs[0].GetComponent<RectTransform>().localScale = Vector2.one;
            podHUDs[0].GetComponent<RectTransform>().localPosition = Vector2.zero;
        }
        else if (nbCLM == 2)
        {
            //podHUDs[0].lapManager = FindObjectOfType<RaceManager>().characterLapManagers[0];
            podHUDs[0].lapManager = _clm[0];
            _clm[0].GetComponentInChildren<MiniMapPodsManager>().parentBlits = podHUDs[0].miniMapMask;
            _clm[0].GetComponentInChildren<MiniMapPodsManager>().miniMapImg = podHUDs[0].miniMapMap;
            podHUDs[0].GetComponent<RectTransform>().localScale = new Vector2(0.5f, 0.5f);
            podHUDs[0].GetComponent<RectTransform>().sizeDelta = new Vector2(1920 * 2, 1080);
            podHUDs[0].GetComponent<RectTransform>().localPosition = new Vector2(0, 270);

            //podHUDs[1].lapManager = FindObjectOfType<RaceManager>().characterLapManagers[1];
            podHUDs[1].lapManager = _clm[1];
            _clm[1].GetComponentInChildren<MiniMapPodsManager>().parentBlits = podHUDs[1].miniMapMask;
            _clm[1].GetComponentInChildren<MiniMapPodsManager>().miniMapImg = podHUDs[1].miniMapMap;
            podHUDs[1].GetComponent<RectTransform>().localScale = new Vector2(0.5f, 0.5f);
            podHUDs[1].GetComponent<RectTransform>().sizeDelta = new Vector2(1920 * 2, 1080);
            podHUDs[1].GetComponent<RectTransform>().localPosition = new Vector2(0, -270);
        }
        else if (nbCLM == 3)
        {
            //podHUDs[0].lapManager = FindObjectOfType<RaceManager>().characterLapManagers[0];
            podHUDs[0].lapManager = _clm[0];
            _clm[0].GetComponentInChildren<MiniMapPodsManager>().parentBlits = podHUDs[0].miniMapMask;
            _clm[0].GetComponentInChildren<MiniMapPodsManager>().miniMapImg = podHUDs[0].miniMapMap;
            podHUDs[0].GetComponent<RectTransform>().localScale = new Vector2(0.5f, 0.5f);
            podHUDs[0].GetComponent<RectTransform>().sizeDelta = new Vector2(1920 *2 , 1080 );
            podHUDs[0].GetComponent<RectTransform>().localPosition = new Vector2(0, 270);

            //podHUDs[1].lapManager = FindObjectOfType<RaceManager>().characterLapManagers[1];
            podHUDs[1].lapManager = _clm[1];
            _clm[1].GetComponentInChildren<MiniMapPodsManager>().parentBlits = podHUDs[1].miniMapMask;
            _clm[1].GetComponentInChildren<MiniMapPodsManager>().miniMapImg = podHUDs[1].miniMapMap;
            podHUDs[1].GetComponent<RectTransform>().localScale = new Vector2(0.5f, 0.5f);
            podHUDs[1].GetComponent<RectTransform>().localPosition = new Vector2(-480, -270);

            //podHUDs[2].lapManager = FindObjectOfType<RaceManager>().characterLapManagers[2];
            podHUDs[2].lapManager = _clm[2];
            _clm[2].GetComponentInChildren<MiniMapPodsManager>().parentBlits = podHUDs[2].miniMapMask;
            _clm[2].GetComponentInChildren<MiniMapPodsManager>().miniMapImg = podHUDs[2].miniMapMap;
            podHUDs[2].GetComponent<RectTransform>().localScale = new Vector2(0.5f, 0.5f);
            podHUDs[2].GetComponent<RectTransform>().localPosition = new Vector2(480, -270);
        }
        else if (nbCLM == 4)
        {
            //podHUDs[0].lapManager = FindObjectOfType<RaceManager>().characterLapManagers[0];
            podHUDs[0].lapManager = _clm[0];
            _clm[0].GetComponentInChildren<MiniMapPodsManager>().parentBlits = podHUDs[0].miniMapMask;
            _clm[0].GetComponentInChildren<MiniMapPodsManager>().miniMapImg = podHUDs[0].miniMapMap;
            podHUDs[0].GetComponent<RectTransform>().localScale = new Vector2(0.5f, 0.5f);
            podHUDs[0].GetComponent<RectTransform>().localPosition = new Vector2(-480, 270);

            //podHUDs[1].lapManager = FindObjectOfType<RaceManager>().characterLapManagers[1];
            podHUDs[1].lapManager = _clm[1];
            _clm[1].GetComponentInChildren<MiniMapPodsManager>().parentBlits = podHUDs[1].miniMapMask;
            _clm[1].GetComponentInChildren<MiniMapPodsManager>().miniMapImg = podHUDs[1].miniMapMap;
            podHUDs[1].GetComponent<RectTransform>().localScale = new Vector2(0.5f, 0.5f);
            podHUDs[1].GetComponent<RectTransform>().localPosition = new Vector2(480, 270);

            //podHUDs[2].lapManager = FindObjectOfType<RaceManager>().characterLapManagers[2];
            podHUDs[2].lapManager = _clm[2];
            _clm[2].GetComponentInChildren<MiniMapPodsManager>().parentBlits = podHUDs[2].miniMapMask;
            _clm[2].GetComponentInChildren<MiniMapPodsManager>().miniMapImg = podHUDs[2].miniMapMap;
            podHUDs[2].GetComponent<RectTransform>().localScale = new Vector2(0.5f, 0.5f);
            podHUDs[2].GetComponent<RectTransform>().localPosition = new Vector2(-480, -270);
            
            //podHUDs[3].lapManager = FindObjectOfType<RaceManager>().characterLapManagers[3];
            podHUDs[3].lapManager = _clm[3];
            _clm[3].GetComponentInChildren<MiniMapPodsManager>().parentBlits = podHUDs[3].miniMapMask;
            _clm[3].GetComponentInChildren<MiniMapPodsManager>().miniMapImg = podHUDs[3].miniMapMap;
            podHUDs[3].GetComponent<RectTransform>().localScale = new Vector2(0.5f, 0.5f);
            podHUDs[3].GetComponent<RectTransform>().localPosition = new Vector2(480, -270);
        }
    }
}
