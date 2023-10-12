using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace CC.SoundSystem
{
    [CustomEditor(typeof(Node))]
    [CanEditMultipleObjects]
    public class NodeDrawer : UnityEditor.Editor
    {

        //Node properties
        SerializedProperty m_parent;
        SerializedProperty m_children;
        SerializedProperty m_clips;
        SerializedProperty m_multiplier;
        SerializedProperty m_muted;

        //Drawer status
        bool m_rootPathFolderStatus = false;
        bool m_childrenFolderStatus = false;


        private void OnEnable()
        {
            m_parent = serializedObject.FindProperty("m_parent");
            m_children = serializedObject.FindProperty("m_children");
            m_clips = serializedObject.FindProperty("m_clips");
            m_multiplier = serializedObject.FindProperty("m_multiplier");
            m_muted = serializedObject.FindProperty("m_muted");
        }

        public override void OnInspectorGUI()
        {
            DrawRelationships();
            DrawVolumeDetails();
            
            
            //Clip Details
            CC.Core.Utilities.GUI.Layout.DisplayArray(m_clips);

            DrawHelpBoxes();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawVolumeDetails()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginDisabledGroup(m_muted.boolValue);
            m_multiplier.floatValue = EditorGUILayout.Slider("Volume Multiplier", m_multiplier.floatValue, Node.MIN_MULTIPLIER, Node.MAX_MULTIPLIER);
            EditorGUI.EndDisabledGroup();
            GUILayout.Label("Mute", GUILayout.Width(40));
            m_muted.boolValue = EditorGUILayout.Toggle(m_muted.boolValue, GUILayout.Width(20));
            EditorGUILayout.EndHorizontal();
            EditorGUI.ProgressBar(GUILayoutUtility.GetRect(18, 18, "TextField"), (serializedObject.targetObject as Node).GetVolume(), "Volume");
        }


        private void DrawRelationships()
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(m_parent);
            EditorGUI.EndDisabledGroup();

            DrawRootPath();
            DrawChildren();
        }

        #region Relationship Display support
        private void DrawRootPath()
        {
            m_rootPathFolderStatus = EditorGUILayout.BeginFoldoutHeaderGroup(m_rootPathFolderStatus, "Root Path");
            if (m_rootPathFolderStatus)
            {
                EditorGUI.BeginDisabledGroup(true);
                List<Node> rootPath = (serializedObject.targetObject as Node).GetRootPath();

                if(rootPath.Count == 0) EditorGUILayout.HelpBox(new("This is a root node")); 
                for (int i = 0; i < rootPath.Count; i++)
                {
                    EditorGUILayout.LabelField(rootPath[i].name);
                }
                EditorGUI.EndDisabledGroup();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        private void DrawChildren()
        {
            m_childrenFolderStatus = EditorGUILayout.BeginFoldoutHeaderGroup(m_childrenFolderStatus, "Children");
            if (m_childrenFolderStatus)
            {
                EditorGUI.BeginDisabledGroup(true);

                if(m_children.arraySize == 0){ EditorGUILayout.HelpBox(new("This is a leaf node")); }

                for (int i = 0; i < m_children.arraySize; i++)
                {
                    EditorGUILayout.LabelField(m_children.GetArrayElementAtIndex(i).exposedReferenceValue.name);
                }
                EditorGUI.EndDisabledGroup();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }
        #endregion
        private void DrawHelpBoxes()
        {
            if(m_children.arraySize == 0 && m_clips.arraySize == 0)
            {
                EditorGUILayout.HelpBox(new("This node has no clips to play and is a leaf!"), MessageType.Warning);
            }
        }
    }

}
