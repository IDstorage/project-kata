#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Anomaly.Editor
{
    public static class EditorUtils
    {
        public static void Label(string text, int fontSize = 12, FontStyle fontStyle = FontStyle.Normal)
        {
            (int size, FontStyle style) prevSetting = (GUI.skin.label.fontSize, GUI.skin.label.fontStyle);

            GUI.skin.label.fontSize = fontSize;
            GUI.skin.label.fontStyle = fontStyle;

            GUILayout.Label(text);

            GUI.skin.label.fontSize = prevSetting.size;
            GUI.skin.label.fontStyle = prevSetting.style;
        }

        public static void DrawHorizontalLine(Color c, int thickness = 1, int marginLeft = 0, int marginRight = 0)
        {
            Rect rect = EditorGUILayout.GetControlRect(false, thickness);

            rect.x += marginLeft;
            rect.y -= marginRight;
            rect.height = thickness;

            EditorGUI.DrawRect(rect, c);
        }
    }
}
#endif