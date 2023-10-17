using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using System.Linq;
using UnityEditor;
using CC.Core.Utilities.IO;
namespace CC.SoundSystem.Editor
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
            ParentPort.OnConnect += AddChild;
            ParentPort.OnDisconnect += RemoveChild;
            inputContainer.Add(ParentPort);
            
            ChildrenPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(bool));
            ChildrenPort.portName = "Children";
            outputContainer.Add(ChildrenPort);
            
            var nodeDetails = new IMGUIContainer(UnityEditor.Editor.CreateEditor(Node).OnInspectorGUI);
            extensionContainer.Add(nodeDetails);


            RefreshExpandedState();
        }

        public new virtual Port InstantiatePort(Orientation orientation, Direction direction, Port.Capacity capacity, System.Type type)
        {
            return Port.Create<Edge>(orientation, direction, capacity, type);
        }

        private static void AddChild(UnityEditor.Experimental.GraphView.Port port)
        {
            if (port.capacity != UnityEditor.Experimental.GraphView.Port.Capacity.Single) throw new System.ArgumentException("Port must have Single Capacity to add Children");
            Edge edge = port.connections.First();
            (edge.output.node as GraphNode).Node.AddChild((edge.input.node as GraphNode).Node);
        }
        private static void RemoveChild(UnityEditor.Experimental.GraphView.Port port)
        {
            if(!(port.node as GraphNode).Node.Parent.RemoveChild((port.node as GraphNode).Node))
            {
                Debug.Log("Remove Failed");
            }
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


        private class RelationalEdgeConnector : EdgeConnector<Edge>
        {
            public RelationalEdgeConnector() : base(new RelationalEdgeConnectorListener())
            {
            }
            private class RelationalEdgeConnectorListener : IEdgeConnectorListener
            {
                public void OnDrop(GraphView graphView, Edge edge)
                {
                    (edge.output.node as GraphNode).Node.AddChild((edge.input.node as GraphNode).Node);

                }

                public void OnDropOutsidePort(Edge edge, Vector2 position)
                {
                    Debug.Log("Edge 1: " + edge.output.portName);
                    Debug.Log("Edge 2: " + edge.input.portName);
                }
            }
        }
        /// <summary>
        /// Provides the compile time location of this File so that stylesheet may be found.
        /// </summary>
        /// Helpful Link : https://stackoverflow.com/questions/47841441/how-do-i-get-the-path-to-the-current-c-sharp-source-code-file
        private static class FileLocation
        {
            public static string Path => GetThisFilePath().Replace('\\', System.IO.Path.AltDirectorySeparatorChar);
            public static string Directory => System.IO.Path.GetDirectoryName(GetThisFilePath()).Replace('\\', System.IO.Path.AltDirectorySeparatorChar);
            private static string GetThisFilePath([System.Runtime.CompilerServices.CallerFilePath] string path = null)
            {
                return path;
            }
        }
    }
}
