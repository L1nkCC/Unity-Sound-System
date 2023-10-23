using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEditor.UI;
using UnityEngine.UIElements;

namespace CC.SoundSystem.Editor
{
    [CustomEditor(typeof(MixerMenu))]
    [CanEditMultipleObjects]
    public class MixerMenuEditor : GridLayoutGroupEditor
    {
        SerializedProperty m_selectedDomain;

        int m_selectedDomainIndex;
        System.Action OnSelectedDomainChange = () => { };

        protected override void OnEnable()
        {
            base.OnEnable();
            m_selectedDomain = serializedObject.FindProperty("m_selectedDomain");
        }

        public override VisualElement CreateInspectorGUI()
        {
            VisualElement rootElement = new VisualElement();
            rootElement.Add(new IMGUIContainer(Draw));
            OnSelectedDomainChange += () => DomainNoticeHandler.UpdateDomainNotices(rootElement,m_selectedDomain.stringValue);
            return rootElement;
        }

        public void Draw()
        {
            GUILayout.BeginHorizontal();
            int tmpIndex = EditorGUILayout.Popup(m_selectedDomainIndex, Domain.GetAll());
            if (tmpIndex != m_selectedDomainIndex)
            {
                m_selectedDomainIndex = tmpIndex;
                m_selectedDomain.stringValue = Domain.GetAll()[tmpIndex];
                serializedObject.ApplyModifiedProperties();
                (target as MixerMenu).UpdateMenu();
                OnSelectedDomainChange();
            }
            if (GUILayout.Button("Refresh Domain")) (target as MixerMenu).UpdateMenu();
            GUILayout.EndHorizontal();
            (target as MixerMenu).m_volumeSliderPrefab = EditorGUILayout.ObjectField((target as MixerMenu).m_volumeSliderPrefab, typeof(VolumeSlider), false) as VolumeSlider;
            base.OnInspectorGUI();
        }
    }
}
