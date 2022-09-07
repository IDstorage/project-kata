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
    [SerializeField] private Actor targetActor;

    [Space(10)]
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

        var c = gaugeColor.Evaluate(value);
        c.a = Anomaly.Utils.Math.IsZero(value) ? 0F : 1F;
        gaugeUI.color = c;
    }

#if UNITY_EDITOR
    public void UpdateValue(float v = 0F)
    {
        if (targetActor == null) return;
        targetActor.AddPosture(v);
    }
#endif

    public void OnUpdate()
    {
        if (targetActor == null) return;

        var status = targetActor.Status;

        float scale = 1F - status.posture;
        bool valueChanged = value != scale;

        if (!valueChanged) return;

        SetValue(scale);
    }
}


#if UNITY_EDITOR
[CustomEditor(typeof(PostureUI))]
public class PostureUIEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (!EditorApplication.isPlaying) return;

        GUILayout.Space(10);

        EditorGUILayout.BeginHorizontal("box");
        if (GUILayout.Button("+", GUILayout.Height(30)))
        {
            (target as PostureUI).UpdateValue(-Random.Range(0.05f, 0.2f));
        }
        if (GUILayout.Button("-", GUILayout.Height(30)))
        {
            (target as PostureUI).UpdateValue(Random.Range(0.05f, 0.2f));
        }
        EditorGUILayout.EndHorizontal();
    }
}
#endif
