using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

public abstract class ButtonMap : ScriptableObject
{
    public abstract ButtonControl GetButtonControl(string key);
}