#if UNITY_EDITOR

namespace Anomaly.Editor
{
    using System;
    using UnityEngine;
    using UnityEditor;
    using System.Collections.Generic;

    [CustomEditor(typeof(CustomObject), true)]
    public class CustomObjectEditor : Editor
    {
        private CustomObject self = null;
        private Dictionary<string, bool> editorFold = new Dictionary<string, bool>();

        private List<System.Reflection.FieldInfo> serializedFields = new List<System.Reflection.FieldInfo>();
        private List<System.Reflection.FieldInfo> componentDataList = new List<System.Reflection.FieldInfo>();

        private int selectedTab = 0;
        private string[] tabs = new string[] {
            "All",
            "Componets",
            "Fields"
        };


        private void OnEnable()
        {
            self = target as CustomObject;
            self.InitializeComponents(target.GetType());


            var fields = target.GetType().GetFields(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);
            foreach (var field in fields)
            {
                if (!field.FieldType.IsSubclassOf(typeof(CustomComponent.BaseData)))
                {
                    if (Attribute.GetCustomAttribute(field, typeof(HideInInspector)) != null) continue;
                    if (!field.IsPublic && Attribute.GetCustomAttribute(field, typeof(SerializeField)) == null) continue;

                    serializedFields.Add(field);
                    continue;
                }

                componentDataList.Add(field);
            }
        }


        public override void OnInspectorGUI()
        {
            GUILayout.Space(5);

            selectedTab = GUILayout.Toolbar(selectedTab, tabs);

            GUILayout.Space(10);

            switch (selectedTab)
            {
                case 0:
                    ShowComponentTab();
                    GUILayout.Space(10);
                    EditorUtils.DrawHorizontalLine(Color.gray);
                    GUILayout.Space(20);
                    ShowBaseTab();
                    break;
                case 1:
                    ShowComponentTab();
                    break;
                case 2:
                    ShowBaseTab();
                    break;
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void ShowComponentTab()
        {
            EditorGUILayout.BeginVertical("box");

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorUtils.Label("Component Data", 16, FontStyle.Bold);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUI.indentLevel++;

            for (int i = 0; i < componentDataList.Count; ++i)
            {
                EditorGUILayout.BeginVertical("box");
                SerializeAll(componentDataList[i].FieldType.FullName.Split('+')[0].Replace("Component", ""), componentDataList[i].Name);
                EditorGUILayout.EndVertical();
            }

            EditorGUI.indentLevel--;

            EditorGUILayout.EndVertical();
        }

        private void ShowBaseTab()
        {
            float prevWidth = EditorGUIUtility.labelWidth;

            EditorGUIUtility.labelWidth = 70F;

            for (int i = 0; i < serializedFields.Count; ++i)
            {
                Serialize(serializedFields[i].Name);
            }

            EditorGUIUtility.labelWidth = prevWidth;
        }

        private void SerializeAll(string title, string fieldName)
        {
            var serializedProperty = serializedObject.FindProperty(fieldName);

            if (!editorFold.ContainsKey(title)) editorFold.Add(title, true);
            editorFold[title] = EditorGUILayout.Foldout(editorFold[title], title);

            EditorGUI.indentLevel += 2;

            if (editorFold[title])
            {
                var fieldInfo = serializedProperty.serializedObject.targetObject.GetType().GetField(fieldName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);
                var fields = fieldInfo.FieldType.GetFields(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
                foreach (var field in fields)
                {
                    EditorGUILayout.PropertyField(serializedProperty.FindPropertyRelative(field.Name), true);
                }
            }

            EditorGUI.indentLevel -= 2;
        }

        private void Serialize(string fieldName)
        {
            var serializedProperty = serializedObject.FindProperty(fieldName);
            if (serializedProperty == null) return;

            EditorGUILayout.PropertyField(serializedProperty, true);
        }
    }
}
#endif