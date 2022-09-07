using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStatusData", menuName = "Status/Player")]
public class PlayerStatus : ActorStatus
{
    public CustomKey bindingKeyMap;

    public override void Initialize()
    {
        base.Initialize();

        // Register key map
        CustomKey.Current = bindingKeyMap;
    }
}
