using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

namespace CC.SoundSystem
{
    /// Author: L1nkCC
    /// Created: 10/24/2023
    /// Last Edited: 10/25/2023
    /// 
    /// <summary>
    /// Slider to be used in CC.SoundSystem. Use this on a prefab for Mixer Menu. 
    /// Set Label, Slider, and expandButton or have the components attached to this gameobject or a child.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class VolumeSlider : MonoBehaviour
    {
        [Tooltip("Name of the sound type it is representing")][SerializeField] TextMeshProUGUI m_label;
        [Tooltip("Slider for manipulating Sound Mixing")][SerializeField] Slider m_volumeSlider;
        [Tooltip("Opens a menu with this soundtype as root")][SerializeField] Button m_expandButton;
        [SerializeField] public Node Node;
        [SerializeField] MixerMenu m_menu;

        /// <summary>
        /// Generic Constructor of a Slider in this menu
        /// </summary>
        /// <param name="node">Node the slider will be referencing</param>
        /// <returns>Created VolumeSlider</returns>
        public static VolumeSlider CreateInstance(VolumeSlider prefab, Node node, GameObject menu)
        {
            VolumeSlider slider = Instantiate(prefab, menu.transform);
            slider.ConnectNode(node);
            MixerMenu mixerMenu;
            if(menu.TryGetComponent<MixerMenu>(out mixerMenu))
            {
                 slider.m_menu = mixerMenu;
            }else if(menu.transform.parent.gameObject.TryGetComponent<MixerMenu>(out mixerMenu))
            {
                slider.m_menu = mixerMenu;
            }
            if (slider.m_menu != null && slider.m_expandButton != null) {slider.m_expandButton.onClick.AddListener(() => slider.m_menu.UpdateMenu(slider.Node)); }
            slider.name = slider.Node.name + " Volume Slider";
            return slider;
        }

        /// <summary>
        /// Reflect passed node in the slider
        /// </summary>
        /// <param name="node">Node to reflect information of</param>
        public void ConnectNode(Node node)
        {
            Node = node;
            m_label.text = Node.name;
            SetSliderValues();
            if (m_expandButton != null) m_expandButton.enabled = Node.Expandable;
        }

        /// <summary>
        /// Assure Components are attached to this gameobject or in children. Will Set null values
        /// </summary>
        private void AssureComponents()
        {
            m_volumeSlider = m_volumeSlider == null ? GetComponentInChildren<Slider>() : m_volumeSlider;
            m_label = m_label == null ? GetComponentInChildren<TextMeshProUGUI>() : m_label;
            m_expandButton = m_expandButton == null ? GetComponentInChildren<Button>() : m_expandButton;
        }

        /// <summary>
        /// Set Slider values to reflect Node
        /// </summary>
        private void SetSliderValues()
        {
            m_volumeSlider.value = Node == null ? Node.MAX_MULTIPLIER : Node.Multiplier;
            m_volumeSlider.maxValue = Node.MAX_MULTIPLIER;
            m_volumeSlider.minValue = Node.MIN_MULTIPLIER;
        }

        /// <summary>
        /// Assure Slider matches Node
        /// </summary>
        private void Awake()
        {
            SetSliderValues();
            m_volumeSlider.onValueChanged.AddListener((float multiplier) => Node.SetMultiplier(multiplier));
            if (m_menu != null && m_expandButton != null) { m_expandButton.onClick.AddListener(() => m_menu.UpdateMenu(Node)); }
        }

        /// <summary>
        /// Assure that the Slider is formatted
        /// </summary>
        private void OnValidate()
        {
            AssureComponents();

            if (m_volumeSlider == null) throw new MenuBuilderUtilities.MenuComponentException("No Slider for Volume Slider " + m_label.text + " to target. Please Attach a UnityEngine.UI.Slider to VolumeSlider");

            if (m_label == null) throw new MenuBuilderUtilities.MenuComponentException("No Label for Volume Slider " + m_label.text + " to Target. Please Attach a UnityEngine.UI.Slider to VolumeSlider");
        }
    }
}
