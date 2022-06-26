using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Anomaly.Utils
{
    [System.Serializable]
    public partial class PolymorphValue<_Typ> where _Typ : struct
    {
        [SerializeField]
        private _Typ defaultValue = default(_Typ);

        [SerializeField]
        private SerializableDictionary<_Typ> otherValues = new SerializableDictionary<_Typ>();

        public _Typ Default => defaultValue;
        public _Typ Get(string key = "") => string.IsNullOrEmpty(key) ? defaultValue : otherValues.Get(key);
    }
}



#if UNITY_EDITOR
namespace Anomaly.Utils
{
    using UnityEditor;

    public partial class PolymorphValue<_Typ>
    {
        public void OnInspectorGUI(Editor editor, SerializedProperty target, string fieldName)
        {
            GUILayout.BeginVertical("box");

            GUILayout.BeginHorizontal("box");
            GUILayout.FlexibleSpace();
            GUILayout.Label(fieldName);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(5);

            GUILayout.BeginVertical("box");

            EditorGUILayout.PropertyField(target.FindPropertyRelative(nameof(defaultValue)), new GUIContent("Default"));
            otherValues.OnInspectorGUI(editor, target.FindPropertyRelative(nameof(otherValues)), "Others");

            GUILayout.EndVertical();

            GUILayout.EndVertical();
        }
    }
}
#endif