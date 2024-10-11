using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Linq;

public class StatDisplayer : MonoBehaviour
{
    [SerializeField] GameObject AccelerationSlider = null;
    [SerializeField] GameObject MaxSpeedSlider = null;
    [SerializeField] GameObject TurningSlider = null;
    [SerializeField] GameObject StrengthSlider = null;
    [SerializeField] GameObject TurboPowerSlider = null;
    [SerializeField] GameObject TurboDurabilitySlider = null;
    [SerializeField] GameObject TurboCooldownSlider = null;
    
    public PodOnCustom podOnCustom = null;

    private void Start()
    {
        DisactivateDisplay();
    }

    private void Update()
    {
        if (podOnCustom != null)
            UpdateStats();
    }

    public void UpdateStats()
    {
        ActivateDisplay();
        List<Stat> stats = podOnCustom.ToPodModelStats();

        if (stats != null)
        {
            foreach (Stat stat in stats)
            {
                switch (stat.StatType)
                {
                    case StatType.Acceleration:
                        if (AccelerationSlider != null)
                        {
                            AccelerationSlider.GetComponentInChildren<Slider>().value = stat.Value / 999;
                            AccelerationSlider.GetComponentInChildren<Text>().text = "Acceleration : " + stat.Value;
                        }
                        break;
                    case StatType.MaxSpeed:
                        if (MaxSpeedSlider != null)
                        {
                            MaxSpeedSlider.GetComponentInChildren<Slider>().value = stat.Value / 999;
                            MaxSpeedSlider.GetComponentInChildren<Text>().text = "Max Speed : " + stat.Value;
                        }
                        break;
                    case StatType.Turning:
                        if (TurningSlider != null)
                        {
                            TurningSlider.GetComponentInChildren<Slider>().value = stat.Value / 999;
                            TurningSlider.GetComponentInChildren<Text>().text = "Turning : " + stat.Value;
                        }
                        break;
                    case StatType.Strength:
                        if (StrengthSlider != null)
                        {
                            StrengthSlider.GetComponentInChildren<Slider>().value = stat.Value / 999;
                            StrengthSlider.GetComponentInChildren<Text>().text = "Strength : " + stat.Value;
                        }
                        break;
                    case StatType.TurboPower:
                        if (TurboPowerSlider != null)
                        {
                            TurboPowerSlider.GetComponentInChildren<Slider>().value = stat.Value / 999;
                            TurboPowerSlider.GetComponentInChildren<Text>().text = "Turbo Power : " + stat.Value;
                        }
                        break;
                    case StatType.TurboDuration:
                        if (TurboDurabilitySlider != null)
                        {
                            TurboDurabilitySlider.GetComponentInChildren<Slider>().value = stat.Value / 999;
                            TurboDurabilitySlider.GetComponentInChildren<Text>().text = "Turbo Duration : " + stat.Value;
                        }
                        break;
                    case StatType.TurboCooldown:
                        if (TurboCooldownSlider != null)
                        {
                            TurboCooldownSlider.GetComponentInChildren<Slider>().value = stat.Value / 999;
                            TurboCooldownSlider.GetComponentInChildren<Text>().text = "Turbo Cooldown : " + stat.Value;
                        }
                        break;
                    default:
                        break;
                }
            }
        }
    }

    public void Display(List<Stat> _stats)
    {
        //Debug.Log("Begin");
        DisactivateDisplay();
        foreach (Stat stat in _stats)
        {
            //Debug.Log(stat.StatType + " " + stat.Value);
            switch (stat.StatType)
            {
                case StatType.Acceleration:
                    if (AccelerationSlider != null && _stats.Find(x => x.StatType == stat.StatType) != null)
                    {
                        //Debug.Log(stat.StatType);
                        AccelerationSlider.SetActive(true);
                        AccelerationSlider.GetComponentInChildren<Slider>().value = stat.Value / 999;
                        AccelerationSlider.GetComponentInChildren<Text>().text = "Acceleration : " + stat.Value;
                    }
                    break;
                case StatType.MaxSpeed:
                    if (MaxSpeedSlider != null && _stats.Find(x => x.StatType == stat.StatType) != null)
                    {
                        //Debug.Log(stat.StatType);
                        MaxSpeedSlider.SetActive(true);
                        MaxSpeedSlider.GetComponentInChildren<Slider>().value = stat.Value / 999;
                        MaxSpeedSlider.GetComponentInChildren<Text>().text = "Max Speed : " + stat.Value;
                    }
                    break;
                case StatType.Turning:
                    if (TurningSlider != null && _stats.Find(x => x.StatType == stat.StatType) != null)
                    {
                        //Debug.Log(stat.StatType);
                        TurningSlider.SetActive(true);
                        TurningSlider.GetComponentInChildren<Slider>().value = stat.Value / 999;
                        TurningSlider.GetComponentInChildren<Text>().text = "Turning : " + stat.Value;
                    }
                    break;
                case StatType.Strength:
                    if (StrengthSlider != null && _stats.Find(x => x.StatType == stat.StatType) != null)
                    {
                        //Debug.Log(stat.StatType);
                        StrengthSlider.SetActive(true);
                        StrengthSlider.GetComponentInChildren<Slider>().value = stat.Value / 999;
                        StrengthSlider.GetComponentInChildren<Text>().text = "Strength : " + stat.Value;
                    }
                    break;
                case StatType.TurboPower:
                    if (TurboPowerSlider != null && _stats.Find(x => x.StatType == stat.StatType) != null)
                    {
                        //Debug.Log(stat.StatType);
                        TurboPowerSlider.SetActive(true);
                        TurboPowerSlider.GetComponentInChildren<Slider>().value = stat.Value / 999;
                        TurboPowerSlider.GetComponentInChildren<Text>().text = "Turbo Power : " + stat.Value;
                    }
                    break;
                case StatType.TurboDuration:
                    if (TurboDurabilitySlider != null && _stats.Find(x => x.StatType == stat.StatType) != null)
                    {
                        //Debug.Log(stat.StatType);
                        TurboDurabilitySlider.SetActive(true);
                        TurboDurabilitySlider.GetComponentInChildren<Slider>().value = stat.Value / 999;
                        TurboDurabilitySlider.GetComponentInChildren<Text>().text = "Turbo Duration : " + stat.Value;
                    }
                    break;
                case StatType.TurboCooldown:
                    if (TurboCooldownSlider != null && _stats.Find(x => x.StatType == stat.StatType) != null)
                    {
                        //Debug.Log(stat.StatType);
                        TurboCooldownSlider.SetActive(true);
                        TurboCooldownSlider.GetComponentInChildren<Slider>().value = stat.Value / 999;
                        TurboCooldownSlider.GetComponentInChildren<Text>().text = "Turbo Cooldown : " + stat.Value;
                    }
                    break;
                default: break;
            }
        }
    }

    public void DisactivateDisplay()
    {
        if (AccelerationSlider) AccelerationSlider.SetActive(false);
        if (MaxSpeedSlider) MaxSpeedSlider.SetActive(false);
        if (TurningSlider) TurningSlider.SetActive(false);
        if (StrengthSlider) StrengthSlider.SetActive(false);
        if (TurboPowerSlider) TurboPowerSlider.SetActive(false);
        if (TurboDurabilitySlider) TurboDurabilitySlider.SetActive(false);
        if (TurboCooldownSlider) TurboCooldownSlider.SetActive(false);
    }

    public void ActivateDisplay()
    {
        if (AccelerationSlider) AccelerationSlider.SetActive(true);
        if (MaxSpeedSlider) MaxSpeedSlider.SetActive(true);
        if (TurningSlider) TurningSlider.SetActive(true);
        if (StrengthSlider) StrengthSlider.SetActive(true);
        if (TurboPowerSlider) TurboPowerSlider.SetActive(true);
        if (TurboDurabilitySlider) TurboDurabilitySlider.SetActive(true);
        if (TurboCooldownSlider) TurboCooldownSlider.SetActive(true);
    }
}
