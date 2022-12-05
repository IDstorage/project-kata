using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PatternSequenceEditorWindow : EditorWindow
{
    private class WindowInfo
    {
        public int id;

        public int x, y;
        public string title = System.Guid.NewGuid().ToString();
        public GUI.WindowFunction func;

        public Rect windowRect;
        public (int x, int y) margin;

        public Rect Rect => new Rect(windowRect.x + (windowRect.width + margin.x) * x, windowRect.y + (windowRect.height + margin.y) * y, windowRect.width, windowRect.height);

        public bool IsBranch => node is PatternSequence.Branch;

        public PatternSequence.Node node;
        public WindowInfo next, alternative;
    }

    private static PatternSequenceEditorWindow window;
    private static Vector2 mainFormSize, snbSize;

    private Vector2 rootSpace = Vector2.zero;

    private PatternSequence targetSequence;
    private string sequenceName;

    private int globalID = 100000;
    private Dictionary<int, WindowInfo> windows = new Dictionary<int, WindowInfo>();
    private WindowInfo head;
    private Rect windowRect = new Rect(0, 0, 150, 80);
    private (int x, int y) margin = (10, 20);


    [MenuItem("Tools/Pattern Sequence Editor")]
    private static void ShowWindow()
    {
        if (window != null)
        {
            window.Close();
            window = null;
        }
        window = EditorWindow.CreateInstance<PatternSequenceEditorWindow>();

        window.ShowUtility();

        mainFormSize = new Vector2(700F, 400F);
        snbSize = new Vector2(290F, 400F);

        window.minSize = mainFormSize;

        // Center
        var mainWindow = EditorGUIUtility.GetMainWindowPosition();
        var current = window.position;

        current.x = (mainWindow.x + mainWindow.width * 0.5f) - current.width * 0.5f;
        current.y = (mainWindow.y + mainWindow.height * 0.5f) - current.height * 0.5f - 100F;

        window.position = current;

        window.titleContent = new GUIContent("");
    }


    private void OnGUI()
    {
        if (windows.Count == 0) head = AddWindowInfo(globalID, 0, 0);

        mainFormSize.x = window.position.width - snbSize.x - 10;
        mainFormSize.y = snbSize.y = window.position.height;

        EditorGUILayout.BeginHorizontal();

        // Main
        EditorGUILayout.BeginVertical("box", GUILayout.Width(mainFormSize.x));
        DragBackground();
        DisplayMainForm();
        EditorGUILayout.EndVertical();

        // SNB
        EditorGUILayout.BeginVertical(GUILayout.Width(snbSize.x));
        DisplaySNB();
        EditorGUILayout.EndVertical();

        Space(10);

        EditorGUILayout.EndHorizontal();
    }


    private void DragBackground()
    {
        if (Event.current.keyCode == KeyCode.Space)
        {
            rootSpace = Vector2.zero;
            Repaint();
        }
        if (Event.current.type != EventType.MouseDrag) return;
        if (Event.current.button != 2) return;

        rootSpace.x += Event.current.delta.x;
        rootSpace.y += Event.current.delta.y;

        Repaint();
    }

    private void DisplayMainForm()
    {
        if (targetSequence == null)
        {
            AlignCenter(horizontal: false, () =>
                AlignCenter(horizontal: true, () =>
                    GUILayout.Label("No pattern sequence file was selected")
                )
            );
            return;
        }

        GUILayout.Label($"Selected sequence: \t{targetSequence.name}", EditorStyles.boldLabel);

        GUI.BeginGroup(new Rect(rootSpace.x, rootSpace.y, mainFormSize.x - rootSpace.x + 4, mainFormSize.y - rootSpace.y - 6));
        BeginWindows();

        Vector2 deltaCenter = new Vector2((position.width - windowRect.width) * 0.5f, position.height * 0.25f - windowRect.height * 0.5f);

        foreach (var pair in windows)
        {
            var windowInfo = pair.Value;
            windowInfo.windowRect = windowRect;
            windowInfo.margin = margin;

            var rect = windowInfo.Rect;
            rect.position += deltaCenter;
            GUI.Window(windowInfo.id, rect, windowInfo.func, string.Empty);

            if (windowInfo.next != null)
            {
                var childRect = windowInfo.next.Rect;
                childRect.position += deltaCenter;

                DrawNodeCurve(Color.white,
                    new Vector2(rect.x + rect.width, rect.y + rect.height * 0.5f),
                    new Vector2(childRect.x, childRect.y + childRect.height * 0.5f));
            }
            if (windowInfo.alternative != null)
            {
                var childRect = windowInfo.alternative.Rect;
                childRect.position += deltaCenter;

                DrawNodeCurve(Color.white,
                    new Vector2(rect.x + rect.width * 0.5f, rect.y + rect.height),
                    new Vector2(childRect.x, childRect.y + childRect.height * 0.5f),
                    new Vector2(rect.x + rect.width * 0.5f, childRect.y + childRect.height * 0.5f));
            }
        }

        Space();

        EndWindows();
        GUI.EndGroup();


        void DrawNodeCurve(Color c, Vector2 startPos, Vector2 endPos, params Vector2[] middles)
        {
            float distance = Mathf.Abs(startPos.y - endPos.y);

            var handleColor = Handles.color;
            Handles.color = c;

            if (middles.Length == 0)
            {
                Handles.DrawLine(startPos, endPos);
            }
            else
            {
                Handles.DrawLine(startPos, middles[0]);
                for (int i = 0; i < middles.Length - 1; ++i)
                {
                    Handles.DrawLine(middles[i], middles[i + 1]);
                }
                Handles.DrawLine(middles[middles.Length - 1], endPos);
            }

            Handles.color = handleColor;
        }
    }

    private void DisplaySNB()
    {
        Space(20);
        AlignCenter(horizontal: true, () => GUILayout.Label("Pattern Sequence Editor", EditorStyles.boldLabel));
        Space(20);

        Anomaly.Editor.EditorUtils.DrawHorizontalLine(Color.gray);

        Space(10);

        AlignCenter(horizontal: true, () =>
        {
            float prev = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 60;
            targetSequence = EditorGUILayout.ObjectField("Target", targetSequence, typeof(PatternSequence), false, GUILayout.Width(snbSize.x - 30)) as PatternSequence;
            EditorGUIUtility.labelWidth = prev;
        });

        Space();

        Anomaly.Editor.EditorUtils.DrawHorizontalLine(Color.gray);

        Space(10);

        AlignLeft(horizontal: true, () =>
        {
            Space(12);
            GUILayout.Label("Create new one", EditorStyles.boldLabel);
        });

        Space(2);

        AlignRight(horizontal: true, () =>
        {
            sequenceName = EditorGUILayout.TextField(sequenceName, GUILayout.Width(200));

            bool click = GUILayout.Button("Create", GUILayout.Width(60));
            if (click)
            {
                if (string.IsNullOrEmpty(sequenceName) || string.IsNullOrWhiteSpace(sequenceName)) sequenceName = "NewPatternSequence";

                string fileName = $"Assets/{sequenceName}.asset";

                if (AssetDatabase.LoadAssetAtPath<PatternSequence>(fileName) != null)
                {
                    EditorUtility.DisplayDialog("Warning!", $"Sequence {sequenceName} is already exist.\nYou cannot create sequence when sequence's name is same", "Confirm");
                    return;
                }

                var newAsset = ScriptableObject.CreateInstance<PatternSequence>();

                AssetDatabase.CreateAsset(newAsset, $"Assets/{sequenceName}.asset");
                AssetDatabase.SaveAssets();

                targetSequence = newAsset;
            }
            Space(15);
        });

        Space(10);
    }


    private WindowInfo AddWindowInfo(int id, int x, int y, GUI.WindowFunction func = null)
    {
        var rect = windowRect;
        rect.x = windowRect.x + (rect.width + margin.x) * x;
        rect.y = windowRect.y + (rect.height + margin.y) * y;

        var newInfo = new WindowInfo() { id = id, x = x, y = y, func = func, node = new PatternSequence.Node() };
        if (newInfo.func == null) newInfo.func = DefaultWindowFunc;

        windows.Add(newInfo.id, newInfo);
        return newInfo;
    }

    private void DefaultWindowFunc(int id)
    {
        var self = windows[id];

        GUI.Label(new Rect(0, -22, windowRect.width - 45F, windowRect.height), $"{self.id} / ({self.x}, {self.y})");

        if (self.next != null)
            GUI.Label(new Rect(0, 0, windowRect.width - 45F, windowRect.height), $"Next ({self.next.x}, {self.next.y})");
        if (self.alternative != null)
            GUI.Label(new Rect(0, 22, windowRect.width - 45F, windowRect.height), $"Alt ({self.alternative.x}, {self.alternative.y})");

        if (self.IsBranch) return;

        bool createNew = GUI.Button(new Rect(windowRect.width - 45F, 0, 45F, windowRect.height - 25F), "+");
        bool changeToBranch = GUI.Button(new Rect(windowRect.width - 45F, windowRect.height - 25F, 45F, 25F), "└");

        if (changeToBranch)
        {
            self.node = new PatternSequence.Branch();

            if (self.next == null)
            {
                self.next = AddWindowInfo(++globalID, self.x + 1, self.y);
                Arrange(head, self.next, true);
            }

            self.alternative = AddWindowInfo(++globalID, self.x + 1, self.y + 1);
            Arrange(head, self.alternative);

            return;
        }

        if (!createNew) return;

        var info = AddWindowInfo(++globalID, self.x + 1, self.y);

        if (self.next == null)
        {
            self.next = info;
            return;
        }

        Arrange(head, info, true);

        info.next = self.next;
        self.next = info;


        void Arrange(WindowInfo target, WindowInfo compare, bool justHorizontal = false)
        {
            if (target == null || compare == null) return;
            if (ReferenceEquals(target, compare)) return;

            if (target.y >= compare.y && !justHorizontal) target.y++;
            if (target.x >= compare.x && target.y == compare.y)
            {
                if (target.IsBranch) target.alternative.x++;
                target.x++;
            }

            Arrange(target.next, compare, justHorizontal);
            Arrange(target.alternative, compare, justHorizontal);
        }
    }


    #region Editor Helper
    private void Space(float pixels = -1)
    {
        if (pixels < 0F) GUILayout.FlexibleSpace();
        else GUILayout.Space(pixels);
    }

    private void AlignCenter(bool horizontal, System.Action callback)
    {
        if (horizontal) EditorGUILayout.BeginHorizontal();
        else EditorGUILayout.BeginVertical();

        GUILayout.FlexibleSpace();
        callback?.Invoke();
        GUILayout.FlexibleSpace();

        if (horizontal) EditorGUILayout.EndHorizontal();
        else EditorGUILayout.EndVertical();
    }
    private void AlignLeft(bool horizontal, System.Action callback)
    {
        if (horizontal) EditorGUILayout.BeginHorizontal();
        else EditorGUILayout.BeginVertical();

        callback?.Invoke();
        GUILayout.FlexibleSpace();

        if (horizontal) EditorGUILayout.EndHorizontal();
        else EditorGUILayout.EndVertical();
    }
    private void AlignRight(bool horizontal, System.Action callback)
    {
        if (horizontal) EditorGUILayout.BeginHorizontal();
        else EditorGUILayout.BeginVertical();

        GUILayout.FlexibleSpace();
        callback?.Invoke();

        if (horizontal) EditorGUILayout.EndHorizontal();
        else EditorGUILayout.EndVertical();
    }
    #endregion
}
