using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace CC.SoundSystem.Editor
{
    public class DomainEditorWindow : EditorWindow
    {
        SerializedObject m_serialized;

        //tabs
        int m_tabSelected = 0;
        static readonly string[] TAB_OPTIONS = {"Create Domain", "Delete Domain"};

        //User inputed values
        [SerializeField] string m_domainName;
        [SerializeField] string[] m_nodeNames;

        //User GUI interface inputs
        [SerializeField] int m_domainToDeleteIndex;

        string DomainToDelete => DomainEditorHandler.GetDomains()[m_domainToDeleteIndex];

        [MenuItem("Window/CC/Sound System/Domain Editor")]
        public static void CreateWindow()
        {
            GetWindow<DomainEditorWindow>();
        }

        protected void OnEnable()
        {
            titleContent = new("Domain Editor");
            m_serialized = new SerializedObject(this);
        }
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
        protected  void DrawCreationWidget()
        {
            EditorGUILayout.PropertyField(m_serialized.FindProperty("m_domainName"));
            Core.Utilities.GUI.Layout.DisplayArray(m_serialized.FindProperty("m_nodeNames"));

            if(GUILayout.Button("Create Domain")) { DomainEditorHandler.CreateNodes(m_domainName, m_nodeNames); ResetCreationValues(); ShowNotification(new("Domain Created Successfully!")); }
        }

        protected void DrawDeletionWidget()
        {
            GUILayout.Space(10);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Domain to Delete");
            m_domainToDeleteIndex = EditorGUILayout.Popup(m_domainToDeleteIndex, DomainEditorHandler.GetDomains());
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Delete Domain")) { DomainEditorHandler.DeleteDomain(DomainToDelete); ShowNotification(new("Domain Deleted Successfully!")); }
        }

        protected void ResetCreationValues()
        {
            m_serialized.FindProperty("m_domainName").stringValue = null;
            m_serialized.FindProperty("m_nodeNames").ClearArray();
        }

    }
}
