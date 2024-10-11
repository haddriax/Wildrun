using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Quentin
{
    public class SceneNavigation
    {
        public static void LobbyMulti()
        {
            SceneManager.LoadScene("Production/Scenes/Menu/Lobby");
        }

        public static void MainMenu()
        {
            SceneManager.LoadScene("Production/Scenes/Menu/MainMenu");
        }

        public static void VehiculeSelection()
        {
            SceneManager.LoadScene("Production/Scenes/Customization/MenuSelectionVehicules");
        }

        internal static void Customization()
        {
            SceneManager.LoadScene("Production/Scenes/Customization/Garage");
        }

        internal static void RaceMulti()
        {
            SceneManager.LoadScene("Production/Scenes/Races/RaceMulti");
        }

        internal static void RaceSolo()
        {
            SceneManager.LoadScene("Production/Scenes/Races/RaceSolo");
        }

        internal static void AIRace()
        {
            SceneManager.LoadScene("Production/Scenes/Races/AIRace");
        }

        internal static void TestInput()
        {
            SceneManager.LoadScene("Development/TestRewired/SampleScene");
        }
    }
}