using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace CC.SoundSystem
{
    [RequireComponent(typeof(RectTransform))]
    public class MixerMenu : GridLayoutGroup
    {
        [SerializeField] string m_selectedDomain;
        [SerializeField] public VolumeSlider m_volumeSliderPrefab;

        RectTransform GetRectTransform() { return this.transform as RectTransform; }
        protected override void OnValidate()
        {
            if(m_volumeSliderPrefab != null)
                cellSize = (m_volumeSliderPrefab.transform as RectTransform).rect.size;
        }

        public void UpdateMenu(Node root = null)
        {
            if (root == null)
                root = Editor.Domain.GetRoot(m_selectedDomain);
            ClearChildren();
            SpawnMenu(root);
        }

        private void SpawnMenu(Node root)
        {
            VolumeSlider rootSlider = ConstructSlider(root);
            rootSlider.gameObject.AddComponent<LayoutElement>().ignoreLayout = true;
            rootSlider.transform.position = Vector3.zero;
            for (int nodeIndex = 0; nodeIndex < root.Children.Count; nodeIndex++)
            {
                ConstructSlider(root.Children[nodeIndex]);
            }
        }
        private void ClearChildren()
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
        
        private VolumeSlider ConstructSlider(Node node)
        {
            VolumeSlider slider = Instantiate(m_volumeSliderPrefab, GetRectTransform());
            slider.ConnectNode(node);
            slider.name = slider.node.name + " Volume Slider";
            return slider;
        }
    }
}
