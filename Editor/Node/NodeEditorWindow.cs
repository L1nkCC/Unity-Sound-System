using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace CC.SoundSystem.Editor
{
    /// Author: L1nkCC
    /// Created: 10/13/2023
    /// Last Edited: 10/13/2023
    /// <summary>
    /// Select a node from any domain and edit it as if it were in the inspector
    /// NOTE: mostly uncessary but helpful for debugging purposes or for getting details without knowledge of the file locations
    /// </summary>
    public class NodeEditorWindow : EditorWindow
    {
        //this object serialized
        SerializedObject m_serialized;

        //Selected Domain
        [SerializeField] int m_domainSelectedIndex;
        string DomainSelected => Domain.GetAll()[m_domainSelectedIndex];

        //Nodes in Selected Domain
        Node[] DomainSelectedNodes =>  Domain.GetNodes(DomainSelected);
        string[] DomainSelectedNodesNames => Domain.GetNodeNames(DomainSelected);



        //Selected node from selected Domain
        [SerializeField] int m_nodeSelectedIndex;
        Node NodeSelected => DomainSelectedNodes[m_nodeSelectedIndex];


        /// <summary>
        /// Creation through Unity Toolbar
        /// </summary>
        [MenuItem("Window/CC/Sound System/Node Editor")]
        public static void CreateWindow()
        {
            GetWindow<NodeEditorWindow>();
        }

        /// <summary>
        /// Intialization
        /// </summary>
        protected void OnEnable()
        {
            titleContent = new("Edit Nodes");
            m_serialized = new SerializedObject(this);
        }

        /// <summary>
        /// Draw Screen including Domain Selection, Node Selection, and Node Editor
        /// </summary>
        protected void OnGUI()
        {
            GUILayout.Label("Edit Nodes", Core.Utilities.GUI.Styles.Title);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Select Domain");
            m_domainSelectedIndex = EditorGUILayout.Popup(m_domainSelectedIndex, Domain.GetAll());
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
