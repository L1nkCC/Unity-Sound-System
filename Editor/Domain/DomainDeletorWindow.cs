using UnityEngine;
using UnityEditor;

namespace CC.SoundSystem.Editor
{
    /// Author: L1nkCC
    /// Created: 10/20/203
    /// Last Edited: 10/20/2023
    /// 
    /// <summary>
    /// Window for Deletion of a Domain
    /// </summary>
    public class DomainDeletorWindow : EditorWindow, IIMGUIElement
    {
        //this objects serialization
        SerializedObject m_serialized;

        //Domain Deletion information
        [SerializeField] int m_domainToDeleteIndex;
        string DomainToDelete => Domain.GetAll()[m_domainToDeleteIndex];


        /// <summary>
        /// Allow for creation through tool bar
        /// </summary>
        [MenuItem("Window/CC/Sound System/Domain/Deletor")]
        public static void CreateWindow()
        {
            GetWindow<DomainDeletorWindow>();
        }

        /// <summary>
        /// Generic Setup
        /// </summary>
        protected void OnEnable()
        {
            titleContent = new("Domain Deletor");
            m_serialized = new SerializedObject(this);
        }

        /// <summary>
        /// Draw Element w/ Title
        /// </summary>
        public void OnGUI()
        {
            GUILayout.Label("Delete Sound Domain", Core.Utilities.GUI.Styles.Title);
            GUILayout.Space(10);
            DrawDeletionWidget();
            m_serialized.ApplyModifiedProperties();
        }
        /// <summary>
        /// Draw Widget
        /// </summary>
        private void DrawDeletionWidget()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Domain to Delete");
            m_domainToDeleteIndex = EditorGUILayout.Popup(m_domainToDeleteIndex, Domain.GetAll());
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Delete Domain")) { Domain.DeleteDomain(DomainToDelete); ShowNotification(new("Domain Deleted Successfully!")); }
        }
    }
}
