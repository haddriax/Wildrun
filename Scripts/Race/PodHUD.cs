using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PodHUD : MonoBehaviour
{
    [SerializeField] Text rankTexts;
    [SerializeField] Text totalRankTexts;
    [SerializeField] Text timeText;
    [SerializeField] Text lapText;
    [SerializeField] GameObject GoodwayPanel;
    [SerializeField] Text speed;
    [SerializeField] Slider boost;
    [SerializeField] Slider shield;

    bool totalRankSeted = false;

    public CharacterLapManager lapManager;

    public GameObject miniMapMap;
    public GameObject miniMapMask;
    public int maxSpeed = 1000;

    RaceManager raceManager;

    private void Start()
    {
        //lapManager = transform.parent.parent.GetComponentInChildren<CharacterLapManager>();
        raceManager = FindObjectOfType<RaceManager>();
        try
        {
            totalRankTexts.text = "/" + FindObjectsOfType<CharacterLapManager>().Length;
            totalRankSeted = true;
        }
        catch
        {
            totalRankTexts.text = "/6";
        }
    }

    private void LateUpdate()
    {
        if (!totalRankSeted)
        {
            totalRankTexts.text = "/" + FindObjectsOfType<CharacterLapManager>().Length;
        }

        try
        {
            rankTexts.text = lapManager.Rank.ToString() /*+ "/" + FindObjectsOfType<CharacterLapManager>().Length + "\nRank"*/;
            string scd = ((int)lapManager.TotalTime % 60).ToString();
            string min = ((int)lapManager.TotalTime / 60).ToString();
            timeText.text = ((min.Length == 1) ? "0" : "") + min + ":" + ((scd.Length == 1) ? "0" : "") + scd + "\nTime";
            lapText.text = lapManager.CurrentLap + "/" + raceManager.LapNumber + "\nLap";
            GoodwayPanel.SetActive(!lapManager.isGoodWay);
        }
        catch { }

        try
        {
            boost.value = lapManager.GetComponentInChildren<Vehicle.Engine>()._Boost.CurrentCapacity01;
            shield.value = lapManager.GetComponentInChildren<Vehicle.Shield>().CurrentShield01;
            //speed.text = ((int)(maxSpeed * lapManager.GetComponentInChildren<Vehicle.Engine>().SpeedPercentage01)).ToString();
            speed.text = ((int)(lapManager.GetComponentInParent<VehiculeLayout>().PodStats.Find(x => x.StatType == StatType.MaxSpeed).Value * lapManager.GetComponentInChildren<Vehicle.Engine>().SpeedPercentage01)).ToString();
        }
        catch
        {
            Debug.Log("dla merde");
        }
    }
}
