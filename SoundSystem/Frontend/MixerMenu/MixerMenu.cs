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

        [SerializeField] public VolumeSlider m_rootSlider;

        public string SelectedDomain => m_selectedDomain;


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
        /// </summary>
        /// <param name="domainName">Domain to search</param>
        /// <param name="root">root to base Menu off</param>
        public void UpdateMenu(Node root, string domainName = null)
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
        /// Update Menu from root of passed DomainName
        /// </summary>
        /// <param name="domainName">Domain to display</param>
        public void UpdateMenu(string domainName)
        {
            UpdateMenu(Domain.GetRoot(domainName), domainName);
        }

        /// <summary>
        /// For Button Press Use. This will load the previous Menu/ the menu for the parent of the currently inspected node
        /// </summary>
        public void LoadParentMenu()
        {
            if (!m_rootSlider.Node.IsRoot)
            {
                UpdateMenu(m_rootSlider.Node.Parent);
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
                VolumeSlider.CreateInstance(m_volumeSliderPrefab,root.Children[nodeIndex], m_layout);
            }
        }
        /// <summary>
        /// Destroy all Gameobjects under menu
        /// </summary>
        private void ClearMenu()
        {
            if (m_layout == null) return;
            System.Action Destroy = () => { };
            foreach(Transform child in m_layout.transform)
            {
                Destroy += () =>
                {
                    if (m_layout.transform != child)
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
            if (m_rootSlider == null)
                m_rootSlider = VolumeSlider.CreateInstance(m_volumeSliderPrefab, root, this.gameObject);
            else
                m_rootSlider.ConnectNode(root);
            m_rootSlider.name = m_rootSlider.Node.name + " Root Volume Slider";
            m_rootSlider.transform.SetAsFirstSibling();
            return m_rootSlider;
        }
#endregion
    }
}
