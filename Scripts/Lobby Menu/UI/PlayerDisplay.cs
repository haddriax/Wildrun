using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDisplay : MonoBehaviour
{
    [SerializeField] int m_playerNumber;
    [SerializeField] GameObject empty;
    [SerializeField] GameObject connected;

    UserControllersManager m_userControllersManager;

    ConnectionHandler connectionHandler;

    void Awake()
    {
        m_userControllersManager = UserControllersManager.Instance();
        connectionHandler = FindObjectOfType<ConnectionHandler>();
    }

    void Update()
    {
        if (RewiredManager.Instance.PlayersConnected.Count >= m_playerNumber)
        {
            Connect();
        }
        else
        {
            Disconnect();
        }
    }

    public void Connect()
    {
        connected.SetActive(true);
        empty.SetActive(false);
    }

    public void Disconnect()
    {
        empty.SetActive(true);
        connected.SetActive(false);
    }
}
