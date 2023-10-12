using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace CC.SoundSystem.Editor
{
    public class NodeEditorView : EditorWindow
    {
        SerializedObject m_serialized;

        //User inputed values
        [SerializeField] string m_domainName;
        [SerializeField] string[] m_nodeNames;

        //User GUI interface inputs
        [SerializeField] int m_domainSelectedIndex;
        string DomainSelected => DomainEditorHandler.GetDomains()[m_domainSelectedIndex];
        Node[] DomainSelectedNodes =>  DomainEditorHandler.GetNodes(DomainSelected);
        string[] DomainSelectedNodesNames 
        { 
            get{
                Node[] nodes = DomainSelectedNodes;
                string[] nodeNames = new string[nodes.Length];
                for (int i = 0; i < nodes.Length; i++)
                {
                    nodeNames[i] = nodes[i].name;
                }
                return nodeNames;
            }
        }



        //Select node from Domain
        [SerializeField] int m_nodeSelectedIndex;
        Node NodeSelected => DomainSelectedNodes[m_nodeSelectedIndex];

        public SerializedObject Serialized => m_serialized;

        [MenuItem("Window/CC/Sound System/Node Editor")]
        public static void CreateWindow()
        {
            GetWindow<NodeEditorView>();
        }

        protected void OnEnable()
        {
            titleContent = new("Edit Nodes");
            m_serialized = new SerializedObject(this);
        }

        protected void OnGUI()
        {
            GUILayout.Label("Edit Nodes", Core.Utilities.GUI.Styles.Title);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Select Domain");
            m_domainSelectedIndex = EditorGUILayout.Popup(m_domainSelectedIndex, DomainEditorHandler.GetDomains());
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Select Node");
            m_nodeSelectedIndex = EditorGUILayout.Popup(m_nodeSelectedIndex, DomainSelectedNodesNames);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);


            UnityEditor.Editor nodeEditor = UnityEditor.Editor.CreateEditor(NodeSelected);
            nodeEditor.OnInspectorGUI();

            m_serialized.ApplyModifiedProperties();
        }
    }
}
