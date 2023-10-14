using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor;

namespace CC.SoundSystem
{
    public class GraphNode : UnityEditor.Experimental.GraphView.Node
    {
        Node _node;

        //Ports  --  Relational
        Port parentPort;
        Port childrenPort;


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="node"></param>
        public GraphNode(Node node) : base()
        {
            _node = node;
            title = _node.name;


            parentPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
            parentPort.portName = "Parent";
            inputContainer.Add(parentPort);
            
            childrenPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(bool));
            childrenPort.portName = "Children";
            outputContainer.Add(childrenPort);
            
            var nodeDetails = new IMGUIContainer(UnityEditor.Editor.CreateEditor(_node).OnInspectorGUI);
            extensionContainer.Add(nodeDetails);

        }


        /// <summary>
        /// Create a wrapper GraphNode for each CC.SoundSystem.Node passed in
        /// NOTE: Naming Conflict between CC.SoundSystem.Node and UnityEditor.Experimental.GraphView.Node. This is for CC.SoundSystem.Node
        /// </summary>
        /// <param name="nodes">Nodes to create GraphNodes for</param>
        /// <returns>Array containing all nodes wrapped in a GraphNode. Will be parallel to passed array.</returns>
        public static GraphNode[] Load(Node[] nodes)
        {
            GraphNode[] graphNodes = new GraphNode[nodes.Length];
            for(int i= 0; i < nodes.Length; i++)
            {
                graphNodes[i] = new GraphNode(nodes[i]);
            }
            return graphNodes;
        }

        /// <summary>
        /// Check to see if Node has any port connection to passed port. No port of a node connects to another
        /// NOTE: Only checks between relational port connections defined in GraphNode. Will not reflect any child class port connections
        /// </summary>
        /// <param name="parameterNode">Node to check port connections with</param>
        /// <returns>true if there is a connection or if passed node is the same as the node this was called from</returns>
        public bool AnyPortConnectionTo(GraphNode parameterNode)
        {
            if (this == parameterNode) return true;
            foreach(Edge edge in childrenPort.connections)
            {
                if (edge.input == parameterNode.parentPort) return true;
                if (edge.output == parameterNode.parentPort) return true;
                if (edge.input == parameterNode.childrenPort) return true;
                if (edge.output == parameterNode.childrenPort) return true;
            }
            foreach(Edge edge in parentPort.connections)
            {
                if (edge.input == parameterNode.parentPort) return true;
                if (edge.output == parameterNode.parentPort) return true;
                if (edge.input == parameterNode.childrenPort) return true;
                if (edge.output == parameterNode.childrenPort) return true;
            }
            return false;
        }
    }
}
