using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using CC.Core.Utilities.IO;
using System.IO;

namespace CC.SoundSystem.Editor
{
    public class NodeConnectorView : GraphView
    {
        public new class UxmlFactory : UxmlFactory<NodeConnectorView, UxmlTraits> { }

        /// <summary>
        /// Contructor
        /// </summary>
        public NodeConnectorView() : base()
        {
            Insert(0, new GridBackground());
            
            var stylesheet = AssetDatabase.LoadAssetAtPath<StyleSheet>((FileLocation.Directory + Path.AltDirectorySeparatorChar + "NodeConnectorView.uss").GetRelativeUnityPath());
            styleSheets.Add(stylesheet);

            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            this.StretchToParentSize();

            //tmp load for testing
            LoadDomain(Domain.GetAll()[0]);
        }

        /// <summary>
        /// Control the acceptable connections between Ports on the View
        /// </summary>
        /// <param name="startPort"></param>
        /// <param name="nodeAdapter"></param>
        /// <returns></returns>
        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            List<Port> compatiblePorts = new List<Port>();

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
        /// NOTE: Spacing out the spawns is not yet implemented
        /// </summary>
        /// <param name="domain">Source for nodes</param>
        public void LoadDomain(string domain)
        {
            GraphNode[] nodes = GraphNode.Load(Domain.GetNodes(domain));
            
            for(int i= 0; i< nodes.Length; i++)
            {
                AddElement(nodes[i]);
            }
            LoadConnections(nodes);
            
        }
        /// <summary>
        /// Will load the connections of the nodes wrapped by Graph node to the Graph View
        /// </summary>
        /// <param name="nodes">Nodes to Load port connects of</param>
        private void LoadConnections(GraphNode[] nodes)
        {
            //Load Connections
            for (int i = 0; i < nodes.Length; i++)
            {
                if (nodes[i].Node.IsRoot) continue;
                for (int j = 0; j < nodes.Length; j++)
                {
                    if (nodes[i].Node.Parent == nodes[j].Node)
                    {
                        Edge edge = nodes[i].ParentPort.ConnectTo(nodes[j].ChildrenPort);
                        AddElement(edge);
                        continue;
                    }
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
