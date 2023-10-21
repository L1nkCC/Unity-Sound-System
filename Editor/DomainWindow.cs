using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
namespace CC.SoundSystem.Editor
{
    /// Author: L1nkCC
    /// Created: 10/20/2023
    /// Last Edited: 10/20/2023
    /// 
    /// <summary>
    /// A tabbed menu for all Domain Commands
    /// </summary>
    public class DomainWindow : EditorWindow
    {
        //this serialized
        SerializedObject m_serialized;

        //tabs
        int m_tabSelected = 0;
        static readonly string[] TAB_OPTIONS = { "Create Domain", "Delete Domain", "Domain Content"};

        IIMGUIElement[] windows;

        /// <summary>
        /// Allow for creation through tool bar
        /// </summary>
        [MenuItem("Window/CC/Sound System/Domain Editor")]
        public static void CreateWindow()
        {
            GetWindow<DomainWindow>();
        }
        /// <summary>
        /// Generic Setup
        /// </summary>
        protected void OnEnable()
        {
            titleContent = new("Domain Editor");
            m_serialized = new SerializedObject(this);
            windows =  new IIMGUIElement[]{ ScriptableObject.CreateInstance<DomainCreatorWindow>(), ScriptableObject.CreateInstance<DomainDeletorWindow>(), ScriptableObject.CreateInstance<DomainContentWindow>() };
        }

        /// <summary>
        /// Draw Tab and Corresponding Menu with tab
        /// </summary>
        public void OnGUI()
        {
            m_tabSelected = GUILayout.Toolbar(m_tabSelected, TAB_OPTIONS);
            windows[m_tabSelected].OnGUI();
            m_serialized.ApplyModifiedProperties();
        }
    }
}
