using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace CC.SoundSystem
{
    /// Author: L1nkCC
    /// Created: 10/24/2023
    /// Last Edited: 10/24/2023
    /// 
    /// <summary>
    /// Menu that Creates interactable UI for Sound Mixing
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class MixerMenu : UIBehaviour
    {
        [SerializeField] string m_selectedDomain = Domain.GetAll()[0];
        [SerializeField] public VolumeSlider m_volumeSliderPrefab;
        [SerializeField] public GameObject m_layout;

        private VolumeSlider m_rootSlider;

        /// <summary>
        /// Set the size of cells in the GridLayout Group
        /// </summary>
        public void SetCellSize()
        {
            GridLayoutGroup gridLayoutGroup;
            if (m_volumeSliderPrefab != null && m_layout != null && m_layout.TryGetComponent<GridLayoutGroup>(out gridLayoutGroup))
                gridLayoutGroup.cellSize = (m_volumeSliderPrefab.transform as RectTransform).rect.size;
        }

        /// <summary>
        /// Update the menu to show nodes that are children of root and in the specified domain
        /// NOTE: passing no domainName will result in no domain change.
        ///       passing no root will result in getting a root of the domain
        /// </summary>
        /// <param name="domainName">Domain to search</param>
        /// <param name="root">root to base Menu off</param>
        public void UpdateMenu(string domainName = null, Node root = null)
        {
            if(domainName != null)
                m_selectedDomain = domainName;
            if (root == null)
                root = Domain.GetRoot(m_selectedDomain);
            ClearMenu();
            CreateMenuHolder();
            SpawnMenu(root);

            LayoutRebuilder.ForceRebuildLayoutImmediate(this.transform as RectTransform);
        }

        /// <summary>
        /// For Button Press Use. This will load the previous Menu/ the menu for the parent of the currently inspected node
        /// </summary>
        public void LoadParentMenu()
        {
            if (!m_rootSlider.Node.IsRoot)
            {
                UpdateMenu(m_selectedDomain, m_rootSlider.Node.Parent);
            }
        }

        #region Menu Utilities
        /// <summary>
        /// Create a Gameobject of GridLayoutGroup type for organization
        /// </summary>
        private void CreateMenuHolder()
        {
            if(m_layout == null)
            {
                m_layout = new GameObject("Menu Holder", typeof(GridLayoutGroup));
                m_layout.transform.SetParent(this.transform,false);
                (m_layout.transform as RectTransform).Fill();
                (m_layout.transform as RectTransform).sizeDelta = (transform as RectTransform).sizeDelta - new Vector2(0, 2*(m_volumeSliderPrefab.transform as RectTransform).rect.height);
            }
        }
        /// <summary>
        /// Create sliders for Menu
        /// </summary>
        /// <param name="root">The root that we will see the children of</param>
        private void SpawnMenu(Node root)
        {
            ConstructRootSlider(root);
            for (int nodeIndex = 0; nodeIndex < root.Children.Count; nodeIndex++)
            {
                ConstructSlider(root.Children[nodeIndex]);
            }
        }
        /// <summary>
        /// Destroy all Gameobjects under menu
        /// </summary>
        private void ClearMenu()
        {
            System.Action Destroy = () => { };
            foreach(Transform child in transform)
            {
                Destroy += () =>
                {
                    if (this.transform != child)
                        DestroyImmediate(child.gameObject);
                };
            }
            Destroy();
        }
        #endregion

        #region Slider Constructors
        /// <summary>
        /// Root Constructor of a Slider heading this menu
        /// </summary>
        /// <param name="root">Root that the slider will be based on</param>
        /// <returns>A Volume Slider that is referencing root</returns>
        private VolumeSlider ConstructRootSlider(Node root)
        {
            m_rootSlider = Instantiate(m_volumeSliderPrefab, this.transform);
            m_rootSlider.ConnectNode(root);
            m_rootSlider.name = m_rootSlider.Node.name + " Root Volume Slider";
            LayoutElement rootElement = m_rootSlider.gameObject.AddComponent<LayoutElement>();
            rootElement.minHeight = (m_rootSlider.transform as RectTransform).rect.height;
            rootElement.preferredHeight = (m_rootSlider.transform as RectTransform).rect.height;
            rootElement.flexibleHeight = (m_rootSlider.transform as RectTransform).rect.height;
            rootElement.minWidth = (m_rootSlider.transform as RectTransform).rect.width;
            rootElement.preferredWidth = (m_rootSlider.transform as RectTransform).rect.width;
            rootElement.flexibleWidth = (m_rootSlider.transform as RectTransform).rect.width;
            m_rootSlider.transform.SetAsFirstSibling();
            return m_rootSlider;
        }

        /// <summary>
        /// Generic Constructor of a Slider in this menu
        /// </summary>
        /// <param name="node">Node the slider will be referencing</param>
        /// <returns>Created VolumeSlider</returns>
        private VolumeSlider ConstructSlider(Node node)
        {
            VolumeSlider slider = Instantiate(m_volumeSliderPrefab, m_layout.transform);
            slider.ConnectNode(node);
            slider.name = slider.Node.name + " Volume Slider";
            slider.AddListener(() => UpdateMenu(m_selectedDomain, node));
            return slider;
        }
        #endregion
    }
}
