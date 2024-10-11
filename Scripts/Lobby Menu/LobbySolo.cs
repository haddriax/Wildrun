using UnityEngine;
using System.Collections;
using Rewired;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

public class LobbySolo : MonoBehaviour
{
    [SerializeField] VehiculeSelecter vehiculeSelecter;
    Player player = null;
    SoundManagerFMOD manager = SoundManagerFMOD.GetInstance();

    void Update()
    {
        if (player == null)
        {
            player = vehiculeSelecter.player;
        }

        if (player != null)
        {
            if (player.GetButtonDown("Connect"))
            {
                manager.PlayClickForwardUI(transform);
                SelectionVehiculeRace svr = FindObjectOfType<SelectionVehiculeRace>();
                List<PodModel> podModelsSelected = FindObjectsOfType<VehiculeSelecter>().Where(x => x.player != null).Select(x => svr.podmodels[x.currentVehicule]).ToList();
                RaceManager.PodsSelected = podModelsSelected;

                manager.StopSound();
                LoadingScript.LoadNewScene(Scenes.RaceSolo);
            }
        }
    }
}
