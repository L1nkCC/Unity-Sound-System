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
    /// Last Edited: 10/18/2023
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
        /// Delete Domain by name
        /// </summary>
        /// <param name="domainName">Folder to Delete with path relative to the Domains Folder</param>
        public static void DeleteDomain(string domainName)
        {
            AssetDatabase.DeleteAsset((path+domainName).GetRelativeUnityPath());
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
            if (GetNodeNames(domainName).Contains(node.name)) throw new DomainUniqueNamingException("Domain "+ domainName + " already has a node named " + node.name + ". Please Enter a unqiuely named node for this domain");
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
        /// <param name="domainNames"></param>
        /// <param name="nodeNames"></param>
        private static void ValidateInputs(string domainNames, string[] nodeNames)
        {
            if (Directory.Exists(path + domainNames)) throw new DomainUniqueNamingException("Domain Already Exists. Please Enter another name for the domain");
            if (nodeNames.Distinct().Count() != nodeNames.Length) throw new DomainUniqueNamingException("Node Names are not distinct. Please Enter names that are unique");
            nodeNames.ValidateInput();
            domainNames.ValidateInput();
        }


        /// <summary>
        /// Exception regarding naming conflictions within Domains or with Domains
        /// </summary>
        [Serializable]
        public class DomainUniqueNamingException : Exception
        {
            public DomainUniqueNamingException() { }
            public DomainUniqueNamingException(string message) : base(message) { }
            public DomainUniqueNamingException(string message, Exception inner) : base(message, inner) { }
            protected DomainUniqueNamingException(
              System.Runtime.Serialization.SerializationInfo info,
              System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
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
