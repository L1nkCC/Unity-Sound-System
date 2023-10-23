using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace CC.SoundSystem
{
    [RequireComponent(typeof(RectTransform))]
    public class VolumeSlider : MonoBehaviour
    {
        [SerializeField] Slider m_slider;
        [SerializeField] TextMeshProUGUI m_label;


        public Node node;

        private void Awake()
        {
            m_slider = GetComponentInChildren<Slider>();
            m_slider.maxValue = Node.MAX_MULTIPLIER;
            m_slider.minValue = Node.MIN_MULTIPLIER;
            m_slider.value = Node.MAX_MULTIPLIER;
            m_label = GetComponentInChildren<TextMeshProUGUI>();
        }

        public void ConnectNode(Node pnode)
        {
            node = pnode;
            m_slider.onValueChanged.AddListener(node.SetMultiplier);
            m_label.text = node.name;
        }

        private void OnValidate()
        {
            if (m_slider == null)
            m_slider = GetComponentInChildren<Slider>();
            if (m_slider == null) throw new MenuBuilderUtilities.MenuComponentException("No Slider for Volume Slider to target. Please Attach a UnityEngine.UI.Slider to VolumeSlider");

            if (m_label == null)
            m_label = GetComponentInChildren<TextMeshProUGUI>();
            if (m_label == null) throw new MenuBuilderUtilities.MenuComponentException("No Label for Volume Slider to Target. Please Attach a UnityEngine.UI.Slider to VolumeSlider");
        }
    }
}
