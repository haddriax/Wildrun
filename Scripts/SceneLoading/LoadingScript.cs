using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum Scenes
{
    MainMenu,
    LobbyMulti,
    VehiculeSelection,
    Customization,
    Customization2,
    RaceMulti,
    RaceSolo,
    AIRace,
    TestInput,
    TimeAttack,
    RewardScene,
    LobbySolo
};

public class LoadingScript : MonoBehaviour
{
    public static string m_scene = "Loading Scene";
    
    Coroutine loading = null;
    int cpt = 0;
    private void Update()
    {
        cpt++;
        if (cpt == 2 && loading == null)
            loading = StartCoroutine(LoadingRoutine());
    }

    // Entry function, start the Loading of the _scene
    public static void LoadNewScene(Scenes _scene)
    {
        switch (_scene)
        {
            case Scenes.MainMenu:
                //m_scene = "Production/Scenes/Menu/MainMenu";
                m_scene = "Production/Scenes/Menu/MenuFinal";
                break;
            case Scenes.LobbyMulti:
                m_scene = "Production/Scenes/Menu/LobbyMulti";
                break;
            case Scenes.LobbySolo:
                m_scene = "Production/Scenes/Menu/LobbySolo";
                break;
            case Scenes.VehiculeSelection:
                m_scene = "Production/Scenes/Customization/MenuSelectionVehicules";
                break;
            case Scenes.Customization:
                m_scene = "Production/Scenes/Customization/Garage";
                break;
            case Scenes.Customization2:
                m_scene = "Production/Scenes/Customization/GarageV2";
                break;
            case Scenes.RaceMulti:
                //m_scene = "Production/Scenes/Races/RaceMulti";
                //m_scene = "Production/Scenes/Races/Race_milestone_Multi";
                m_scene = "Production/Scenes/Races/RacesCanyon/Multi_Race_Canyon";
                break;
            case Scenes.RaceSolo:
                //m_scene = "Production/Scenes/Races/RaceSolo";
                //m_scene = "Production/Scenes/Races/Race_milestone_Solo";
                m_scene = "Production/Scenes/Races/RacesCanyon/Solo_Race_Canyon";
                break;
            case Scenes.TimeAttack:
                //m_scene = "Production/Scenes/Races/TimeAttack";
                m_scene = "Production/Scenes/Races/Race_milestone_TimeAttack";
                break;
            case Scenes.AIRace:
                //m_scene = "Production/Scenes/Races/AIRace";
                m_scene = "Production/Scenes/Races/Race_milestone_AI";
                break;
            case Scenes.TestInput:
                m_scene = "Development/TestRewired/TestInput";
                break;
            case Scenes.RewardScene:
                m_scene = "Production/Scenes/Rewards/RewardScreen";
                break;
            default:
                m_scene = "Production/Scenes/Loading/Loading Scene";
                break;
        }
        SceneManager.LoadScene("Production/Scenes/Loading/Loading Scene");
    }
    
    IEnumerator LoadingRoutine()
    {
        //yield return new WaitForSeconds(5f);
        AsyncOperation asyncLoading = SceneManager.LoadSceneAsync(m_scene);

        while (!asyncLoading.isDone)
        {
            yield return null;
        }
    }

    ///// <summary>
    ///// MenuControl are use to imply the changement 
    ///// of the controls between the Game to Menus. 
    ///// Set before the Load. 
    ///// </summary>
    //private static void MenuControl()
    //{
    //    RewiredManager manager = RewiredManager.Instance;
    //    manager.ChangeControlToMenu();
    //}

    ///// <summary>
    ///// GameControl are use to imply the changement 
    ///// of the controls between the Menus and Game. 
    ///// Set before the Load. 
    ///// </summary>
    //private static void GameControl()
    //{
    //    RewiredManager manager = RewiredManager.Instance;
    //    manager.ChangeControlToGame();
    //}
}