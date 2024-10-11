using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneNavigation
{
    private static void LobbyMulti()
    {
        SceneManager.LoadScene("Production/Scenes/Menu/Lobby");
    }

    private static void MainMenu()
    {
        SceneManager.LoadScene("Production/Scenes/Menu/MainMenu");
    }

    private static void VehiculeSelection()
    {
        SceneManager.LoadScene("Production/Scenes/Customization/MenuSelectionVehicules");
    }

    private static void Customization()
    {
        SceneManager.LoadScene("Production/Scenes/Customization/Garage");
    }

    private static void RaceMulti()
    {
        SceneManager.LoadScene("Production/Scenes/Races/RaceMulti");
    }

    private static void RaceSolo()
    {
        SceneManager.LoadScene("Production/Scenes/Races/RaceSolo");
    }

    private static void AIRace()
    {
        SceneManager.LoadScene("Production/Scenes/Races/AIRace");
    }

    private static void TestInput()
    {
        SceneManager.LoadScene("Development/TestRewired/SampleScene");
    }
}