using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "ActorStatusData", menuName = "Status/Actor")]
public class ActorStatus : ScriptableObject
{
    [Header("Variables")]
    public float hp;
    [Range(0F, 1F)]
    public float posture;

    [Header("Stat")]
    public float maximumHP;
    [Space(5)]
    [Tooltip("Increase amount per second")]
    public float postureIncreaseSpeed;
    [Tooltip("pds <=> health")]
    public AnimationCurve postureIncreaseSpeedScale;
    [Tooltip("Unit: sec")]
    public float postureIncreaseDelay;
    [Tooltip("pdd <=> health")]
    public AnimationCurve postureIncreaseDelayScale;
    [Space(5)]
    public float moveSpeed;
    public float moveSpeedMultiflier;

    [Tooltip("Events")]
    public System.Action<float> onHPChanged, onPostureChanged;


    public float GetPostureIncreaseDelay(float hpScale) => postureIncreaseDelayScale.Evaluate(hpScale) * postureIncreaseDelay;
    public float GetPostureIncreaseSpeed(float hpScale) => postureIncreaseSpeedScale.Evaluate(hpScale) * postureIncreaseSpeed * Time.deltaTime;

    public virtual void Initialize()
    {
    }
}
