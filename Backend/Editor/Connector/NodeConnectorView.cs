using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using CC.Core.Utilities.IO;
using System.IO;
using System.Linq;

namespace CC.SoundSystem.Editor
{
    /// Author: L1nkCC
    /// Created: 10/22/2023
    /// Last Edited: 10/22/2023
    /// 
    /// <summary>
    /// A GraphView optimized for GraphNode. This allows for easy manipulation of GraphNodes
    /// </summary>
    public class NodeConnectorView : GraphView, IDomain
    {
        public new class UxmlFactory : UxmlFactory<NodeConnectorView, UxmlTraits> { }

        //Callbacks
        public System.Action OnConnectionChange = ()=> { };
        public System.Action OnAddNode = () => { };

        //domain loaded currently
        private string m_selectedDomain;
        
        //Easy accessors
        protected IEnumerable<GraphNode> GraphNodes { 
            get 
            {
                return graphElements.Where(
                    (GraphElement ge) => { return ge.GetType() == typeof(GraphNode); }
                    ).Cast<GraphNode>();
            } 
        }
        GraphNode[] Roots => GraphNodes.Where(node => node.Node.IsRoot).ToArray();


        /// <summary>
        /// Contructor
        /// </summary>
        public NodeConnectorView() : base()
        {
            Insert(0, new GridBackground());
            
            var stylesheet = AssetDatabase.LoadAssetAtPath<StyleSheet>((FileLocation.Directory + Path.AltDirectorySeparatorChar + "NodeConnectorView.uss").GetRelativeUnityPath());
            styleSheets.Add(stylesheet);

            AddManipulators();

            this.StretchToParentSize();
        }


        /// <summary>
        /// Add necessary Manipulators to GraphView so it works as intended
        /// </summary>
        private void AddManipulators()
        {
            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            this.AddManipulator(CreateContextualMenuManipulator());
        }

        /// <summary>
        /// Create a basic Contextual Menu that only allow for Creation and Deletion of GraphNodes
        /// </summary>
        /// <returns> Manipulator to be added to View</returns>
        private IManipulator CreateContextualMenuManipulator()
        {
            ContextualMenuManipulator menuManipulator = new ContextualMenuManipulator
                (
                    menuEvent => 
                    {
                        GraphNode[] selectedGraphNodes = selection.Where(selected => selected is GraphNode).Select(selected => selected as GraphNode).ToArray();
                        string plural = selectedGraphNodes.Length > 1 ? "s" : "";
                        menuEvent.menu.ClearItems();

                        menuEvent.menu.AppendAction("Add Node", actionEvent => CreateGraphNodeWindow.CreateInstance(this)); 
                        if (selectedGraphNodes.Length > 0) 
                        {
                            menuEvent.menu.AppendAction("Delete Node" + plural, actionEvent => DeleteGraphNodes(selectedGraphNodes)); 
                        }
                    }
                );
            return menuManipulator;
        }


        /// <summary>
        /// Control the acceptable connections between Ports on the View
        /// </summary>
        /// <param name="startPort"></param>
        /// <param name="nodeAdapter"></param>
        /// <returns></returns>
        public override List<UnityEditor.Experimental.GraphView.Port> GetCompatiblePorts(UnityEditor.Experimental.GraphView.Port startPort, NodeAdapter nodeAdapter)
        {
            List<UnityEditor.Experimental.GraphView.Port> compatiblePorts = new List<UnityEditor.Experimental.GraphView.Port>();

            ports.ForEach(port => 
            {
                if (startPort.node != port.node && startPort.direction != port.direction && !(startPort.node as GraphNode).AnyPortConnectionTo(port.node as GraphNode))
                    {
                        compatiblePorts.Add(port);
                    }
            });
            return compatiblePorts;
        }

        /// <summary>
        /// Load GraphNodes for each Node in the domain onto the view 
        /// </summary>
        /// <param name="domain">Source for nodes</param>
        public void LoadDomain(string domain)
        {
            m_selectedDomain = domain;
            foreach(GraphElement ge in graphElements)
            {
                RemoveElement(ge);
            }
            GraphNode[] nodes = GraphNode.Load(Domain.GetNodes(domain));
            
            for(int i= 0; i< nodes.Length; i++)
            {
                AddElement(nodes[i]);
            }
            PlaceNodes();
        }

        /// <summary>
        /// Find a GraphNode on the View that wraps the Passed Node
        /// </summary>
        /// <param name="node">Node to find wrapped pairing for</param>
        /// <returns>The GraphNode on the view that wraps the node passed or null if there is no GraphNode that does that</returns>
        private GraphNode GetCorrespondingGraphNode(Node node)
        {
            foreach (GraphNode graphNode in GraphNodes)
            {
                if (graphNode.Node == node)
                {
                    return graphNode;
                }
            }
            return null;
        }
        /// <summary>
        /// Position the View Camera over the Graphnode that wraps the passed Node. Will do nothing if there is no corresponding graphnode to passed node
        /// </summary>
        /// <param name="node">Node to focus on</param>
        public void Focus(Node node)
        {
            GraphNode graphNode = GetCorrespondingGraphNode(node);
            if (graphNode == null) return;
            Focus(graphNode);
        }

        /// <summary>
        /// Position View Camera over graph Element
        /// </summary>
        /// <param name="graphElement">Element to place camera infront of</param>
        private void Focus(GraphElement graphElement)
        {
            Vector3 newPosition = new Vector3(-graphElement.GetPosition().x, -graphElement.GetPosition().y, graphElement.transform.position.z) +//put node at top left corner
                new Vector3(viewport.contentRect.width/2 - graphElement.contentRect.width/2, viewport.contentRect.height/2 - graphElement.contentRect.height/2); // position in center
            UpdateViewTransform(newPosition, graphElement.transform.scale);
            graphElement.Focus();
        }

        /// <summary>
        /// Organize Nodes and place them according to the order they would be found in a Breadth First Search
        /// </summary>
        public void PlaceNodes()
        {
            static void position(GraphNode node, int level, int depth)
            {
                float y = level * GraphNode.BASE_HEIGHT;
                float x = depth * GraphNode.BASE_WIDTH;
                node.SetPosition(x, y);
            }
            
            HashSet<GraphNode> visited = new();
            int level = 0;
            for(int iroot =0; iroot < Roots.Length; iroot++)
            {
                Stack<(GraphNode, int)> stack = new(); //hold graphnode and current depth for that node
                stack.Push((Roots[iroot], 0));
                while(stack.Count != 0)
                {
                    (GraphNode currNode, int depth) = stack.Pop();
                    if (!visited.Contains(currNode))
                    {
                        visited.Add(currNode);
                        position(currNode, level, depth);
                        GraphNode[] children = currNode.GetChildren();
                        if(children.Length == 0)
                        {
                            level++;
                        }
                        else
                        {
                            foreach(GraphNode child in children)
                            {
                                stack.Push((child, depth + 1));
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Delete graphnodes from the domain as well as the graph view
        /// </summary>
        /// <param name="graphNodes">GraphNodes to Delete</param>
        private void DeleteGraphNodes(IEnumerable<GraphNode> graphNodes)
        {
            foreach (GraphNode graphNode in graphNodes)
                DeleteGraphNode(graphNode);
        }
        /// <summary>
        /// Delete a graphnode from the domain as well as the graph view
        /// </summary>
        /// <param name="graphNode">GraphNode to delete</param>
        private void DeleteGraphNode(GraphNode graphNode)
        {
            base.RemoveElement(graphNode);
            Domain.DeleteNode(m_selectedDomain, graphNode.Node);
        }

        /// <summary>
        /// Remove Element from Graph. Will safely remove GraphNodes and maintain their relationships
        /// </summary>
        /// <param name="ge">Graph Element to remove</param>
        public new void RemoveElement(GraphElement ge)
        {
            if(ge.GetType() == typeof(GraphNode))
            {
                (ge as GraphNode).ClearCallbacks();
            }
            base.RemoveElement(ge);
        }
        /// <summary>
        /// Add Element to Graph. Will Load GraphNode with its connections and assign callbacks and call OnAddNode callback
        /// </summary>
        /// <param name="ge">Graph Element to add</param>
        public new void AddElement(GraphElement ge)
        {
            base.AddElement(ge);
            if(ge.GetType() == typeof(GraphNode))
            {
                GraphNode graphNode = ge as GraphNode;
                LoadConnections(graphNode);
                graphNode.ParentPort.OnConnect += (Port port) => { OnConnectionChange(); };
                graphNode.ParentPort.OnDisconnect += (Port port) => { OnConnectionChange(); };
                OnAddNode();
            }
        }
        /// <summary>
        /// Create a New GraphNode and Add it to the View. Will make a corresponding Node and add it to the domain
        /// </summary>
        /// <param name="nodeName">New node's name</param>
        /// <param name="parent">Parent of the node to be created</param>
        /// <returns>Newly created and added GraphNode</returns>
        public GraphNode AddNewGraphNode(string nodeName, Node parent = null)
        {
            GraphNode graphNode = new GraphNode(Node.CreateInstance(nodeName));
            Domain.AddNode(m_selectedDomain, graphNode.Node);
            AddElement(graphNode);
            return graphNode;
        }

        /// <summary>
        /// takes the passed Node and connects it on the GraphView according to the wrapped Nodes Relationships
        /// </summary>
        /// <param name="graphNode">Node to load connections of</param>
        private void LoadConnections(GraphNode graphNode)
        {
            foreach (GraphNode connectedGraphNode in GraphNodes)
            {
                if(graphNode.Node.Parent == connectedGraphNode.Node)
                {
                    Edge edge = graphNode.ParentPort.ConnectTo(connectedGraphNode.ChildrenPort);
                    AddElement(edge);
                }
                if (connectedGraphNode.Node.Parent == graphNode.Node)
                {
                    Edge edge = connectedGraphNode.ParentPort.ConnectTo(graphNode.ChildrenPort);
                    AddElement(edge);
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
