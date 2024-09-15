using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Anomaly.Utils;

[CreateAssetMenu(menuName = "Custom Key Mapping", fileName = "NewCustomKey")]
public class CustomKey : ScriptableObject
{
    public static CustomKey Current { get; set; }

    public AKeyCode Attack = AKeyCode.MouseLeft;
    public AKeyCode Defense = AKeyCode.MouseRight;

    public AKeyCode Sprint = AKeyCode.LeftShift;

    public AKeyCode Targeting = AKeyCode.Q;
}