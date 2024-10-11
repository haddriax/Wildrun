using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LeaderBoard : MonoBehaviour
{
    [SerializeField] public Canvas canvas;
    [SerializeField] Text[] playerNameTexts;
    [SerializeField] Text[] playerTimeTexts;
    [SerializeField] Button lobby;
    [SerializeField] Button main;
    [SerializeField] Button returnBtn;


    RewardManager m_rewardManager;
    private bool rewarding;
    private bool isDisplayed = false;

    public void Start()
    {
        m_rewardManager = FindObjectOfType<RewardManager>();
        //DisplayLeaderBoard(FindObjectsOfType<CharacterLapManager>().ToList());
    }

    private void Update()
    {
        if (isDisplayed)
        {
            if (Input.GetButtonDown("ButtonA"))
            {
                if (FindObjectOfType<RaceManager>().RaceType == RaceType.Multi)
                {
                    LoadingScript.LoadNewScene(Scenes.MainMenu);
                }
                else if (canvas.gameObject.activeSelf && rewarding)
                {
                    m_rewardManager.RewardPlayer();
                }
                else
                {
                    LoadingScript.LoadNewScene(Scenes.MainMenu);
                }
            }
        }
    }

    public void DisplayLeaderBoard(List<CharacterLapManager> _finishedCharacters)
    {
        isDisplayed = true;
        canvas.gameObject.SetActive(true);
        for (int i = 0; i < _finishedCharacters.Count; i++)
        {
            playerNameTexts[i].text = _finishedCharacters[i].Name;
            string scd = ((int)_finishedCharacters[i].TotalTime % 60).ToString();
            string min = ((int)_finishedCharacters[i].TotalTime / 60).ToString();
            playerTimeTexts[i].text = ((min.Length == 1) ? "0" : "") + min + "." + ((scd.Length == 1) ? "0" : "") + scd + "\nTime";
        }
        if (FindObjectOfType<RaceManager>().RaceType == RaceType.Solo && _finishedCharacters.First().GetComponentInChildren<UserControllerRewired>() && SavedDatasManager.LockedParts.Count > 0)
        {
            rewarding = true;
            main.onClick.AddListener(() => m_rewardManager.RewardPlayer());

            //test
            //m_rewardManager.RewardPlayer();
        }
        else
        {
            rewarding = false;
            main.onClick.AddListener(() => LoadingScript.LoadNewScene(Scenes.MainMenu));
        }
    }
}
