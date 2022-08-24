using System.Collections;
using System.Collections.Generic;
using Anomaly;
using UnityEngine;

public class CustomKeySelector : CustomBehaviour
{
    [SerializeField]
    private CustomKey keyMap;

    protected override void Initialize()
    {
        CustomKey.Current = keyMap;
    }
}
