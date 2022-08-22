using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class InputManager : Anomaly.Utils.CustomSingletonBehaviour<InputManager>
{
    public class Button
    {
        public const string Attack = "Mouse Left";
        public const string Defense = "Mouse Right";
    }

    [SerializeField]
    private ButtonMap map;

    public enum Status
    {
        None,

        Begin,
        Hold,
        End
    }

    public Status GetButtonStatus(string key)
    {
        var button = map.GetButtonControl(key);
        if (button.wasPressedThisFrame) return Status.Begin;
        if (button.wasReleasedThisFrame) return Status.End;
        return button.isPressed ? Status.Hold : Status.None;
    }

    public bool IsPressed(string key)
    {
        return GetButtonStatus(key) == Status.Begin;
    }
    public bool IsHeld(string key)
    {
        return GetButtonStatus(key) == Status.Hold;
    }
    public bool IsReleased(string key)
    {
        return GetButtonStatus(key) == Status.End;
    }


    protected override void Initialize()
    {
        DontDestroyOnLoad(gameObject);
    }
}
