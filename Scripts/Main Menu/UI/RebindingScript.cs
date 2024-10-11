using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RebindingScript : MonoBehaviour
{
    UnityEvent m_unityEvent;
    UserControllersManager m_userControllerManager;

    EnumInputs m_buttonPressed;
    [SerializeField] EnumActions m_actionRebound;

    [SerializeField] Text m_text;


    string m_label;


    bool m_isEventActive = false;
    Event e;

    // Start is called before the first frame update
    void Start()
    {
        m_label = m_text.text;

        if(m_unityEvent == null)
        {
            m_unityEvent = new UnityEvent();
        }
    }

    public void Select()
    {
        m_text.text = "Press any button";
        m_isEventActive = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Unselect()
    {
        m_text.text = m_label;
        m_isEventActive = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void OnGUI()
    {
        if (m_isEventActive)
        {
            e = Event.current;
            if (e.isKey)
            {
                if (e.keyCode == KeyCode.Space)
                {
                    Debug.Log("Space Pressed");
                    Unselect();
                }
                else
                {
                    Debug.Log(e.keyCode.ToString());
                }
            }
        }
    }

    
}
