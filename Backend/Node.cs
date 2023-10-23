using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;


namespace CC.SoundSystem
{

    /// Author: L1nkCC
    /// Created: 10/13/2023
    /// Last Edited: 10/23/2023
    /// <summary>
    /// Holds information for each sound designation for easy manipulation by game developers
    /// </summary>
    [System.Serializable]
    public class Node : ScriptableObject
    {
        //bounds on m_multiplier's value
        public const float MAX_MULTIPLIER = 1;
        public const float MIN_MULTIPLIER = 0;

        //Node Relationships
        [SerializeField] private Node m_parent;
        [SerializeField] private List<Node> m_children = new();
        //Node Content
        [SerializeField] private List<AudioClip> m_clips = new();
        [SerializeField] private float m_multiplier = 1;//should stay between 0:1
        [SerializeField] private bool m_muted = false;

        //Public accessors
        public Node Parent => m_parent;
        public bool IsRoot => m_parent == null;

        public List<Node> Children { get { return m_children; } }

        /// <summary>
        /// Allow for easy creation of nodes as Scriptable Objects
        /// </summary>
        /// <param name="name">Name of the node</param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static Node CreateInstance(string name, Node parent = null)
        {
            Node node = ScriptableObject.CreateInstance<Node>();
            node.name = name;
            node.m_parent = parent;
            return node;
        }


        /// <summary>
        /// Get the top level Node from the tree this node is in. NOTE: Will run forever if Head node has a parent assigned
        /// </summary>
        /// <returns> Head node of tree.</returns>
        private Node GetRoot()
        {
            Node parent = this;
            while (!parent.IsRoot)
            {
                parent = parent.m_parent;
            }
            return parent;
        }
        /// <summary>
        /// Get all nodes that give a path back to the root
        /// </summary>
        /// <returns>Path of nodes to Root from this node</returns>
        public List<Node> GetRootPath()
        {
            List<Node> rootPath = new();
            Node parent = m_parent;
            while (parent != null)
            {
                rootPath.Add(parent);
                parent = parent.m_parent;
            }
            return rootPath;
        }


        /// <summary>
        /// Iteratatively works up the SoundType tree, multiplying the volume scale multipliers to get the effective Volume Scale for this Type.
        /// </summary>
        /// <returns>Volume scale associated with this SoundType.</returns>
        public float GetVolume()
        {
            float typeMultiplier = MAX_MULTIPLIER;
            Node parent = this;
            while (parent != null)
            {
                typeMultiplier *= parent.m_multiplier * (parent.m_muted ? 0 : 1);
                parent = parent.m_parent;
            }
            return typeMultiplier;
        }

        /// <summary>
        /// Set Multiplier Value to passed value if it is Between 0 and 1. Else no change
        /// </summary>
        /// <param name="multiplier">New Multiplier value</param>
        public void SetMultiplier(float multiplier)
        {
            m_multiplier = multiplier <= MAX_MULTIPLIER && multiplier >= MIN_MULTIPLIER ? multiplier : m_multiplier;
        }


        #region Child related Operations
        /// <summary>
        /// Recursively works down the tree and making a HashSet of all currently used names
        /// </summary>
        /// <param name="currentNamesFound"> [Optional] This is used for the recursive call. Contains all types found by the last call to this function.</param>
        /// <returns> All names used by the children</returns>
        private HashSet<string> GetChildNodeNames([Optional] HashSet<string> currentNamesFound)
        {
            if (currentNamesFound == null)
                currentNamesFound = new HashSet<string>();
            foreach (Node child in m_children)
            {
                currentNamesFound.Add(child.name);
                child.GetChildNodeNames(currentNamesFound);
            }
            return currentNamesFound;
        }


        /// <summary>
        /// Gets all node names that exist in the tree that this node is currently in.
        /// </summary>
        /// <returns> All node names in tree.</returns>
        private HashSet<string> GetTreeNodeNames()
        {
            return GetRoot().GetChildNodeNames();
        }

        /// <summary>
        /// Adds a child Node whilst checking to make sure that the type is unique to the tree
        /// </summary>
        /// <param name="childNode">node to become a child of the calling node</param>
        /// <returns>true if the node was added as a child, false if the node had a repeated name</returns>
        public bool AddChild(Node childNode)
        {
            HashSet<string> typesUsed = GetTreeNodeNames();
            if (!typesUsed.Contains(childNode.name))
            {
                typesUsed.Add(childNode.name);
                m_children.Add(childNode);
                childNode.m_parent = this;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Removes a child Node connection
        /// </summary>
        /// <param name="childNode">Child Node to be removed</param>
        /// <returns>true if the node was found and removed. False if not</returns>
        public bool RemoveChild(Node childNode)
        {
            if (m_children.Remove(childNode))
            {
                childNode.m_parent = null;
                return true;
            }
            return false;
        }


        /// <summary>
        /// Fills out tree children from a set of nodes that are linked to their parents but not their children. Adds Children to their parents m_children. O(nlogn)
        /// </summary>
        /// <param name="nodes"> The Set of Nodes that have defined parents.</param>
        /// <param name="parent"> [Optional] The parent that will act as the root to fill children down from.</param>
        /// <returns>Root of the tree.</returns>
        public static Node FillChildren(HashSet<Node> nodes, [Optional] Node parent)
        {
            foreach (Node node in nodes)
            {
                if (parent == null)
                    parent = node.GetRoot();
                if (Equals(node.m_parent.name, parent.name))
                {
                    parent.AddChild(node);
                    FillChildren(nodes, node);
                }
            }
            return parent;
        }
        #endregion
    }

}


