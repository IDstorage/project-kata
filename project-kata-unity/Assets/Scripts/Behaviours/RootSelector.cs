using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootSelector : Anomaly.CustomBehaviour
{
    [SerializeField]
    private Anomaly.CustomBehaviour root;
    public Anomaly.CustomBehaviour Root => root;

    protected override void Initialize() { }
}
