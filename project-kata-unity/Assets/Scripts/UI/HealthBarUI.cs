using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Anomaly;
using Anomaly.Utils;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class HealthBarUI : CustomBehaviour
{
    [SerializeField] private Actor targetActor;

    [Space(10)]
    [SerializeField] private Image gaugeUI;
    [SerializeField] private Image gaugeFollowerUI;

    [Space(10)]
    [SerializeField] private Gradient gaugeColor;
    [SerializeField, Range(0F, 1F)] private float value = 1F;

    [Space(10)]
    [SerializeField] private float animateDelay = 1F;
    [SerializeField] private float animateScale = 1F;


    private SmartCoroutine coroutineInstance;


    public void SetValue(float v)
    {
        if (gaugeUI == null || gaugeFollowerUI == null) return;

        value = Mathf.Clamp01(v);

        gaugeUI.color = gaugeColor.Evaluate(value);

        gaugeUI.fillAmount = value;
        if (gaugeFollowerUI.fillAmount <= value)
        {
            gaugeFollowerUI.fillAmount = value;
            return;
        }

        if (coroutineInstance == null) coroutineInstance = SmartCoroutine.Create(CoAnimate);
        else coroutineInstance.Stop();

        coroutineInstance.Start();

        IEnumerator CoAnimate()
        {
            yield return new WaitForSeconds(animateDelay);

            while (gaugeFollowerUI.fillAmount > gaugeUI.fillAmount)
            {
                gaugeFollowerUI.fillAmount -= Time.deltaTime * animateScale;
                yield return null;
            }

            gaugeFollowerUI.fillAmount = gaugeUI.fillAmount;
        }
    }

#if UNITY_EDITOR
    public void UpdateValue(float v = 0F)
    {
        if (targetActor == null) return;
        targetActor.AddHP(v);
    }
#endif

    public void OnUpdate()
    {
        if (targetActor == null) return;

        var status = targetActor.Status;

        float scale = status.hp / status.maximumHP;
        bool valueChanged = gaugeUI.fillAmount != scale;

        if (!valueChanged) return;

        SetValue(scale);
    }
}


#if UNITY_EDITOR
[CustomEditor(typeof(HealthBarUI))]
public class HealthBarUIEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (!EditorApplication.isPlaying) return;

        GUILayout.Space(10);

        EditorGUILayout.BeginHorizontal("box");
        if (GUILayout.Button("+", GUILayout.Height(30)))
        {
            (target as HealthBarUI).UpdateValue(Random.Range(5F, 20F));
        }
        if (GUILayout.Button("-", GUILayout.Height(30)))
        {
            (target as HealthBarUI).UpdateValue(-Random.Range(5F, 20F));
        }
        EditorGUILayout.EndHorizontal();
    }
}
#endif
