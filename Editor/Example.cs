using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
/*
namespace CC.SoundSystem
{

    public class SoundTreeEditorWindow : EditorWindow//, IHasCustomMenu
    {
        EditorNode<TestSounds>[] nodes = new EditorNode<TestSounds>[System.Enum.GetValues(typeof(TestSounds)).Length];

        [MenuItem("Window/CC/Sound Tree")]
        public static void ShowWindow()
        {
            var rootWindow =GetWindow<SoundTreeEditorWindow>("Sound System");
            EditorNode<TestSounds> lastNode = null;
            for(int i = 0; i < rootWindow.nodes.Length; i++)
            {
                rootWindow.nodes[i] = new EditorNode<TestSounds>("",(TestSounds)i, lastNode,new Rect(10 + (110*i), 10, 100, 100));
                lastNode = rootWindow.nodes[0];
            }
        }
        private void OnGUI()
        {
            for(int i = 0; i < nodes.Length; i++)
                if(!nodes[i].IsRoot) DrawNodeCurve((nodes[i].Parent as EditorNode<TestSounds>).m_rect, nodes[i].m_rect); // Here the curve is drawn under the windows

            BeginWindows();
            for(int i = 0; i< nodes.Length; i++)
            {
                nodes[i].m_rect = GUI.Window(i, nodes[i].m_rect, DrawNodeWindow, System.Enum.GetName(typeof(TestSounds),(TestSounds)i));
            }
            EndWindows();
        }
        void DrawNodeWindow(int id)
        {
            GUI.DragWindow();
        }
        void DrawNodeCurve(Rect start, Rect end)
        {
            Vector3 startPos = new Vector3(start.x + start.width, start.y + start.height / 2, 0);
            Vector3 endPos = new Vector3(end.x, end.y + end.height / 2, 0);
            Vector3 startTan = startPos + Vector3.right * 50;
            Vector3 endTan = endPos + Vector3.left * 50;
            Color shadowCol = new Color(0, 0, 0, 0.06f);
            for (int i = 0; i < 3; i++) // Draw a shadow
                Handles.DrawBezier(startPos, endPos, startTan, endTan, shadowCol, null, (i + 1) * 5);
            Handles.DrawBezier(startPos, endPos, startTan, endTan, Color.black, null, 1);
        }

    }
}*/
