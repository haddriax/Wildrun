using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class StatPanel : MonoBehaviour
{
    [SerializeField] Slider accelerationSlider;
    [SerializeField] Slider MaxSpeedSlider;
    [SerializeField] Slider TurningSlider;
    [SerializeField] Slider StrengthSlider;
    [SerializeField] Slider TurboPowerSlider;
    [SerializeField] Slider TurboDurabilitySlider;
    [SerializeField] Slider TurboCooldownSlider;

    List<Stat> stats;

    public bool onCustom = true;

    private void Update()
    {
        //if (onCustom)
    }

    public void UpdateStats(PodModel _model = null)
    {
        if (_model != null)
            stats = _model.Stats;
        else
        {
            stats = FindObjectOfType<PodOnCustom>().ToPodModelStats();
        }

        if (stats != null)
            foreach (Stat stat in stats)
            {
                switch (stat.StatType)
                {
                    case StatType.Acceleration:
                        accelerationSlider.value = stat.Value / 999;
                        break;
                    case StatType.MaxSpeed:
                        MaxSpeedSlider.value = stat.Value / 999;
                        break;
                    case StatType.Turning:
                        TurningSlider.value = stat.Value / 999;
                        break;
                    case StatType.Strength:
                        StrengthSlider.value = stat.Value / 999;
                        break;
                    case StatType.TurboPower:
                        TurboPowerSlider.value = stat.Value / 999;
                        break;
                    case StatType.TurboDuration:
                        TurboDurabilitySlider.value = stat.Value / 999;
                        break;
                    case StatType.TurboCooldown:
                        TurboCooldownSlider.value = stat.Value / 999;
                        break;
                    default:
                        break;
                }
            }
    }
}
