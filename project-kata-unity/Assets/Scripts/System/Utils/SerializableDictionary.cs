using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Anomaly.Utils
{
    [System.Serializable]
    public partial class SerializableDictionary<_Typ> : ISerializationCallbackReceiver
    {
        #region Constructor
        public SerializableDictionary() { }
        public SerializableDictionary(string defaultKey, _Typ defaultValue = default(_Typ))
        {
            keyList.Add(defaultKey);
            valueList.Add(defaultValue);
            OnAfterDeserialize();
        }
        public SerializableDictionary(IEnumerable<string> defaultKeys, IEnumerable<_Typ> defaultValues = null)
        {
            Debug.Assert(defaultKeys.Count() == defaultValues.Count());
            keyList.AddRange(defaultKeys);
            valueList.AddRange(defaultValues);
            OnAfterDeserialize();
        }
        #endregion

        public Dictionary<string, _Typ> Container { get; set; } = new Dictionary<string, _Typ>();

        public _Typ Get(string key)
        {
            Debug.Assert(Container.TryGetValue(key, out var value));
            return value;
        }

        #region Serialization
        [SerializeField]
        private List<string> keyList = new List<string>();
        [SerializeField]
        private List<_Typ> valueList = new List<_Typ>();

        public void OnAfterDeserialize()
        {
            Container.Clear();
            for (int i = 0; i < keyList.Count; ++i)
            {
                if (Container.ContainsKey(keyList[i]))
                {
                    keyList[i] = System.Guid.NewGuid().ToString();
                }
                Container.Add(keyList[i], valueList[i]);
            }
            keyList = null;
            valueList = null;
        }
        public void OnBeforeSerialize()
        {
            keyList = Container.Keys.ToList();
            valueList = Container.Values.ToList();
        }
        #endregion
    }
}



#if UNITY_EDITOR
namespace Anomaly.Utils
{
    using UnityEditor;

    public partial class SerializableDictionary<_Typ>
    {
        public void OnInspectorGUI(Editor editor, SerializedProperty target, string fieldName)
        {
            OnBeforeSerialize();

            EditorGUILayout.BeginVertical("box");

            var keyProperty = target.FindPropertyRelative(nameof(keyList));
            var valueProperty = target.FindPropertyRelative(nameof(valueList));

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(fieldName);
            if (GUILayout.Button("+", GUILayout.Width(40)))
            {
                keyProperty.arraySize++;
                keyProperty.GetArrayElementAtIndex(keyProperty.arraySize - 1).stringValue = System.Guid.NewGuid().ToString();
                valueProperty.arraySize++;
            }
            EditorGUILayout.EndHorizontal();

            for (int i = 0; i < keyProperty.arraySize; ++i)
            {
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.PropertyField(keyProperty.GetArrayElementAtIndex(i), new GUIContent(""), GUILayout.Width(120));
                GUILayout.Space(5);
                EditorGUILayout.PropertyField(valueProperty.GetArrayElementAtIndex(i), new GUIContent(""));

                GUILayout.Space(40);

                Color prevColor = GUI.backgroundColor;
                GUI.backgroundColor = Color.red;
                if (GUILayout.Button("-", GUILayout.Width(40)))
                {
                    keyProperty.DeleteArrayElementAtIndex(i);
                    valueProperty.DeleteArrayElementAtIndex(i);
                    break;
                }
                GUI.backgroundColor = prevColor;

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();
        }
    }
}
#endif