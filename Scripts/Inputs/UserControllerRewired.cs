using Rewired;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserControllerRewired : MonoBehaviour
{
    public Player player = null;
    //public Vehicle.MainController mainController;
    public Vehicle.MainController mainController;
    public bool RaceRunning = false;

    private void Start()
    {
        //SetMainController(GetComponent<Vehicle.MainController>());
        SetMainController(GetComponent<Vehicle.MainController>());
    }

    private void Update()
    {
        if (RaceRunning && player != null && mainController != null)
            SendControllerDatas();
    }

    private void SendControllerDatas()
    {
        //Debug.Log(player.GetAxis("Acceleration"));
        //Debug.Log(player.GetAxis("Turn"));
        mainController.AccelerationValue = player.GetAxis("Acceleration");
        mainController.TurnValue = player.GetAxis("Turn");
        mainController.IsBoosting = player.GetButton("Boost");
        mainController.IsBraking = player.GetButton("Brake");
    }

    public void SetPlayer(Player _player)
    {
        player = _player;
    }

    public void SetMainController(Vehicle.MainController _mainController)
    {
        mainController = _mainController;
    }

    public void InitComponent(Player _p, Vehicle.MainController _mc)
    {
        SetPlayer(_p);
        SetMainController(_mc);
    }

    internal void Disactiv()
    {
        mainController.AccelerationValue = 0;
        mainController.TurnValue = 0;
        enabled = false;
    }
}
