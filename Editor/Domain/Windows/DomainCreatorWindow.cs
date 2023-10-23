using UnityEditor;
using UnityEngine;

namespace CC.SoundSystem.Editor
{
    /// Author: Connor Leslie
    /// Created: 10/20/2023
    /// Last Edited: 10/22/2023
    /// 
    /// <summary>
    /// Creation Window for SoundSystem Domains
    /// </summary>
    public class DomainCreatorWindow : EditorWindow, IIMGUIElement
    {
        //this serialized
        SerializedObject m_serialized;

        //Domain Creation information
        [SerializeField] string m_domainName;
        [SerializeField] string[] m_nodeNames;

        //Callback
        public System.Action<string> OnDomainCreation = (string domainName) => { };

        /// <summary>
        /// Generic Setup
        /// </summary>
        protected void OnEnable()
        {
            titleContent = new("Domain Creator");
            m_serialized = new SerializedObject(this);
        }

        /// <summary>
        /// Draw Element w/ Title
        /// </summary>
        public void OnGUI()
        {
            GUILayout.Label("Create Domain", Core.Utilities.GUI.Styles.Title);
            GUILayout.Space(10);
            DrawCreationWidget();
            m_serialized.ApplyModifiedProperties();
        }
        /// <summary>
        /// Draw Widget
        /// </summary>
        private void DrawCreationWidget()
        {
            EditorGUILayout.PropertyField(m_serialized.FindProperty("m_domainName"));
            Core.Utilities.GUI.Layout.DisplayArray(m_serialized.FindProperty("m_nodeNames"));

            if (GUILayout.Button("Create Domain")) { CreateDomain();  }
        }

        /// <summary>
        /// Handle Creation after button press
        /// </summary>
        private void CreateDomain()
        {
            Domain.CreateDomain(m_domainName, m_nodeNames); 
            OnDomainCreation(m_domainName); 
            ResetCreationValues(); 
            ShowNotification(new("Domain " + m_domainName + " Created Successfully!"));
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
