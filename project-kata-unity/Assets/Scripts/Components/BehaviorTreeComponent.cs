using System.Collections.Generic;
using UnityEngine;
using Anomaly;
using UnityBehaviorTree;

[System.Serializable]
public class BehaviorTreeComponent : CustomComponent, IUpdater
{
    [SerializeField]
    private GameObject targetAI;

    [SerializeField]
    private BehaviorTree targetBehaviorTree;


    public void SetBehaviorTree(BehaviorTree tree)
    {
        targetBehaviorTree = tree;
    }

    public void Update()
    {
        targetBehaviorTree?.Update(targetAI, Time.deltaTime);
    }


#if UNITY_EDITOR
    public override void OnInspectorGUI(CustomComponent target)
    {
        base.OnInspectorGUI(target);

        GUILayout.Space(10);

        if (GUILayout.Button("Show"))
        {
            UnityEditor.EditorApplication.ExecuteMenuItem("Tools/BehaviorTree Viewer");
            var window = UnityEditor.EditorWindow.GetWindow<BehaviorTreeViewer>();
            window.TargetBehaviorTree = (target as BehaviorTreeComponent).targetBehaviorTree;
        }
    }
#endif
}