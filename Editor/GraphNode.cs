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
        public Node Node { get; protected set; }

        //Ports  --  Relational
        public Port ParentPort { get; protected set; }
        public Port ChildrenPort { get; protected set; }


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="node"></param>
        public GraphNode(Node node) : base()
        {
            Node = node;
            title = Node.name;


            ParentPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
            ParentPort.portName = "Parent";
            inputContainer.Add(ParentPort);
            
            ChildrenPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(bool));
            ChildrenPort.portName = "Children";
            outputContainer.Add(ChildrenPort);
            
            var nodeDetails = new IMGUIContainer(UnityEditor.Editor.CreateEditor(Node).OnInspectorGUI);
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
            //Create GraphNodes
            GraphNode[] graphNodes = new GraphNode[nodes.Length];
            for(int i= 0; i < nodes.Length; i++)
            {
                graphNodes[i] = new GraphNode(nodes[i]);
                //Place GraphNodes
                Rect pos = new Rect(graphNodes[i].GetPosition());
                pos.x += 400 * (i % 10);
                pos.y += 400 * (i / 10);
                graphNodes[i].SetPosition(pos);
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
            foreach(Edge edge in ChildrenPort.connections)
            {
                if (edge.input == parameterNode.ParentPort) return true;
                if (edge.output == parameterNode.ParentPort) return true;
                if (edge.input == parameterNode.ChildrenPort) return true;
                if (edge.output == parameterNode.ChildrenPort) return true;
            }
            foreach(Edge edge in ParentPort.connections)
            {
                if (edge.input == parameterNode.ParentPort) return true;
                if (edge.output == parameterNode.ParentPort) return true;
                if (edge.input == parameterNode.ChildrenPort) return true;
                if (edge.output == parameterNode.ChildrenPort) return true;
            }
            return false;
        }
    }
}
