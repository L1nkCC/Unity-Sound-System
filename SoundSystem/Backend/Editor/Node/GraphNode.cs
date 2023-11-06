using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using System.Linq;
using System.Collections.Generic;

namespace CC.SoundSystem.Editor
{
    /// Author: L1nkCC
    /// Created: 10/18/2023
    /// Last Edited: 10/19/2023
    /// <summary>
    /// A wrapper class for CC.SoundSystem.Node so that it may be viewable and editable through a GraphView.
    /// NOTE: Naming Conflict between CC.SoundSystem.Node and UnityEditor.Experimental.GraphView.Node.
    /// </summary>
    public class GraphNode : UnityEditor.Experimental.GraphView.Node
    {
        //wrapped node
        public Node Node { get; protected set; }

        //Ports  --  Relational
        public Port ParentPort { get; protected set; }
        public Port ChildrenPort { get; protected set; }

        //Used For placing nodes
        public static int BASE_WIDTH = 600;
        public static int BASE_HEIGHT = 300;


        /// <summary>
        /// Constructor to wrap a Node for GraphView Representation
        /// </summary>
        /// <param name="node">Wrapped Node</param>
        public GraphNode(Node node) : base()
        {
            //Set up names and wrapped node
            Node = node;
            title = Node.name;
            name = Node.name;

            //Setup Parent Port
            ParentPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
            ParentPort.portName = "Parent";
            ParentPort.OnConnect += AddParent;
            ParentPort.OnDisconnect += RemoveParent;
            inputContainer.Add(ParentPort);
            
            //Setup Child Port
            ChildrenPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(bool));
            ChildrenPort.portName = "Children";
            outputContainer.Add(ChildrenPort);
            
            //Expose Details of node
            var nodeDetails = new IMGUIContainer(UnityEditor.Editor.CreateEditor(Node).OnInspectorGUI);
            extensionContainer.Add(nodeDetails);

            //Update details
            RefreshExpandedState();
        }

        /// <summary>
        /// Override of GraphView.Node.InstantiatePort to use CC.SoundSystem.Editor.Port rather than GraphView.Port. 
        /// A required extention for callbacks for OnConnect and OnDisconnect
        /// </summary>
        /// <param name="orientation">Which orientation the ports be on the Node</param>
        /// <param name="direction">Define if this Port should be an input or output. Helps decide which side the port should be on.</param>
        /// <param name="capacity">How many connections this port should handle. Either 1 or many.</param>
        /// <param name="type">Depericated</param>
        /// <returns>A new CC.SoundSystem.Editor.Port of the given specifications</returns>
        public new virtual Port InstantiatePort(Orientation orientation, Direction direction, Port.Capacity capacity, System.Type type)
        {
            return Port.Create<Edge>(orientation, direction, capacity, type);
        }


        /// <summary>
        /// Get the Parent Node as defined by port connections
        /// </summary>
        /// <returns>Connected Parent or null</returns>
        public GraphNode GetParent()
        {
            if(ParentPort.connections.Count() == 1)
            {
                return ParentPort.connections.ElementAt(0).output.node as GraphNode;
            }
            return null;
        }

        /// <summary>
        /// Get Connected Children Nodes as defined by port connections. 
        /// NOTE: May get off snyc with wrapper but shouldn't
        /// </summary>
        /// <returns>All Children Nodes</returns>
        public GraphNode[] GetChildren()
        {
            List<GraphNode> children = new();
            foreach(Edge edge in ChildrenPort.connections)
            {
                children.Add(edge.input.node as GraphNode);
            }
            return children.ToArray();
        }

        /// <summary>
        /// Given the parent port of the child, add the passed port's GraphNode's node as a child of the passed port's connection's GraphNodes's node.
        /// NOTE: This is only Available to be used on a port that has Capacity.Single.
        /// </summary>
        /// <param name="port">Port that is connected to the child after a connection to a parent</param>
        private static void AddParent(UnityEditor.Experimental.GraphView.Port port)
        {
            if (port.capacity != UnityEditor.Experimental.GraphView.Port.Capacity.Single) throw new System.ArgumentException("Port must have Single Capacity to add Children");
            Edge edge = port.connections.First();
            (edge.output.node as GraphNode).Node.AddChild((edge.input.node as GraphNode).Node);
            UnityEditor.EditorUtility.SetDirty((edge.output.node as GraphNode).Node);
            UnityEditor.EditorUtility.SetDirty((edge.input.node as GraphNode).Node);
        }
        /// <summary>
        /// Given the parent port of the child, remove the passed port's GraphNode's node as a child of the passed port's connection's GraphNodes's node.
        /// </summary>
        /// <param name="port">Port that is connected to the child after a disconnection to a parent</param>
        private static void RemoveParent(UnityEditor.Experimental.GraphView.Port port)
        {
            if(!(port.node as GraphNode).Node.Parent.RemoveChild((port.node as GraphNode).Node))
            {
                Debug.Log("Remove Failed");
            }
        }
        /// <summary>
        /// Clear the Callbacks (OnConnect and OnDisconnect). This is mainly for the purposes of removing a grphnode without updating the connections of the wrapped node.
        /// </summary>
        public void ClearCallbacks()
        {
            ParentPort.OnConnect = null;
            ParentPort.OnDisconnect = null;
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
            }
            return graphNodes;
        }

        /// <summary>
        /// Expose Position Editing 
        /// </summary>
        /// <param name="x">x position</param>
        /// <param name="y">y position</param>
        public void SetPosition(float x, float y)
        {
            Rect pos = new Rect(GetPosition());
            pos.x = x;
            pos.y = y;
            SetPosition(pos);
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
