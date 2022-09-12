using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Anomaly;
using Anomaly.Utils;
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

    private SmartCoroutine coroutineInstance;

    public void SetValue(float percent)
    {
        if (rootUI == null || gaugeUI == null || bgUI == null) return;

        SetGauge(percent);

        if (coroutineInstance == null) coroutineInstance = SmartCoroutine.Create(CoAnimate);
        else coroutineInstance.Stop();

        coroutineInstance.Start();

        IEnumerator CoAnimate()
        {
            float hpScale = targetActor.Status.hp / targetActor.Status.maximumHP;

            yield return new WaitForSeconds(targetActor.Status.GetPostureIncreaseDelay(hpScale));

            while (targetActor.Status.posture < 1F)
            {
                targetActor.AddPosture(targetActor.Status.GetPostureIncreaseSpeed(hpScale), false);
                SetGauge(1F - targetActor.Status.posture);
                yield return null;
            }

            targetActor.SetPosture(1F, false);
        }
    }

    private void SetGauge(float percent)
    {
        value = Mathf.Clamp01(percent);

        float scale = gaugeUI.rectTransform.rect.height / rootUI.rect.height;

        var size = gaugeUI.rectTransform.sizeDelta;
        size.x = Mathf.Lerp(246F, rootUI.rect.width * scale, value);
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
        targetActor.AddPosture(v, false);
    }
#endif


    private void OnEnable()
    {
        if (targetActor == null) return;
        targetActor.Status.onPostureChanged += SetValue;
    }
    private void OnDisable()
    {
        if (targetActor == null) return;
        targetActor.Status.onPostureChanged -= SetValue;
    }

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
