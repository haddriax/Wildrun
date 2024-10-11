using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuCanvasHandler : MonoBehaviour
{
    [Header("Menu Popups")]
    [SerializeField] GameObject m_singlePlayerPopup;
    [SerializeField] GameObject m_optionsPopup;
    [SerializeField] GameObject m_creditsPopup;



    [Header("Option Popups")]
    [SerializeField] GameObject m_gameOptionsPopup;
    [SerializeField] GameObject m_videoOptionsPopup;
    [SerializeField] GameObject m_displayOptionsPopup;
    [SerializeField] GameObject m_soundOptionsPopup;
    [SerializeField] GameObject m_controlsOptionsPopup;

    public enum m_optionPopups { Game, Video, Display, Sound, Controls };

    SoundManagerFMOD manager;
    int FMODInstance;

    private void Awake()
    {
        manager = SoundManagerFMOD.GetInstance();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (!manager.alreadyUsing)
        {
            FMODInstance = manager.InitInstance("event:/Soundtrack/SoundTrackMenu2D");
            manager.PlayInstance(FMODInstance, transform, GameObject.Find("SoundManager").GetComponent<Rigidbody>(), true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) LoadingScript.LoadNewScene(Scenes.TestInput);
    }


    #region Options

    public void OpenOptions()
    {
        m_creditsPopup.SetActive(false);
        m_optionsPopup.SetActive(true);

        manager.PlayClickForwardUI(transform);
    }

    public void CloseOptions()
    {
        m_optionsPopup.SetActive(false);

        manager.PlayClickBackwardUI(transform);
    }

    #endregion

    #region Credits

    public void OpenCredits()
    {
        m_optionsPopup.SetActive(false);
        m_creditsPopup.SetActive(true);

        manager.PlayClickForwardUI(transform);
    }

    public void CloseCredits()
    {
        m_creditsPopup.SetActive(false);

        manager.PlayClickBackwardUI(transform);
    }

    #endregion





    public void OpenGameOptionPopup()
    {
        m_gameOptionsPopup.SetActive(true);
        m_videoOptionsPopup.SetActive(false);
        m_displayOptionsPopup.SetActive(false);
        m_soundOptionsPopup.SetActive(false);
        m_controlsOptionsPopup.SetActive(false);

        manager.PlayClickForwardUI(transform);
    }

    public void OpenVideoOptionPopup()
    {
        m_gameOptionsPopup.SetActive(false);
        m_videoOptionsPopup.SetActive(true);
        m_displayOptionsPopup.SetActive(false);
        m_soundOptionsPopup.SetActive(false);
        m_controlsOptionsPopup.SetActive(false);

        manager.PlayClickForwardUI(transform);
    }

    public void OpenDisplayOptionPopup()
    {
        m_gameOptionsPopup.SetActive(false);
        m_videoOptionsPopup.SetActive(false);
        m_displayOptionsPopup.SetActive(true);
        m_soundOptionsPopup.SetActive(false);
        m_controlsOptionsPopup.SetActive(false);

        manager.PlayClickForwardUI(transform);
    }

    public void OpenSoundOptionPopup()
    {
        m_gameOptionsPopup.SetActive(false);
        m_videoOptionsPopup.SetActive(false);
        m_displayOptionsPopup.SetActive(false);
        m_soundOptionsPopup.SetActive(true);
        m_controlsOptionsPopup.SetActive(false);

        manager.PlayClickForwardUI(transform);
    }

    public void OpenControlsOptionPopup()
    {

        m_gameOptionsPopup.SetActive(false);
        m_videoOptionsPopup.SetActive(false);
        m_displayOptionsPopup.SetActive(false);
        m_soundOptionsPopup.SetActive(false);
        m_controlsOptionsPopup.SetActive(true);

        manager.PlayClickForwardUI(transform);
    }

    public void Multi()
    {
        manager.PlayClickForwardUI(transform);
        LoadingScript.LoadNewScene(Scenes.LobbyMulti);
    }

    public void Solo()
    {
        manager.PlayClickForwardUI(transform);
        manager.StopSound();
        LoadingScript.LoadNewScene(Scenes.RaceSolo);
    }

    public void Custom()
    {
        manager.PlayClickForwardUI(transform);
        LoadingScript.LoadNewScene(Scenes.VehiculeSelection);
    }

    public void Custom2()
    {
        manager.PlayClickForwardUI(transform);
        LoadingScript.LoadNewScene(Scenes.Customization2);
    }

    public void AI()
    {
        manager.PlayClickForwardUI(transform);
        LoadingScript.LoadNewScene(Scenes.AIRace);
    }

    public void TimeAttack()
    {
        manager.PlayClickForwardUI(transform);
        LoadingScript.LoadNewScene(Scenes.TimeAttack);
    }

    public void LobbySolo()
    {
        manager.PlayClickForwardUI(transform);
        LoadingScript.LoadNewScene(Scenes.LobbySolo);
    }



    #region Exit

    public void QuitApplication()
    {
        RewiredManager rewired = RewiredManager.Instance;
        
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            rewired.ExitGame();
            Application.Quit();
#endif
    }

    #endregion
}
