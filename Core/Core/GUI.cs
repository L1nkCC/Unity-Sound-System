using UnityEngine;
using UnityEditor;
namespace CC.Core.Utilities.GUI
{

    /// Author: LinkCC
    /// Created: 10/9/23
    /// Last Edited: 10/9/23
    /// <summary>
    /// Hold Information Concerning Styling in displays
    /// </summary>
    public static class Styles
    {

        /// <summary>
        /// Style used for Titles
        /// </summary>
        public static GUIStyle Title 
        {
            get
            {
                GUIStyle _title = new();
                _title.fontSize = 30;
                _title.alignment = TextAnchor.MiddleCenter;
                _title.fontStyle = FontStyle.Bold;
                _title.normal.textColor = Color.grey;
                return _title;
            }
        }
    }

    /// Author: LinkCC
    /// Created: 10/11/2023
    /// Last Edited: 10/22/23
    /// <summary>
    /// Support for different ways to display Properties with Unity Editor
    /// </summary>
    public static class Layout
    {
        public static void DisplayArray(SerializedProperty array)
        {
            if (!array.isArray) throw new System.ArgumentException("Propery must be an array");
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(array.displayName, GUILayout.MinWidth(100));
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(array.FindPropertyRelative("Array.size"), GUIContent.none, GUILayout.MaxWidth(70));
            EditorGUI.EndDisabledGroup();
            if (GUILayout.Button("+", GUILayout.Width(30))) array.arraySize++;
            EditorGUI.BeginDisabledGroup(array.arraySize < 1);
            if (GUILayout.Button("-", GUILayout.Width(30))) array.arraySize--;
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndHorizontal();
            for(int i =0; i < array.arraySize; i++)
            {
                EditorGUILayout.PropertyField(array.GetArrayElementAtIndex(i), GUIContent.none);
            }
        }
    }
}
