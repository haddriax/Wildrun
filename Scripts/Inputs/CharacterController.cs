using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Vehicle;

public class CharacterController
{
    public MainController MainController = null;
    public CharacterLapManager characterLapManager = null;
    public void Do(EnumActions _action, float _intensity)
    {
        if (MainController != null)
        {
            switch (_action)
            {
                case EnumActions.Accelerate:
                    MainController.AccelerationValue = _intensity;
                    break;
                case EnumActions.Brake:
                    break;
                case EnumActions.Turn:
                    MainController.TurnValue = _intensity;
                    break;
                case EnumActions.Handbrake:
                    break;
                case EnumActions.Special:
                    break;
                case EnumActions.Honk:
                    break;
            }
        }
    }

    public void SetMainController(int _index)
    {
        Debug.Log("nb Vehicules : " + GameObject.FindObjectsOfType<VehiculeLayout>().ToList().Count);
        MainController[] tmp = GameObject.FindObjectsOfType<VehiculeLayout>().ToList().Where(x => x.Index == _index).Select(x => x.GetComponentInChildren<MainController>()).ToArray();
        MainController = tmp.Length > 0 ? tmp.First() : null;
        characterLapManager = MainController.GetComponent<CharacterLapManager>();
        if (MainController == null) Debug.Log("NULL TAMER");
    }
}
