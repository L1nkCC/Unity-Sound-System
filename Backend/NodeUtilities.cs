using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CC.SoundSystem
{
    /// Author: L1nkCC
    /// Created: 10/24/2023
    /// Last Edited: 10/24/2023
    /// 
    /// <summary>
    /// Supporting class to help orgainize Nodes
    /// </summary>
    public static class NodeUtilities
    {
        /// <summary>
        /// Do the passed action on each Node that has Root in it's root path.
        /// This will do the action as the nodes would be found a depth first search
        /// </summary>
        /// <param name="Roots">Root of which to get children from</param>
        /// <param name="action">Action to do on node. System.Action-Node: node to act on - int: level Node was found on (Increments when a leaf is reached) - int: depth node was found on (Increments on depth)</node></param>
        public static void ForEachChildDepthFirst(this Node root, System.Action<Node, int, int> action) { ForEachChildDepthFirst(new Node[] { root }, action); }
        /// <summary>
        /// Do the passed action on each Node that has One of roots in it's root path.
        /// This will do the action as the nodes would be found a depth first search
        /// </summary>
        /// <param name="Roots">Roots of which to get children from</param>
        /// <param name="action">Action to do on node. System.Action-Node: node to act on - int: level Node was found on (Increments when a leaf is reached) - int: depth node was found on (Increments on depth)</node></param>
        public static void ForEachChildDepthFirst(this IEnumerable<Node> Roots, System.Action<Node, int, int> action)
        {
            HashSet<Node> visited = new();
            int level = 0;
            foreach (Node root in Roots)
            {
                Stack<(Node, int)> stack = new(); //hold node and current depth for that node
                stack.Push((root, 0));
                while (stack.Count != 0)
                {
                    (Node currNode, int depth) = stack.Pop();
                    if (!visited.Contains(currNode))
                    {
                        visited.Add(currNode);
                        action(currNode, level, depth);
                        if (currNode.Children.Count == 0)
                        {
                            level++;
                        }
                        else
                        {
                            foreach (Node child in currNode.Children)
                            {
                                stack.Push((child, depth + 1));
                            }
                        }
                    }
                }
            }
        }
    }
}
