using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Anomaly;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class PostureUI : CustomBehaviour
{
    [SerializeField] private RectTransform rootUI;
    [SerializeField] private RectTransform bgUI;
    [SerializeField] private Image gaugeUI;

    [Space(10)]
    [SerializeField] private Gradient gaugeColor;
    [SerializeField, Range(0F, 1F)] private float value = 0F;

    public void SetValue(float percent)
    {
        if (rootUI == null || gaugeUI == null || bgUI == null) return;

        value = Mathf.Clamp01(percent);

        float scale = gaugeUI.rectTransform.rect.height / rootUI.rect.height;

        var size = gaugeUI.rectTransform.sizeDelta;
        size.x = Mathf.Lerp(246F, rootUI.rect.width * scale, percent);
        gaugeUI.rectTransform.sizeDelta = size;

        bgUI.sizeDelta = new Vector2(rootUI.rect.width * scale, bgUI.sizeDelta.y);

        gaugeUI.color = gaugeColor.Evaluate(value);
    }

#if UNITY_EDITOR
    public void UpdateValue()
    {
        var c = gaugeUI.color;
        c.a = Anomaly.Utils.Math.IsZero(value) ? 0F : 1F;
        gaugeUI.color = c;
        SetValue(value);
    }
#endif

    protected override void Initialize()
    {
    }
}


#if UNITY_EDITOR
[CustomEditor(typeof(PostureUI))]
public class PostureUIEditor : Editor
{
    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();
        base.OnInspectorGUI();
        if (EditorGUI.EndChangeCheck())
        {
            (target as PostureUI).UpdateValue();
        }
    }
}
#endif
