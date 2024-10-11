using Cinemachine;
using Rewired;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VehiculeSelecter : MonoBehaviour
{
    [SerializeField] Camera cam;
    public int playerNumber;

    RewiredManager rewiredManager;
    GameObject choosenPod = null;
    public int currentVehicule = 0;
    List<GameObject> pods = null;
    List<GameObject> pivotsCam = null;
    public Player player { get; private set; }
    public SelectionVehiculeRace SelectionVehicule = null;

    private void Start()
    {
        player = null;
        rewiredManager = RewiredManager.Instance;
        //if (SelectionVehicule)
        //{
        //    pods = SelectionVehicule.pods;
        //    pivotsCam = SelectionVehicule.pivotsCam;
        //}
        if (playerNumber == -1)
        {
            StartCoroutine("DetectGamePad");
        }
    }

    public void ReInit()
    {
        player = null;
    }

    private void Update()
    {
        pods = pods ?? SelectionVehicule.pods;
        pivotsCam = pivotsCam ?? SelectionVehicule.pivotsCam;

        if ((playerNumber > -1 && rewiredManager.PlayersConnected.Count >= playerNumber) || (playerNumber == -1 && player != null))
        {
            if (playerNumber != -1)
                player = rewiredManager.PlayersConnected[playerNumber - 1];
            if (player.GetButtonDown("Left"))
            {
                currentVehicule--;
                if (currentVehicule < 0) currentVehicule = pods.Count - 1;
            }
            if (player.GetButtonDown("Right"))
            {
                currentVehicule++;
                if (currentVehicule >= pods.Count) currentVehicule = 0;
            }
        }
        cam.transform.SetPositionAndRotation(pivotsCam[currentVehicule].transform.position, pivotsCam[currentVehicule].transform.rotation);
    }

    public void SetPlayer(Player _p)
    {
        player = _p;
    }

    IEnumerator DetectGamePad()
    {
        while (rewiredManager.GetPlayerAnyButton() == null)
        {
            yield return null;
        }
        player = rewiredManager.GetPlayerAnyButton();
    }
}
