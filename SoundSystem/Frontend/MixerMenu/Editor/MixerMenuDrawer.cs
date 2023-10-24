using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace CC.SoundSystem.Editor
{
    /// Author: L1nkCC
    /// Created: 10/24/2023
    /// Last Edited: 10/24/203
    /// 
    /// <summary>
    /// Editor For Mixer Menu
    /// </summary>
    [CustomEditor(typeof(MixerMenu))]
    [CanEditMultipleObjects]
    public class MixerMenuEditor : UnityEditor.Editor
    {

        //status
        int m_selectedDomainIndex;

        //Callback
        System.Action OnSelectedDomainChange = () => { };

        //Easy Accessor
        string SelectedDomain => Domain.GetAll()[m_selectedDomainIndex];

        /// <summary>
        /// Create Visual Element for Editor Display
        /// </summary>
        /// <returns>Editor Display in a Visual Element</returns>
        public override VisualElement CreateInspectorGUI()
        {
            VisualElement rootElement = new VisualElement();
            rootElement.Add(new IMGUIContainer(Draw));
            OnSelectedDomainChange += () => DomainNoticeHandler.UpdateDomainNotices(rootElement,SelectedDomain);
            return rootElement;
        }

        /// <summary>
        /// Draw the Editor Display
        /// </summary>
        public void Draw()
        {
            GUILayout.BeginHorizontal();
            int tmpIndex = EditorGUILayout.Popup(new GUIContent("Target Domain", "Domain the menu will be referencing"),m_selectedDomainIndex, Domain.GetAll());
            if (tmpIndex != m_selectedDomainIndex)
            {
                m_selectedDomainIndex = tmpIndex;
                serializedObject.ApplyModifiedProperties();
                (target as MixerMenu).UpdateMenu(SelectedDomain);
                OnSelectedDomainChange();
            }
            if (GUILayout.Button(new GUIContent("Refresh Domain", "Reset Menu and reload all children of Menu"))) (target as MixerMenu).UpdateMenu();
            GUILayout.EndHorizontal();
            (target as MixerMenu).m_volumeSliderPrefab = EditorGUILayout.ObjectField(new GUIContent("Volume Slider", "Slider Prefab to use in Menu Construction"),(target as MixerMenu).m_volumeSliderPrefab, typeof(VolumeSlider), false) as VolumeSlider;
            (target as MixerMenu).m_layout = EditorGUILayout.ObjectField(new GUIContent("Menu Holder", "Contains all sliders that are a child of Root"),(target as MixerMenu).m_layout, typeof(GameObject), true) as GameObject;
            (target as MixerMenu).SetCellSize();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
