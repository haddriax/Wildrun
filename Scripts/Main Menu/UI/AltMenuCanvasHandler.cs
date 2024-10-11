using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AltMenuCanvasHandler : MonoBehaviour
{
    [Header("Camera Positions")]
    [SerializeField] GameObject m_mainPosition;
    [SerializeField] GameObject m_customizationPosition;
    [SerializeField] GameObject m_creditsPosition;
    [SerializeField] GameObject m_optionsPosition;
    [SerializeField] GameObject m_lobbyPosition;


    [Header("Overlays")]
    [SerializeField] GameObject m_mainOverlay;
    [SerializeField] GameObject m_customizationOverlay;
    [SerializeField] GameObject m_creditsOverlay;
    [SerializeField] GameObject m_optionsOverlay;
    [SerializeField] GameObject m_lobbyOverlay;

    private PodOnCustom podOnCustom = null;
    private GameObject firstBtn;

    SoundManagerFMOD manager;
    int FMODInstance;

    private void Awake()
    {
        manager = SoundManagerFMOD.GetInstance();
    }

    private void Start()
    {
        if (!manager.alreadyUsing)
        {
            FMODInstance = manager.InitInstance("event:/Soundtrack/SoundTrackMenu2D");
            manager.PlayInstance(FMODInstance, transform, GameObject.Find("SoundManager").GetComponent<Rigidbody>(), true);
        }

        firstBtn = FindObjectOfType<EventSystem>().firstSelectedGameObject;
    }

    private void Update()
    {
        if (Input.GetButtonDown("ButtonB") && !m_mainOverlay.activeSelf && !m_customizationOverlay.activeSelf)
        {
            OpenMain();
        }
    }

    IEnumerator DisplayOverlayAfterTime(float _delay, GameObject _overlay)
    {
        yield return new WaitForSeconds(_delay);

        _overlay.SetActive(true);
    }

    public void GoToSolo()
    {
        manager.PlayClickForwardUI(transform);
        LoadingScript.LoadNewScene(Scenes.LobbySolo);
    }

    public void GoToMulti()
    {
        manager.PlayClickForwardUI(transform);
        LoadingScript.LoadNewScene(Scenes.LobbyMulti);
    }

    public void OpenMain()
    {
        m_customizationOverlay.SetActive(false);
        m_creditsOverlay.SetActive(false);
        m_optionsOverlay.SetActive(false);

        m_mainPosition.SetActive(true);
        m_customizationPosition.SetActive(false);
        m_creditsPosition.SetActive(false);
        m_optionsPosition.SetActive(false);

        StartCoroutine(DisplayOverlayAfterTime(2f, m_mainOverlay));
        FindObjectOfType<EventSystem>().SetSelectedGameObject(firstBtn);

        //if (FindObjectOfType<PodOnCustom>() != null)
        //{
        //    podOnCustom = FindObjectOfType<PodOnCustom>();
        //    if (podOnCustom != null)
        //    {
        //        podOnCustom.gameObject.SetActive(false);
        //    }
        //}
        manager.PlayClickBackwardUI(transform);
    }

    #region Customization
    public void OpenCustomization()
    {
        m_mainOverlay.SetActive(false);

        m_customizationPosition.SetActive(true);
        m_mainPosition.SetActive(false);

        StartCoroutine(DisplayOverlayAfterTime(2f, m_customizationOverlay));
        //if (podOnCustom != null)
        //{
        //    podOnCustom.gameObject.SetActive(true);
        //}

        manager.PlayClickForwardUI(transform);
    }

    public void CloseCustomization()
    {
        m_customizationOverlay.SetActive(false);

        m_mainPosition.SetActive(true);
        m_customizationPosition.SetActive(false);

        StartCoroutine(DisplayOverlayAfterTime(2f, m_mainOverlay));
        FindObjectOfType<EventSystem>().SetSelectedGameObject(firstBtn);

        //if (FindObjectOfType<PodOnCustom>() != null)
        //{
        //    podOnCustom = FindObjectOfType<PodOnCustom>();
        //    if (podOnCustom != null)
        //    {
        //        podOnCustom.gameObject.SetActive(false);
        //    }
        //}
        manager.PlayClickBackwardUI(transform);
    }
    #endregion

    #region Credits
    public void OpenCredits()
    {
        m_mainOverlay.SetActive(false);

        m_creditsPosition.SetActive(true);
        m_mainPosition.SetActive(false);

        StartCoroutine(DisplayOverlayAfterTime(2f, m_creditsOverlay));

        manager.PlayClickForwardUI(transform);
    }

    public void CloseCredits()
    {
        m_creditsOverlay.SetActive(false);

        m_mainPosition.SetActive(true);
        m_creditsPosition.SetActive(false);

        StartCoroutine(DisplayOverlayAfterTime(2f, m_mainOverlay));
        FindObjectOfType<EventSystem>().SetSelectedGameObject(firstBtn);
        
        manager.PlayClickBackwardUI(transform);
    }
    #endregion


    #region Lobby
    public void OpenLobby()
    {
        m_mainOverlay.SetActive(false);

        m_lobbyPosition.SetActive(true);
        m_mainPosition.SetActive(false);

        StartCoroutine(DisplayOverlayAfterTime(2f, m_lobbyOverlay));
        //lobbySetup.SetActive(true);

        manager.PlayClickForwardUI(transform);
    }

    public void CloseLobby()
    {
        //lobbySetup.SetActive(false);
        m_lobbyOverlay.SetActive(false);

        m_mainPosition.SetActive(true);
        m_lobbyPosition.SetActive(false);

        StartCoroutine(DisplayOverlayAfterTime(2f, m_mainOverlay));
        FindObjectOfType<EventSystem>().SetSelectedGameObject(firstBtn);

        manager.PlayClickBackwardUI(transform);
    }
    #endregion

    #region Options
    public void OpenOptions()
    {
        m_mainOverlay.SetActive(false);

        m_optionsPosition.SetActive(true);
        m_mainPosition.SetActive(false);

        StartCoroutine(DisplayOverlayAfterTime(2f, m_optionsOverlay));

        manager.PlayClickForwardUI(transform);
    }

    public void CloseOptions()
    {
        m_optionsOverlay.SetActive(false);

        m_mainPosition.SetActive(true);
        m_optionsPosition.SetActive(false);

        StartCoroutine(DisplayOverlayAfterTime(2f, m_mainOverlay));
        FindObjectOfType<EventSystem>().SetSelectedGameObject(firstBtn);

        manager.PlayClickBackwardUI(transform);
    }
    #endregion

    #region Exit
    public void QuitApplication()
        {
            RewiredManager rewired = RewiredManager.Instance;
            manager.StopSound();

    #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
    #else
                rewired.ExitGame();
                Application.Quit();
    #endif
        }
    #endregion
}
