using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Anomaly.Utils
{
    [System.Serializable]
    public partial class FlexibleValue
    {
        [SerializeField]
        private float defaultValue = default(float);

        private float additionValue = default(float);
        private float multiplierValue = default(float);
        private float finalAdditionValue = default(float);
        private float finalMultiplierValue = default(float);

        public float Value => (defaultValue * multiplierValue + additionValue) * finalMultiplierValue + finalAdditionValue;
    }
}



#if UNITY_EDITOR
namespace Anomaly.Utils
{
    using UnityEditor;

    public partial class FlexibleValue
    {
        public void OnInspectorGUI(string fieldName)
        {
            GUILayout.BeginHorizontal("box");

            GUILayout.BeginHorizontal("box");
            GUILayout.FlexibleSpace();
            GUILayout.Label(fieldName);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(5);

            GUILayout.BeginVertical("box");

            EditorGUILayout.FloatField(defaultValue);

            GUILayout.EndVertical();

            GUILayout.EndHorizontal();
        }
    }
}
#endif