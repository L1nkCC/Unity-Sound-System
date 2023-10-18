using UnityEngine;
using UnityEditor;
namespace CC.SoundSystem.Editor
{
    /// Author: L1nkCC
    /// Created: 10/12/2023
    /// Last Edited: 10/12/2023
    /// 
    /// <summary>
    /// Editor Window allowing for easy creation and deletion of domains
    /// </summary>
    public class DomainEditorWindow : EditorWindow
    {
        //this objects serialization
        SerializedObject m_serialized;

        //tabs
        int m_tabSelected = 0;
        static readonly string[] TAB_OPTIONS = {"Create Domain", "Delete Domain"};

        //Domain Creation information
        [SerializeField] string m_domainName;
        [SerializeField] string[] m_nodeNames;

        //Domain Deletion information
        [SerializeField] int m_domainToDeleteIndex;
        string DomainToDelete => Domain.GetAll()[m_domainToDeleteIndex];


        /// <summary>
        /// Allow for creation through tool bar
        /// </summary>
        [MenuItem("Window/CC/Sound System/Domain Editor")]
        public static void CreateWindow()
        {
            GetWindow<DomainEditorWindow>();
        }

        /// <summary>
        /// Generic Setup
        /// </summary>
        protected void OnEnable()
        {
            titleContent = new("Domain Editor");
            m_serialized = new SerializedObject(this);
        }

        /// <summary>
        /// Draw Tab and Corresponding Menu with tab
        /// </summary>
        public void OnGUI()
        {
            m_tabSelected = GUILayout.Toolbar(m_tabSelected, TAB_OPTIONS);

            switch (m_tabSelected) 
            {
                case (0):
                    GUILayout.Label("Create Sound Domain", Core.Utilities.GUI.Styles.Title);
                    GUILayout.Space(10);
                    DrawCreationWidget();
                break;
                case (1):
                    GUILayout.Label("Delete Sound Domain", Core.Utilities.GUI.Styles.Title);
                    GUILayout.Space(10);
                    DrawDeletionWidget();
                break;
            }
            m_serialized.ApplyModifiedProperties();
        }
        /// <summary>
        /// Draw the necessary tools to create a domain
        /// </summary>
        private void DrawCreationWidget()
        {
            EditorGUILayout.PropertyField(m_serialized.FindProperty("m_domainName"));
            Core.Utilities.GUI.Layout.DisplayArray(m_serialized.FindProperty("m_nodeNames"));

            if(GUILayout.Button("Create Domain")) { Domain.CreateDomain(m_domainName, m_nodeNames); ResetCreationValues(); ShowNotification(new("Domain Created Successfully!")); }
        }

        /// <summary>
        /// Draw the necessary tools to delete a domain
        /// </summary>
        private void DrawDeletionWidget()
        {
            GUILayout.Space(10);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Domain to Delete");
            m_domainToDeleteIndex = EditorGUILayout.Popup(m_domainToDeleteIndex, Domain.GetAll());
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Delete Domain")) { Domain.DeleteDomain(DomainToDelete); ShowNotification(new("Domain Deleted Successfully!")); }
        }

        /// <summary>
        /// Reset the values of the Creation Screen so user does not have to clean them out
        /// </summary>
        protected void ResetCreationValues()
        {
            m_serialized.FindProperty("m_domainName").stringValue = null;
            m_serialized.FindProperty("m_nodeNames").ClearArray();
        }

    }
}
