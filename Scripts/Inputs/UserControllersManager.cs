using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.IO;
using System;

public struct Controller
{
    public UserController m_userController;
    public int m_joystickIndex;
}

public class UserControllersManager
{
    static UserControllersManager m_instance;

    public static UserControllersManager Instance()
    {
        if(m_instance == null)
        {
            m_instance = new UserControllersManager();
        }

        return m_instance;
    }

    static List<Controller> m_controllers = new List<Controller>();

    public int NbControllers => m_controllers.Count;

    void Start()
    {
    }

    // Update is called once per frame
    public void Update()
    {
        // Boucle en jeu
        for (int i = 0; i < m_controllers.Count; i++)
        {
            UserController currentController = m_controllers[i].m_userController;
            if (!currentController.CharacterController.characterLapManager.Finished)
            {
                int currentJoystick = m_controllers[i].m_joystickIndex;

                #region Controllers
                // Joysticks
                currentController.Handle(EnumInputs.AxisHorizontalLeft, Input.GetAxis("HorizontalLeft_" + currentJoystick));        // Left/Right of left Joystick  -OR- 'Q'/'D'
                currentController.Handle(EnumInputs.AxisVerticalLeft, Input.GetAxis("VerticalLeft_" + currentJoystick));            // Up/Down of left Joystick     -OR- 'Z'/'S'
                currentController.Handle(EnumInputs.AxisHorizontalRight, Input.GetAxis("HorizontalRight_" + currentJoystick));      // Left/Right of right Joystick -OR- '←'/'→'
                currentController.Handle(EnumInputs.AxisVerticalRight, Input.GetAxis("VerticalRight_" + currentJoystick));          // Up/Down of right Joystick    -OR- '↑'/'↓'

                ////Triggers
                currentController.Handle(EnumInputs.TriggerRight, Input.GetAxis("TriggerRight_" + currentJoystick));                             // Right trigger                -OR- 
                currentController.Handle(EnumInputs.TriggerLeft, Input.GetAxis("TriggerLeft_" + currentJoystick));                               // Left Trigger                 -OR- 

                //currentController.Handle(EnumInputs.AxisTriggers, Input.GetAxis("Triggers_" + currentJoystick));                    // Left Trigger                 -OR- 

                //// Cross Pad
                currentController.Handle(EnumInputs.AxisVerticalCross, Input.GetAxisRaw("VerticalCross_" + currentJoystick));       // Up/Down of cross-pad         -OR- '1'/'3'
                currentController.Handle(EnumInputs.AxisHorizontalCross, Input.GetAxisRaw("HorizontalCross_" + currentJoystick));   // Right/Left of cross-pad      -OR- '2'/'4'

                //// Buttons
                currentController.Handle(EnumInputs.ButtonA, Input.GetAxisRaw("ButtonA_" + currentJoystick));                       // Button A                     -OR- 
                currentController.Handle(EnumInputs.ButtonB, Input.GetAxisRaw("ButtonB_" + currentJoystick));                       // Button B                     -OR- 
                currentController.Handle(EnumInputs.ButtonX, Input.GetAxisRaw("ButtonX_" + currentJoystick));                       // Button X                     -OR- 
                currentController.Handle(EnumInputs.ButtonY, Input.GetAxisRaw("ButtonY_" + currentJoystick));                       // Button Y                     -OR- 
                currentController.Handle(EnumInputs.ButtonLeft, Input.GetAxisRaw("ButtonRight_" + currentJoystick));                // Button Right                 -OR- 
                currentController.Handle(EnumInputs.ButtonRight, Input.GetAxisRaw("ButtonLeft_" + currentJoystick));                // Button Left                  -OR- 
                currentController.Handle(EnumInputs.ButtonStart, Input.GetAxisRaw("ButtonStart_" + currentJoystick));               // Button Start                 -OR- 'Esc'
                currentController.Handle(EnumInputs.ButtonSelect, Input.GetAxisRaw("ButtonSelect_" + currentJoystick));             // Button Select                -OR- 'Pause'
                #endregion

                #region Keyboard
                // Nothing for now
                #endregion
            }
            else
            {
                currentController.CharacterController.MainController.AccelerationValue = 0;
                currentController.CharacterController.MainController.TurnValue = 0;
            }

        }
    }

    public void AddController(int _joystickIndex)
    {
        if ((m_controllers.FindAll(x => x.m_joystickIndex == _joystickIndex)).Count == 0)
        {
            CharacterController cc = new CharacterController();
            cc.SetMainController(m_controllers.Count + 1);
            m_controllers.Add(new Controller { m_userController = new UserController(cc), m_joystickIndex = _joystickIndex });
        }
        Debug.Log(m_controllers.Count + " : " + m_controllers[m_controllers.Count - 1].m_joystickIndex);
    }

    public void RemoveController(int _joystickIndex)
    {
        m_controllers.RemoveAll(x => x.m_joystickIndex == _joystickIndex);

        Debug.Log(m_controllers.Count + " : " + m_controllers[m_controllers.Count -1].m_joystickIndex);
    }

    public void RemoveAllController()
    {
        m_controllers.Clear();
    }

    public bool IsControllerConnected(int _playerNumber)
    {
        return (m_controllers.Count > _playerNumber);
    }

    public void RebindKey(EnumInputs _button, EnumActions _action, int _controllerId)
    {

    }
}
