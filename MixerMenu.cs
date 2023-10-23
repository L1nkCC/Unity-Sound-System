using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CC.SoundSystem
{
    [RequireComponent(typeof(RectTransform))]
    public class MixerMenu : MonoBehaviour
    {
        [SerializeField] string m_selectedDomain;
        [SerializeField] VolumeSlider m_volumeSliderPrefab;
        [SerializeField] float m_verticalPadding = 20f;
        [SerializeField] float m_horizontalPadding = 20f;


        RectTransform GetRectTransform() { return this.transform as RectTransform; }

        private void Awake()
        {
            SpawnMenu();
        }

        private void SpawnMenu()
        {
            Node[] nodes = Editor.Domain.GetNodes(m_selectedDomain);
            float width = (m_volumeSliderPrefab.transform as RectTransform).rect.width;
            float height = (m_volumeSliderPrefab.transform as RectTransform).rect.height;
            int verticalCapacity = (int) (GetRectTransform().rect.height / (height + m_verticalPadding));
            int horizontalCapacity = (int) (GetRectTransform().rect.width / (width + m_horizontalPadding));
            
            if(horizontalCapacity*verticalCapacity < nodes.Length)
            {
                throw new MenuBuilderUtilities.MenuComponentException("Not enough Space for all Nodes in domain");
            }

            for(int row =0; row < horizontalCapacity; row++)
                for(int col = 0; col < verticalCapacity; col++)
                {
                    if (nodes.Length > col + (row * verticalCapacity))
                    {
                        Node nodeToConnect = nodes[col + (row * verticalCapacity)];
                        Vector3 position = new Vector3(row * (width + m_horizontalPadding), col * (height + m_verticalPadding));
                        Debug.Log(nodeToConnect.name + "  " + row * (width + m_horizontalPadding) + "    h: " + col * (height + m_verticalPadding));
                        VolumeSlider slider = Instantiate(m_volumeSliderPrefab, position, new Quaternion(), GetRectTransform());
                        slider.ConnectNode(nodeToConnect);
                    }
                }
        }
    }
}
