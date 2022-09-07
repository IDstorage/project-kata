using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "ActorStatusData", menuName = "Status/Actor")]
public class ActorStatus : ScriptableObject
{
    public float hp;
    public float maximumHP;

    [Range(0F, 1F)]
    public float posture;

    public float moveSpeed;
    public float moveSpeedMultiflier;


    public virtual void Initialize()
    {
    }
}
