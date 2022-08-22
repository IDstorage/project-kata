using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

[CreateAssetMenu(menuName = "ButtonMap/Windows", fileName = "WindowsButtonMap")]
public class WindowsButtonMap : ButtonMap
{
    private Dictionary<string, ButtonControl> container;

    private void Initialize()
    {
        if (container != null) return;
        container = new Dictionary<string, ButtonControl>()
        {
            { InputManager.Button.Attack, Mouse.current.leftButton },
            { InputManager.Button.Defense, Mouse.current.rightButton }
        };
    }

    public override ButtonControl GetButtonControl(string key)
    {
        Initialize();
        return container[key];
    }
}