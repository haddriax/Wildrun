using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnumLayouts
{
    RightHanded,
    LeftHanded,
    Nintendo,
    Stick
}
public enum EnumActions
{
    Move,
    Accelerate,
    Brake,
    Turn,
    Handbrake,
    Special,
    Honk,
    Nothing,

    test
}
public enum EnumInputs
{
    AxisVerticalLeft,
    AxisHorizontalLeft,
    AxisHorizontalRight,
    AxisVerticalRight,
    AxisVerticalCross,
    AxisHorizontalCross,
    ButtonA,
    ButtonB,
    ButtonX,
    ButtonY,
    ButtonLeft,
    ButtonRight,
    ButtonStart,
    ButtonSelect,
    TriggerLeft,
    TriggerRight,

    AxisTriggers
}

public class UserController
{
    EnumLayouts m_layout = EnumLayouts.RightHanded;
    CharacterController m_characterController;
    public CharacterController CharacterController => m_characterController;
    Dictionary<EnumInputs, EnumActions> m_bindings = new Dictionary<EnumInputs, EnumActions>();

    public UserController()
    {
        m_characterController = new CharacterController();
        InitDictionnary();
    }

    public UserController(CharacterController _characterController)
    {
        Debug.Log("VehiclePlayerPhysicController" + _characterController + "Added to UserController");

        m_characterController = _characterController;
        InitDictionnary();
    }

    

    void InitDictionnary()
    {
        switch (m_layout)
        {
            case EnumLayouts.RightHanded:
                m_bindings.Add(EnumInputs.TriggerRight, EnumActions.Accelerate);
                m_bindings.Add(EnumInputs.TriggerLeft, EnumActions.Brake);
                m_bindings.Add(EnumInputs.AxisHorizontalLeft, EnumActions.Turn);
                m_bindings.Add(EnumInputs.AxisVerticalLeft, EnumActions.Nothing);
                m_bindings.Add(EnumInputs.AxisHorizontalRight, EnumActions.Nothing);
                m_bindings.Add(EnumInputs.AxisVerticalRight, EnumActions.Nothing);
                m_bindings.Add(EnumInputs.AxisHorizontalCross, EnumActions.Nothing);
                m_bindings.Add(EnumInputs.AxisVerticalCross, EnumActions.Nothing);
                m_bindings.Add(EnumInputs.ButtonA, EnumActions.Nothing);
                m_bindings.Add(EnumInputs.ButtonB, EnumActions.Nothing);
                m_bindings.Add(EnumInputs.ButtonX, EnumActions.Nothing);
                m_bindings.Add(EnumInputs.ButtonY, EnumActions.Nothing);
                m_bindings.Add(EnumInputs.ButtonLeft, EnumActions.Nothing);
                m_bindings.Add(EnumInputs.ButtonRight, EnumActions.Nothing);
                m_bindings.Add(EnumInputs.ButtonSelect, EnumActions.Nothing);
                m_bindings.Add(EnumInputs.ButtonStart, EnumActions.Nothing);
                break;

            case EnumLayouts.LeftHanded:
                m_bindings.Add(EnumInputs.TriggerRight, EnumActions.Brake);
                m_bindings.Add(EnumInputs.TriggerLeft, EnumActions.Accelerate);
                m_bindings.Add(EnumInputs.AxisHorizontalLeft, EnumActions.Nothing);
                m_bindings.Add(EnumInputs.AxisVerticalLeft, EnumActions.Nothing);
                m_bindings.Add(EnumInputs.AxisHorizontalRight, EnumActions.Turn);
                m_bindings.Add(EnumInputs.AxisVerticalRight, EnumActions.Nothing);
                m_bindings.Add(EnumInputs.AxisHorizontalCross, EnumActions.Nothing);
                m_bindings.Add(EnumInputs.AxisVerticalCross, EnumActions.Nothing);
                m_bindings.Add(EnumInputs.ButtonA, EnumActions.Nothing);
                m_bindings.Add(EnumInputs.ButtonB, EnumActions.Nothing);
                m_bindings.Add(EnumInputs.ButtonX, EnumActions.Nothing);
                m_bindings.Add(EnumInputs.ButtonY, EnumActions.Nothing);
                m_bindings.Add(EnumInputs.ButtonLeft, EnumActions.Nothing);
                m_bindings.Add(EnumInputs.ButtonRight, EnumActions.Nothing);
                m_bindings.Add(EnumInputs.ButtonSelect, EnumActions.Nothing);
                m_bindings.Add(EnumInputs.ButtonStart, EnumActions.Nothing);
                break;

            case EnumLayouts.Nintendo:
                m_bindings.Add(EnumInputs.TriggerRight, EnumActions.Nothing);
                m_bindings.Add(EnumInputs.TriggerLeft, EnumActions.Nothing);
                m_bindings.Add(EnumInputs.AxisHorizontalLeft, EnumActions.Turn);
                m_bindings.Add(EnumInputs.AxisVerticalLeft, EnumActions.Nothing);
                m_bindings.Add(EnumInputs.AxisHorizontalRight, EnumActions.Nothing);
                m_bindings.Add(EnumInputs.AxisVerticalRight, EnumActions.Nothing);
                m_bindings.Add(EnumInputs.AxisHorizontalCross, EnumActions.Nothing);
                m_bindings.Add(EnumInputs.AxisVerticalCross, EnumActions.Nothing);
                m_bindings.Add(EnumInputs.ButtonA, EnumActions.Accelerate);
                m_bindings.Add(EnumInputs.ButtonB, EnumActions.Brake);
                m_bindings.Add(EnumInputs.ButtonX, EnumActions.Nothing);
                m_bindings.Add(EnumInputs.ButtonY, EnumActions.Nothing);
                m_bindings.Add(EnumInputs.ButtonLeft, EnumActions.Nothing);
                m_bindings.Add(EnumInputs.ButtonRight, EnumActions.Nothing);
                m_bindings.Add(EnumInputs.ButtonSelect, EnumActions.Nothing);
                m_bindings.Add(EnumInputs.ButtonStart, EnumActions.Nothing);
                break;

            case EnumLayouts.Stick:
                m_bindings.Add(EnumInputs.TriggerRight, EnumActions.Nothing);
                m_bindings.Add(EnumInputs.TriggerLeft, EnumActions.Nothing);
                m_bindings.Add(EnumInputs.AxisHorizontalLeft, EnumActions.Turn);
                m_bindings.Add(EnumInputs.AxisVerticalLeft, EnumActions.Move);
                m_bindings.Add(EnumInputs.AxisHorizontalRight, EnumActions.Nothing);
                m_bindings.Add(EnumInputs.AxisVerticalRight, EnumActions.Nothing);
                m_bindings.Add(EnumInputs.AxisHorizontalCross, EnumActions.Nothing);
                m_bindings.Add(EnumInputs.AxisVerticalCross, EnumActions.Nothing);
                m_bindings.Add(EnumInputs.ButtonA, EnumActions.Nothing);
                m_bindings.Add(EnumInputs.ButtonB, EnumActions.Nothing);
                m_bindings.Add(EnumInputs.ButtonX, EnumActions.Nothing);
                m_bindings.Add(EnumInputs.ButtonY, EnumActions.Nothing);
                m_bindings.Add(EnumInputs.ButtonLeft, EnumActions.Nothing);
                m_bindings.Add(EnumInputs.ButtonRight, EnumActions.Nothing);
                m_bindings.Add(EnumInputs.ButtonSelect, EnumActions.Nothing);
                m_bindings.Add(EnumInputs.ButtonStart, EnumActions.Nothing);
                break;
        }
    }

    public void Handle(EnumInputs _input, float _intensity)
    {
        switch (_input)
        {
            default:
                {
                    m_characterController.Do(m_bindings[_input], _intensity);
                }
                break;
        }
    }

    public void Rebind(EnumInputs _input, EnumActions _action)
    {
        m_bindings[_input] = _action;
    }
}
