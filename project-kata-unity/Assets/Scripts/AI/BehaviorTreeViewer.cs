#if UNITY_EDITOR
namespace UnityBehaviorTree
{
    using UnityEditor;
    using UnityEngine;
    using System.Collections.Generic;
    using System.Linq;

    public class BehaviorTreeViewer : EditorWindow
    {
        private class WindowInfo
        {
            public int id;
            public Action action;
            public Rect rect;
            public GUI.WindowFunction func;

            public WindowInfo[] connected;
        }

        [MenuItem("Tools/BehaviorTree Viewer")]
        private static void ShowWindow()
        {
            var window = EditorWindow.CreateInstance<BehaviorTreeViewer>();
            window.titleContent = new GUIContent("Behavior Tree Viewer");
            window.Show();
        }


        private BehaviorTree targetBehaviorTree;
        public BehaviorTree TargetBehaviorTree
        {
            set
            {
                targetBehaviorTree = value;
                AddSequence(targetBehaviorTree.Sequence);
                AlignCenter(windows[targetBehaviorTree.Sequence.GUID]);
            }
        }

        private Vector2 rootSpace = Vector2.one * -50000;
        private Vector2 dragRange = Vector2.zero;

        private Rect windowRect = new Rect(10, 10, 150, 40);
        private (int x, int y) depth = (0, 0);
        private (int x, int y) margin = (10, 50);
        private int windowID = 2;

        private Dictionary<string, WindowInfo> windows = new Dictionary<string, WindowInfo>();


        private void AlignCenter(WindowInfo info)
        {
            if (info.connected == null) return;

            float center = 0F;
            foreach (var child in info.connected)
            {
                AlignCenter(child);
                center += child.rect.x;
            }
            center /= info.connected.Length;

            info.rect.x = center;
        }


        private void OnGUI()
        {
            if (targetBehaviorTree == null) return;

            DragBackground();
            ShowBehaviorTree();
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

        private void ShowBehaviorTree()
        {
            GUI.BeginGroup(new Rect(rootSpace.x, rootSpace.y, 100000, 100000));

            BeginWindows();

            Vector2 deltaCenter = new Vector2((position.width - windowRect.width) * 0.5f, position.height * 0.25f - windowRect.height * 0.5f);
            deltaCenter.x -= windows[targetBehaviorTree.Sequence.GUID].rect.x;
            deltaCenter.y -= windows[targetBehaviorTree.Sequence.GUID].rect.y;

            foreach (var pair in windows)
            {
                var rect = pair.Value.rect;
                rect.position += deltaCenter;
                rect = GUI.Window(pair.Value.id, rect, pair.Value.func, pair.Value.action.Name);


                for (int i = 0; pair.Value.connected != null && i < pair.Value.connected.Length; ++i)
                {
                    var childRect = pair.Value.connected[i].rect;
                    childRect.position += deltaCenter;
                    DrawNodeCurve(rect, childRect, Color.white);
                }

                rect.position -= deltaCenter;
                windows[pair.Key].rect = rect;
            }

            HighlightCallStack();

            EndWindows();

            GUI.EndGroup();
        }

        private void HighlightCallStack()
        {
            if (targetBehaviorTree == null) return;
            if (targetBehaviorTree.CallStack.Count == 0) return;

            Vector2 deltaCenter = new Vector2((position.width - windowRect.width) * 0.5f, position.height * 0.25f - windowRect.height * 0.5f);
            deltaCenter.x -= windows[targetBehaviorTree.Sequence.GUID].rect.x;
            deltaCenter.y -= windows[targetBehaviorTree.Sequence.GUID].rect.y;

            foreach (var stack in targetBehaviorTree.CallStack)
            {
                var connected = windows[stack.GUID].connected;
                if (connected == null) continue;
                for (int i = 0; i < connected.Length; ++i)
                {
                    if (!targetBehaviorTree.CallStack.Contains(connected[i].action)) continue;

                    Rect r1 = windows[stack.GUID].rect, r2 = connected[i].rect;
                    r1.position += deltaCenter;
                    r2.position += deltaCenter;
                    DrawNodeCurve(r1, r2, Color.green);
                }
            }

            Repaint();
        }

        private void DrawNodeCurve(Rect start, Rect end, Color c)
        {
            Vector3 startPos = new Vector3(start.x + start.width * 0.5f, start.y + start.height, 0);
            Vector3 endPos = new Vector3(end.x + end.width * 0.5f, end.y, 0);

            float distance = Mathf.Abs(startPos.y - endPos.y);

            Vector3 startMiddlePos = startPos + Vector3.up * distance * 0.5f;
            Vector3 endMiddlePos = endPos + Vector3.down * distance * 0.5f;

            var handleColor = Handles.color;
            Handles.color = c;

            Handles.DrawLine(startPos, startMiddlePos);
            Handles.DrawLine(endPos, endMiddlePos);
            Handles.DrawLine(startMiddlePos, endMiddlePos);

            Handles.color = handleColor;
        }


        private WindowInfo AddByActionType(Action action)
        {
            if (action is Sequence) return AddSequence(action);
            else if (action is Selector) return AddSelector(action);
            else if (action is SelectorRandom) return AddSelectorRandom(action);
            else if (action is Parallel) return AddParallel(action);
            else if (action is Inverter) return AddInverter(action);
            else if (action is If) return AddIf(action);
            else return AddAction(action);
        }

        private WindowInfo AddSequence(Action action)
        {
            int seqID = windowID++;
            var seqInfo = AddWindowInfo(seqID, depth.x, depth.y, action);

            depth.y++;

            var seq = action as Sequence;

            windows[seqInfo.action.GUID].connected = new WindowInfo[seq.Children.Count];

            for (int i = 0; i < seq.Children.Count; ++i)
            {
                var childRect = AddByActionType(seq.Children[i]);
                windows[seqInfo.action.GUID].connected[i] = childRect;
                depth.x++;
            }

            depth.x--;
            depth.y--;

            return seqInfo;
        }

        private WindowInfo AddSelector(Action action)
        {
            int selectID = windowID++;
            var selectInfo = AddWindowInfo(selectID, depth.x, depth.y, action);

            depth.y++;

            var selector = action as Selector;

            windows[selectInfo.action.GUID].connected = new WindowInfo[selector.Children.Count];

            for (int i = 0; i < selector.Children.Count; ++i)
            {
                var childRect = AddByActionType(selector.Children[i]);
                windows[selectInfo.action.GUID].connected[i] = childRect;
                depth.x++;
            }

            depth.x--;
            depth.y--;

            return selectInfo;
        }
        private WindowInfo AddSelectorRandom(Action action)
        {
            int selectID = windowID++;
            var selectInfo = AddWindowInfo(selectID, depth.x, depth.y, action);

            depth.y++;

            var selector = action as SelectorRandom;

            windows[selectInfo.action.GUID].connected = new WindowInfo[selector.Children.Count];

            for (int i = 0; i < selector.Children.Count; ++i)
            {
                var childRect = AddByActionType(selector.Children[i]);
                windows[selectInfo.action.GUID].connected[i] = childRect;
                depth.x++;
            }

            depth.x--;
            depth.y--;

            return selectInfo;
        }

        private WindowInfo AddParallel(Action action)
        {
            int parallelID = windowID++;
            var parallelInfo = AddWindowInfo(parallelID, depth.x, depth.y, action);

            depth.y++;

            var parallel = action as Parallel;

            windows[parallelInfo.action.GUID].connected = new WindowInfo[parallel.Children.Count];

            for (int i = 0; i < parallel.Children.Count; ++i)
            {
                var childRect = AddByActionType(parallel.Children[i]);
                windows[parallelInfo.action.GUID].connected[i] = childRect;
                depth.x++;
            }

            depth.x--;
            depth.y--;

            return parallelInfo;
        }

        private WindowInfo AddInverter(Action action)
        {
            int invID = windowID++;
            var invInfo = AddWindowInfo(invID, depth.x, depth.y, action);

            depth.y++;

            var inverter = action as Inverter;

            var childRect = AddByActionType(inverter.Child);
            windows[invInfo.action.GUID].connected = new WindowInfo[] { childRect };
            depth.x++;

            depth.y--;

            return invInfo;
        }

        private WindowInfo AddIf(Action action)
        {
            var _if = action as If;

            int ifID = windowID++;
            var ifInfo = AddWindowInfo(ifID, depth.x, depth.y, action, id => GUILabelCenter(new GUIContent(_if.Condition.Name, _if.Condition.Description)));

            depth.y++;

            var childRect = AddByActionType(_if.Child);
            windows[ifInfo.action.GUID].connected = new WindowInfo[] { childRect };

            depth.y--;

            return ifInfo;
        }

        private WindowInfo AddAction(Action action)
        {
            return AddWindowInfo(windowID++, depth.x, depth.y, action);
        }


        private WindowInfo AddWindowInfo(int id, int x, int y, Action action, GUI.WindowFunction func = null)
        {
            // Use this if default window is draggable.
            //if (func == null) func = id => GUI.DragWindow();
            if (func == null) func = id => GUILabelCenter(new GUIContent("---------------", action.Description));

            var rect = windowRect;
            rect.x = windowRect.x + (rect.width + margin.x) * x;
            rect.y = windowRect.y + (rect.height + margin.y) * y;

            windows.Add(action.GUID, new WindowInfo() { id = id, action = action, rect = rect, func = func });
            return windows[action.GUID];
        }

        private void GUILabelCenter(GUIContent content)
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(content);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
    }
}
#endif