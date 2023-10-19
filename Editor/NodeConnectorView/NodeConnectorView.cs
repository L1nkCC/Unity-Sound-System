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
    public class NodeConnectorView : GraphView
    {
        public System.Action OnConnectionChange = new(()=> { });
        private string domainName;
        protected IEnumerable<GraphNode> graphNodes { 
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

            domainName = Domain.GetAll()[0];

            //tmp load for testing
            LoadDomain(domainName);
        }


        private void AddManipulators()
        {
            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            this.AddManipulator(CreateGraphNodeContextualMenu());
        }

        private IManipulator CreateGraphNodeContextualMenu()
        {
            ContextualMenuManipulator menuManipulator = new ContextualMenuManipulator
                (
                    menuEvent => menuEvent.menu.AppendAction("Add Node", actionEvent => AddGraphNode("Nameless"))
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
        /// NOTE: Spacing out the spawns is not yet implemented. May clear connections and save the clearing of those connections
        /// </summary>
        /// <param name="domain">Source for nodes</param>
        public void LoadDomain(string domain)
        {
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


        public void PlaceNodes(GraphNode[] nodes)
        {
            void position(GraphNode node, int level, int depth)
            {
                Debug.Log("NODE: " + node.Node.name + "    LEVEL: " + level + "   DEPTH: " + depth);
                float y = level * GraphNode.BASE_HEIGHT;
                float x = depth * GraphNode.BASE_WIDTH;
                node.SetPosition(x, y);
            }
            GraphNode[] roots = nodes.Where(node => node.GetParent() == null).ToArray();
            HashSet<GraphNode> visited = new();
            int level = -1;
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


        public void DeleteElement(GraphNode ge)
        {
            string domainName = Domain.GetDomainOf(ge.Node);
            base.RemoveElement(ge);
            Domain.DeleteNode(domainName, ge.Node);
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
            if (typeof(GraphNode) != ge.GetType()) throw new System.ArgumentException("AddElement() only accepts GraphElements of type GraphNode. " + ge +" is not of type GraphNode");
            base.AddElement(ge);
            LoadConnections(ge as GraphNode);
        }

        public GraphNode AddGraphNode(string nodeName)
        {
            GraphNode graphNode = new GraphNode(Node.CreateInstance(nodeName));
            Domain.AddNode(domainName, graphNode.Node);
            AddElement(graphNode);
            return graphNode;
        }

        /// <summary>
        /// takes the passed Node and connects it on the GraphView according to the wrapped Nodes Relationships
        /// </summary>
        /// <param name="graphNode">Node to load connections of</param>
        private void LoadConnections(GraphNode graphNode)
        {
            foreach (GraphNode connectedGraphNode in graphNodes)
            {
                if(graphNode.Node.Parent == connectedGraphNode.Node)
                {
                    Edge edge = graphNode.ParentPort.ConnectTo(connectedGraphNode.ChildrenPort);
                    base.AddElement(edge);
                    OnConnectionChange();
                }
                if (connectedGraphNode.Node.Parent == graphNode.Node)
                {
                    Edge edge = connectedGraphNode.ParentPort.ConnectTo(graphNode.ChildrenPort);
                    base.AddElement(edge);
                    OnConnectionChange();
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
