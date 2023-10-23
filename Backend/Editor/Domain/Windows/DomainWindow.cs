using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
namespace CC.SoundSystem.Editor
{
    /// Author: L1nkCC
    /// Created: 10/20/2023
    /// Last Edited: 10/22/2023
    /// 
    /// <summary>
    /// A tabbed menu for all Domain Commands
    /// </summary>
    public class DomainWindow : EditorWindow
    {
        //this serialized
        SerializedObject m_serialized;

        //Types of windows to be referenced. This is the name that will show in the tab selector
        private enum DomainWindowType
        {
            Creator = 0,
            Deletor,
            Content,
        }

        //tabs
        int m_tabSelected = (int) DomainWindowType.Content;
        static readonly string[] TAB_OPTIONS = System.Enum.GetNames(typeof(DomainWindowType));

        //windows organized for internal use
        IIMGUIElement[] windows;

        //Accessors for the windows
        public DomainCreatorWindow Creator => windows[(int)DomainWindowType.Creator] as DomainCreatorWindow;
        public DomainDeletorWindow Deletor => windows[(int)DomainWindowType.Deletor] as DomainDeletorWindow;
        public DomainContentWindow Content => windows[(int)DomainWindowType.Content] as DomainContentWindow;

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
