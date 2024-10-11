using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Vehicle
{
    [AddComponentMenu("_Scripts/Vehicle/Behaviours/UI")]
    public class VehicleUI : MonoBehaviour
    {
        public Image boostGauge;
        public Image shieldGauge;
        public Image speedGauge;

        public float Speed { set => speedGauge.fillAmount = Mathf.Clamp01(value); }
        public float Shield { set => shieldGauge.fillAmount = Mathf.Clamp01(value); }
        public float Boost { set => speedGauge.fillAmount = Mathf.Clamp01(value); }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            boostGauge.fillAmount = GetComponent<Engine>()._Boost.CurrentCapacity01;
            shieldGauge.fillAmount = GetComponent<Shield>().CurrentShield01;
            speedGauge.fillAmount = GetComponent<Engine>().SpeedPercentage01;
        }
    }

}
