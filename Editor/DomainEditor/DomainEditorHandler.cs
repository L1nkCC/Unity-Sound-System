using System.Threading.Tasks;
using UnityEditor;
using System.IO;
using System.Linq;
using System;
using CC.Core.Utilities.IO;
using CC.Core.InputValidation;

namespace CC.SoundSystem
{
    public static class DomainEditorHandler
    {
        static readonly string path = Directory.GetParent(Directory.GetParent(FileLocation.Directory).FullName).FullName + Path.AltDirectorySeparatorChar + "Domains" + Path.AltDirectorySeparatorChar;
        const string EXT = ".asset";

        /// <summary>
        /// Returns all the names of domains
        /// </summary>
        /// <returns>All Domain names</returns>
        public static string[] GetDomains()
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
        /// Create a node for each name passed in at a folder with fileName
        /// </summary>
        /// <param name="domainName">Folder Node scriptable objects will be saved</param>
        /// <param name="nodeNames">Names of Nodes to be created</param>
        public static void CreateNodes(string domainName, string[] nodeNames)
        {
            ValidateInputs(domainName, nodeNames);
            Directory.CreateDirectory(path + domainName);

            foreach (string nodeName in nodeNames)
            {
                Node node = Node.CreateInstance(nodeName);
                string nodeSavePath = path + domainName + Path.AltDirectorySeparatorChar + nodeName + EXT;
                AssetDatabase.CreateAsset(node, nodeSavePath.GetRelativeUnityPath());
            }
            AssetDatabase.SaveAssets();
        }
        /// <summary>
        /// Delete Domain by name
        /// </summary>
        /// <param name="domainName">Folder to Delete with path relative to the Domains Folder</param>
        public static void DeleteDomain(string domainName)
        {
            AssetDatabase.DeleteAsset((path+domainName).GetRelativeUnityPath());
        }


        /// <summary>
        /// Validate Inputs For IO operations
        /// </summary>
        /// <param name="domainNames"></param>
        /// <param name="nodeNames"></param>
        private static void ValidateInputs(string domainNames, string[] nodeNames)
        {
            if (Directory.Exists(path + domainNames)) throw new System.IO.IOException("Domain Already Exists. Please Enter another name for the domain");
            if (nodeNames.Distinct().Count() != nodeNames.Length) throw new ArgumentException("Node Names are not distinct. Please Enter names that are unique");
            nodeNames.ValidateInput();
            domainNames.ValidateInput();
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
    }
}
