using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using CC.Core.Utilities.IO;
using System.IO;
using System.Linq;

namespace CC.SoundSystem.Editor
{
    public class NodeConnectorView : GraphView, IDomain
    {
        public System.Action OnConnectionChange = ()=> { };
        public System.Action OnAddNode = () => { };
        private string m_selectedDomain;
        protected IEnumerable<GraphNode> GraphNodes { 
            get 
            {
                return graphElements.Where(
                    (GraphElement ge) => { return ge.GetType() == typeof(GraphNode); }
                    ).Cast<GraphNode>();
            } 
        }


        public new class UxmlFactory : UxmlFactory<NodeConnectorView, UxmlTraits> { }

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


        private void AddManipulators()
        {
            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            this.AddManipulator(CreateAddGraphNodeContextualMenu());
        }

        private IManipulator CreateAddGraphNodeContextualMenu()
        {
            ContextualMenuManipulator menuManipulator = new ContextualMenuManipulator
                (
                    menuEvent => 
                    {
                        menuEvent.menu.AppendAction("Add Node", actionEvent => CreateGraphNodeWindow.CreateInstance(this)); 
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
            PlaceNodes(nodes);
        }

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
        public void Focus(Node node)
        {
            GraphNode graphNode = GetCorrespondingGraphNode(node);
            if (graphNode == null) return;
            Focus(graphNode);
        }
        public void Focus(GraphElement graphElement)
        {
            Vector3 newPosition = new Vector3(-graphElement.GetPosition().x, -graphElement.GetPosition().y, graphElement.transform.position.z) +//put node at top left corner
                new Vector3(viewport.contentRect.width/2 - graphElement.contentRect.width/2, viewport.contentRect.height/2 - graphElement.contentRect.height/2); // position in center
            UpdateViewTransform(newPosition, graphElement.transform.scale);
            graphElement.Focus();
        }


        public void PlaceNodes(GraphNode[] nodes)
        {
            static void position(GraphNode node, int level, int depth)
            {
                float y = level * GraphNode.BASE_HEIGHT;
                float x = depth * GraphNode.BASE_WIDTH;
                node.SetPosition(x, y);
            }
            GraphNode[] roots = nodes.Where(node => node.Node.IsRoot).ToArray();
            HashSet<GraphNode> visited = new();
            int level = 0;
            for(int iroot =0; iroot < roots.Length; iroot++)
            {
                Stack<(GraphNode, int)> stack = new(); //hold graphnode and current depth for that node
                stack.Push((roots[iroot], 0));
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

        public void DeleteGraphNode(GraphNode graphNode)
        {
            Domain.DeleteNode(m_selectedDomain, graphNode.Node);
            base.RemoveElement(graphNode);
        }

        public new void RemoveElement(GraphElement ge)
        {
            if(ge.GetType() == typeof(GraphNode))
            {
                (ge as GraphNode).ClearCallbacks();
            }
            base.RemoveElement(ge);
        }
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
