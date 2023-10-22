using UnityEditor;
using System.IO;
using System.Linq;
using System;
using CC.Core.Utilities.IO;
using CC.Core.InputValidation;

namespace CC.SoundSystem.Editor
{

    /// Author: L1nkCC
    /// Created: 10/12/2023
    /// Last Edited: 10/21/2023
    /// 
    /// <summary>
    /// Class for accessing nodes by their folder or 'Domain' as this will be how the node recognize which tree they are a part of
    /// </summary>
    public static class Domain
    {
        static readonly string path = Directory.GetParent(Directory.GetParent(FileLocation.Directory).FullName).FullName + Path.AltDirectorySeparatorChar + "Domains" + Path.AltDirectorySeparatorChar;
        const string EXT = ".asset";


        #region Getters
        /// <summary>
        /// Returns all the names of domains
        /// </summary>
        /// <returns>All Domain names</returns>
        public static string[] GetAll()
        {
            string[] paths = Directory.GetDirectories(path);
            for (int i = 0; i < paths.Length; i++)
                paths[i] = Path.GetRelativePath(path,paths[i]);
            return paths;
        }
        /// <summary>
        /// Returns all the nodes of a particular domain
        /// </summary>
        /// <param name="domainName">Domain to source</param>
        /// <returns>All nodes in given domain</returns>
        public static Node[] GetNodes(string domainName)
        {
            string[] nodePaths =  Directory.GetFiles(path + domainName,"*"+EXT, SearchOption.TopDirectoryOnly);
            Node[] nodes = new Node[nodePaths.Length];
            for(int i= 0; i < nodePaths.Length; i++)
            {
                nodes[i] = (AssetDatabase.LoadAssetAtPath(nodePaths[i].GetRelativeUnityPath(), typeof(Node)) as Node);
            }
            return nodes;
        }

        /// <summary>
        /// Get the names of every Node in the Domain
        /// </summary>
        /// <param name="domainName">The domain to get nodes from</param>
        /// <returns>Array of node names in domain</returns>
        public static string[] GetNodeNames(string domainName)
        {
            string[] nodePaths = Directory.GetFiles(path + domainName, "*" + EXT, SearchOption.TopDirectoryOnly);
            string[] nodeNames = new string[nodePaths.Length];
            for(int i= 0; i < nodePaths.Length; i++)
            {
                nodeNames[i] = System.IO.Path.GetFileNameWithoutExtension(nodePaths[i]);
            }
            return nodeNames;
        }

        /// <summary>
        /// Returns the name of the domain the passed node is in
        /// </summary>
        /// <param name="node">Node to get domain of</param>
        /// <returns>Domain name</returns>
        public static string GetDomainOf(Node node)
        {
            string assetPath = AssetDatabase.GetAssetPath(node);
            string domain = Directory.GetParent(assetPath).FullName;
            return Path.GetRelativePath(path, domain);
        }
        #endregion
        #region Status
        /// <summary>
        /// Checks to see if the domain passed has only one node that has no parents / is a root
        /// </summary>
        /// <param name="domain">Domain to search</param>
        /// <returns>if domain contains only one root</returns>
        public static bool HasOneRoot(string domain)
        {
            int countOfRoots = 0;
            Node[] nodes = GetNodes(domain);
            for(int i = 0; i < nodes.Length; i++)
            {
                if (nodes[i].IsRoot) countOfRoots++;
            }
            return countOfRoots == 1;
        }
        #endregion
        #region Domain
        /// <summary>
        /// Create a node for each name passed in at a folder with fileName
        /// </summary>
        /// <param name="domainName">Folder Node scriptable objects will be saved</param>
        /// <param name="nodeNames">Names of Nodes to be created</param>
        public static void CreateDomain(string domainName, string[] nodeNames)
        {
            ValidateInputs(domainName, nodeNames);
            Directory.CreateDirectory(path + domainName);

            foreach (string nodeName in nodeNames)
            {
                Node node = Node.CreateInstance(nodeName);
                AddNode(domainName, node);
            }
        }

        /// <summary>
        /// Set up a default Domain so domain folder is never empty
        /// </summary>
        public static void CreateDefaultDomain()
        {
            try
            {
                string domainName = "Default";
                CreateDomain(domainName, new string[0]);
                Node master = Node.CreateInstance("Master");
                Node background = Node.CreateInstance("Background", master);
                Node UI = Node.CreateInstance("UI", master);
                Node PlayerActions = Node.CreateInstance("PlayerActions", master);
                Node EntityActions = Node.CreateInstance("EntityActions", master);
                AddNode(domainName, master);
                AddNode(domainName, background);
                AddNode(domainName, UI);
                AddNode(domainName, PlayerActions);
                AddNode(domainName, EntityActions);
            }catch(InputValidationException e)
            {
                throw new ApplicationException("Cannot Delete Default Domain if it is the only domain that exists. \n"+e.Message);
            }

        }


        /// <summary>
        /// Delete Domain by name, If no domain would remain, this creates a defualt Domain.
        /// </summary>
        /// <param name="domainName">Folder to Delete with path relative to the Domains Folder</param>
        public static void DeleteDomain(string domainName)
        {
            if (Domain.GetAll().Length == 1 && File.Exists(path + domainName))
            {
                CreateDefaultDomain();
            }
            AssetDatabase.DeleteAsset((path + domainName).GetRelativeUnityPath());
        }

        #endregion
        #region Nodes
        /// <summary>
        /// Saves and adds a node to the domain specified, saving it to the domain's folder
        /// NOTE: Will throw a DomainUniqueNamingException() if the passed node shares a name with a node already in the domain
        /// </summary>
        /// <param name="domainName">The domain the node will be added</param>
        /// <param name="node">The node to be saved</param>
        public static void AddNode(string domainName, Node node)
        {
            ValidateNodeName(domainName, node.name);
            string nodeSavePath = path + domainName + Path.AltDirectorySeparatorChar + node.name + EXT;
            AssetDatabase.CreateAsset(node, nodeSavePath.GetRelativeUnityPath());
            AssetDatabase.SaveAssets();
        }
        /// <summary>
        /// Removes specified node from domain if it is in the domain. Throws an Error if it is not
        /// </summary>
        /// <param name="domainName">Domain to search</param>
        /// <param name="node">Node to delete</param>
        public static void DeleteNode(string domainName, Node node)
        {
            if (!GetNodeNames(domainName).Contains(node.name)) throw new System.ArgumentException("Domain " + domainName + " does not contain a node named " + node.name);
            string nodeSavePath = path + domainName + Path.AltDirectorySeparatorChar + node.name + EXT;
            AssetDatabase.DeleteAsset(nodeSavePath.GetRelativeUnityPath());
            AssetDatabase.SaveAssets();
        }
        #endregion

        #region Utilities
        /// <summary>
        /// Validate Inputs For IO operations
        /// </summary>
        /// <param name="domainName"></param>
        /// <param name="nodeNames"></param>
        private static void ValidateInputs(string domainName, string[] nodeNames)
        {
            if (string.IsNullOrWhiteSpace(domainName)) throw new InputValidationException("Domain Name must not be white space or null");
            if (Directory.Exists(path + domainName)) throw new InputValidationException("Domain "+ domainName + " Already Exists. Please Enter another name for the domain");
            if (nodeNames.Distinct().Count() != nodeNames.Length) throw new InputValidationException("Node Names are not distinct. Please Enter names that are unique");
            nodeNames.ValidateInput();
            domainName.ValidateInput();
        }

        /// <summary>
        /// Check to make sure a nodeName is a valid input. Will throw a InputValidationException if not
        /// </summary>
        /// <param name="domainName">Domain to check</param>
        /// <param name="nodeName">Node that may be added</param>
        public static void ValidateNodeName(string domainName, string nodeName)
        {
            if (GetNodeNames(domainName).Contains(nodeName)) throw new InputValidationException("Node Names are not distinct. Please Enter names that are unique");
            nodeName.ValidateInput();
        }


        /// <summary>
        /// Provides the compile time location of this File so that a reference may be used to save the Enum files
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
        #endregion
    }
}