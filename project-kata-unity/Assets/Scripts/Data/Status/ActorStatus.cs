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

    [Space(10)]
    [Tooltip("Increase amount per second")]
    public float postureIncreaseSpeed;
    [Tooltip("pds <=> health")]
    public AnimationCurve postureIncreaseSpeedScale;
    [Tooltip("Unit: sec")]
    public float postureIncreaseDelay;
    [Tooltip("pdd <=> health")]
    public AnimationCurve postureIncreaseDelayScale;

    [Space(10)]
    public Vector2 parryTimingRange;
    public float defenseToParryInterval;
    public float currentParryTiming;
    public int parryTimingDecreaseCount;

    [Space(10)]
    public float moveSpeed;
    public float moveSpeedMultiflier;


    public System.Action<float> onHPChanged, onPostureChanged;


    public void SetHP(float v, bool notify = true)
    {
        hp = Mathf.Clamp(v, 0F, maximumHP);
        if (notify) onHPChanged?.Invoke(hp / maximumHP);
    }
    public void AddHP(float value, bool notify = true) => SetHP(hp + value, notify);

    public void SetPosture(float v, bool notify = true)
    {
        posture = Mathf.Clamp01(v);
        if (notify) onPostureChanged?.Invoke(1F - posture);
    }
    public void AddPosture(float value, bool notify = true) => SetPosture(posture + value, notify);

    public float GetPostureIncreaseDelay(float hpScale) => postureIncreaseDelayScale.Evaluate(hpScale) * postureIncreaseDelay;
    public float GetPostureIncreaseSpeed(float hpScale) => postureIncreaseSpeedScale.Evaluate(hpScale) * postureIncreaseSpeed * Time.deltaTime;

    public void DecreaseParryTiming()
    {
        currentParryTiming = Mathf.Clamp(currentParryTiming - (parryTimingRange.y - parryTimingRange.x) / parryTimingDecreaseCount,
                                        parryTimingRange.x, parryTimingRange.y);
    }
    public void ResetParryTiming()
    {
        currentParryTiming = parryTimingRange.y;
    }


    public virtual void Initialize() { }
}
