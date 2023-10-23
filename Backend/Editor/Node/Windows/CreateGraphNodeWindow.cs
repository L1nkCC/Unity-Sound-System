using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace CC.SoundSystem.Editor
{
    /// Author: L1nkCC
    /// Created: 10/23/2023
    /// Last Edited: 10/23/2023
    /// 
    /// <summary>
    /// A Window to create a GraphNode into an accepting Window
    /// </summary>
    public class CreateGraphNodeWindow : EditorWindow
    {
        //this serialized
        SerializedObject m_serialized;

        //Connector View that created this
        [SerializeField] IDomain m_connectorView;

        //User inputs
        [SerializeField] int m_selectedDomainIndex;
        [SerializeField] string m_newNodeName;
        [SerializeField] int m_selectedParentIndex;

        //Meta Selections
        string SelectedDomain => Domain.GetAll()[m_selectedDomainIndex];
        string[] NodeNamesInDomain => Domain.GetNodeNames(SelectedDomain);
        Node SelectedParent => Domain.GetNodes(SelectedDomain)[m_selectedParentIndex];

        /// <summary>
        /// Allow for the creation of this window with a reference with which to add the node
        /// </summary>
        /// <param name="view">The Reciver of the AddNewGraphNode call</param>
        /// <returns>The intialization of a CreateGraphNode window</returns>
        public static CreateGraphNodeWindow CreateInstance(IDomain view)
        {
            var window = GetWindow<CreateGraphNodeWindow>();
            window.m_connectorView = view;
            return window;
        }

        /// <summary>
        /// Set some intial values
        /// </summary>
        protected void OnEnable()
        {
            titleContent = new("Add Node");
            m_serialized = new SerializedObject(this);
        }


        /// <summary>
        /// Draw the Window
        /// </summary>
        public void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Select Domain:");
            m_selectedDomainIndex = EditorGUILayout.Popup(m_selectedDomainIndex, Domain.GetAll());
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Select Parent:");
            m_selectedParentIndex = EditorGUILayout.Popup(m_selectedParentIndex, NodeNamesInDomain);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Node Name:");
            m_newNodeName = EditorGUILayout.TextField(m_newNodeName);
            EditorGUILayout.EndHorizontal();

            m_serialized.ApplyModifiedProperties();
            EditorGUI.BeginDisabledGroup(!InputValidation());
            if(GUILayout.Button("Create Node")) 
            {
                m_connectorView.AddNewGraphNode(m_newNodeName, SelectedParent);
                m_serialized.ApplyModifiedProperties();
                this.Close();
            };
            EditorGUI.EndDisabledGroup();

        }

        /// <summary>
        /// Validate the inputs given by the user using Domain.ValidateNodeName. 
        /// Automatically draws a Helpbox with the error message thrown by that function.
        /// </summary>
        /// <returns>true if it is a valid name and false otherwise</returns>
        private bool InputValidation()
        {
            try{
                Domain.ValidateNodeName(SelectedDomain, m_newNodeName);
                return true;
            }catch(CC.Core.InputValidation.InputValidationException e)
            {
                EditorGUILayout.HelpBox(e.Message, MessageType.Error);
                return false;
            }
        }


    }
}
