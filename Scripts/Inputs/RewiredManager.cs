using Rewired;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewiredManager
{
    private static RewiredManager instance = null;
    public static RewiredManager Instance
    {
        get
        {
            if (instance == null) instance = new RewiredManager();
            return instance;
        }
    }

    public Player[] PotentialPlayers { get; private set; }
    public List<Player> PlayersConnected { get; set; }

    private RewiredManager()
    {
        PotentialPlayers = new Player[4];
        PlayersConnected = new List<Player>();
        for (int i = 0; i < PotentialPlayers.Length; i++)
        {
            if (GameObject.FindObjectOfType<InputManager>() != null)
                PotentialPlayers[i] = ReInput.players.GetPlayer(i);
        }
    }

    public bool AddPlayer(Player _p)
    {
        if (!PlayersConnected.Contains(_p))
        {
            //Debug.Log("Add " + _p.id);
            PlayersConnected.Add(_p);
            //PlayersConnected.ForEach(x => Debug.Log(x.id));
            return true;
        }
        return false;
    }

    public bool GetButtonDown(string _action)
    {
        for (int i = 0; i < PotentialPlayers.Length; i++)
        {
            if (_action != "" && PotentialPlayers[i].GetButtonDown(_action)) return true;
        }
        return false;
    }

    public bool GetButton(string _action = "")
    {
        for (int i = 0; i < PotentialPlayers.Length; i++)
        {
            if (_action == "" && PotentialPlayers[i].GetAnyButton()) return true;
            if (_action != "" && PotentialPlayers[i].GetButton(_action)) return true;
        }
        return false;
    }

    public void RemovePlayer(Player _p)
    {
        PlayersConnected.RemoveAll(x => x == _p);
    }

    /// <summary>
    /// Retourne le Joueur qui appuie
    /// </summary>
    /// <param name="_action"></param>
    /// <returns></returns>
    public Player GetPlayerAnyButton(string _action = "")
    {
        for (int i = 0; i < PotentialPlayers.Length; i++)
        {
            if (_action == "" && PotentialPlayers[i].GetAnyButton()) return PotentialPlayers[i];
            if (_action != "" && PotentialPlayers[i].GetButton(_action)) return PotentialPlayers[i];
        }
        return null;
    }

    public bool GetAnyButton()
    {
        foreach(Player p in PotentialPlayers)
        {
            return p.GetAnyButton();
        }
        return false;
    }

    public void RemoveAllPlayers()
    {
        PlayersConnected.Clear();
    }

    public void ExitGame()
    {
        for (int i = 0; i < PlayersConnected.Count; i++)
        {
            PlayersConnected[i].controllers.ClearAllControllers();
        }
        for (int i = 0; i < PotentialPlayers.Length; i++)
        {
            PotentialPlayers[i].controllers.ClearAllControllers();
        }
        PlayersConnected.Clear();
    }

    ///// <summary>
    ///// ChangeControlToMenu is use to change the Control of all controllers 
    ///// between the Game to the Menu. 
    ///// </summary>
    //public void ChangeControlToMenu()
    //{
    //    for (int i = 0; i < potentialPlayers.Length; i++)
    //    {
    //        potentialPlayers[i].controllers.maps.LoadMap(ControllerType.Joystick, i, "Default", "Default", false);
    //        //potentialPlayers[i].controllers.maps.LoadMap(ControllerType.Joystick, i, "Menu", "Default", true);
    //    }

    //    //This for will be use to when the players will be inside the List playersConnected
    //    if (playersConnected.Count != 0)
    //    {
    //        for (int i = 0; i < playersConnected.Count; i++)
    //        {
    //            playersConnected[i].controllers.maps.LoadMap(ControllerType.Joystick, i, "Default", "Default", false);
    //            //playersConnected[i].controllers.maps.LoadMap(ControllerType.Joystick, i, "Menu", "Default", true);
    //        }
    //    }
    //}
    ///// <summary>
    ///// ChangeControlToGame is use to change the Control of all controllers 
    ///// between the Menu to the Game. 
    ///// </summary>
    //public void ChangeControlToGame()
    //{
    //    for (int i = 0; i < potentialPlayers.Length; i++)
    //    {
    //        //potentialPlayers[i].controllers.maps.LoadMap(ControllerType.Joystick, i, "Menu", "Default", false);
    //        potentialPlayers[i].controllers.maps.LoadMap(ControllerType.Joystick, i, "Default", "Default", true);
    //    }


    //    //This for will be use to when the players will be inside the List playersConnected
    //    if (playersConnected.Count != 0)
    //    {
    //        for (int i = 0; i < playersConnected.Count; i++)
    //        {
    //            playersConnected[i].controllers.maps.LoadMap(ControllerType.Joystick, i, "Menu", "Default", false);
    //            playersConnected[i].controllers.maps.LoadMap(ControllerType.Joystick, i, "Default", "Default", true);
    //        }
    //    }
    //}
}
