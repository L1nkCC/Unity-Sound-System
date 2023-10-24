using UnityEditor;
using UnityEngine;
using System.Linq;
namespace CC.SoundSystem.Editor
{
    /// Author: L1nkCC
    /// Created: 10/20/2023
    /// Last Edited: 10/24/2023
    /// 
    /// <summary>
    /// Window for displaying Content inside a domain
    /// </summary>
    public class DomainContentWindow : EditorWindow, IIMGUIElement
    {
        //Serialized
        SerializedObject m_serialized;

        //Domain Selection information
        [SerializeField] int m_selectedDomainIndex;
        string SelectedDomain => Domain.GetAll()[m_selectedDomainIndex];
        Node[] Nodes => Domain.GetNodes(SelectedDomain);

        //Callbacks
        public System.Action<Node> OnNodeSelected = (Node node) => { };
        public System.Action<string> OnDomainSelectionChange = (string domainName) => { };
        public System.Action OnSort = () => { };

        /// <summary>
        /// Generic Setup
        /// </summary>
        protected void OnEnable()
        {
            titleContent = new("Domain Content");
            m_serialized = new SerializedObject(this);
        }

        /// <summary>
        /// Draw Element with title
        /// </summary>
        public void OnGUI()
        {
            GUILayout.Label("Domain Content", Core.Utilities.GUI.Styles.Title);
            GUILayout.Space(10);
            DrawContentWidget();
            m_serialized.ApplyModifiedProperties();
        }

        /// <summary>
        /// Draw Widget
        /// </summary>
        private void DrawContentWidget()
        {
            EditorGUILayout.BeginHorizontal();
            int newSelectedDomainIndex = EditorGUILayout.Popup(m_selectedDomainIndex, Domain.GetAll());
            if(newSelectedDomainIndex != m_selectedDomainIndex)
            {
                m_selectedDomainIndex = newSelectedDomainIndex;
                OnDomainSelectionChange(SelectedDomain);
            }
            if (GUILayout.Button("Sort", GUILayout.Width(50))) OnSort();
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button(new GUIContent("Update Enumerated Type"))) { DomainSaveUtilities.SaveAllDomains(); }// DomainEnumWriter.CreateDomainEnum(SelectedDomain); }

            GUILayout.Space(20);

            EditorGUILayout.LabelField("Nodes", EditorStyles.boldLabel);
            void DrawNode(Node node, int level, int depth)
            {
                EditorGUI.indentLevel = depth;
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(node.name, GUILayout.MinWidth(100));
                if (GUILayout.Button("Focus", GUILayout.MaxWidth(150))) OnNodeSelected(node);
                GUILayout.EndHorizontal();
                EditorGUI.indentLevel = 0;
            }

            NodeUtilities.ForEachChildDepthFirst(Nodes.Where(node => node.IsRoot),DrawNode);
        }

    }
}
