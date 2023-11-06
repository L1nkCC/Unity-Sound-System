using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace CC.SoundSystem
{
    /// <summary>
    /// Handle all Saving to disk operations for Domain
    /// </summary>
    public static class DomainSaveUtilities
    {
        #region Save Structs

        /// <summary>
        /// Class to Wrap Domain Dictionary for saving
        /// </summary>
        [System.Serializable]
        public class Settings
        {
            public DomainSave[] DomainSaves;

            public static Settings GetDomainDictionaryWrapper()
            {
                Settings settings = new();
                string[] domainNames = Domain.GetAll();
                settings.DomainSaves = new DomainSave[domainNames.Length];
                for(int i = 0; i < domainNames.Length; i++)
                {
                    settings.DomainSaves[i] = new DomainSave(domainNames[i]);
                }
                return settings;
            }
        }

        /// <summary>
        /// Wrap entries into dictionary
        /// </summary>
        [System.Serializable]
        public struct DomainSave
        {
            public string Name;
            public List<NodeSave> NodeSaves;

            /// <summary>
            /// Builds a wrapper for the past domain.
            /// NOTE: This will use DepthFirst inorder to assure an order that parent nodes will always appear before children, necessary for node link list relationships
            /// </summary>
            /// <param name="domainName">Domain to convert</param>
            public DomainSave(string domainName)
            {
                Name = domainName;
                NodeSaves = new();
                DomainSave domainSave = this;
                //Assure that order will always have the parent earlier in the list
                NodeUtilities.ForEachChildDepthFirst(Domain.GetRoots(domainName), (Node node, int level, int depth) => { domainSave.NodeSaves.Add(new NodeSave(node)); });
            }
            /// <summary>
            /// Convert to a Dictionary entry
            /// </summary>
            /// <returns>Dictionary entry for domain</returns>
            public (string, HashSet<Node>) Convert()
            {
                return (Name, NodeSave.Convert(NodeSaves));
            }

        }
        /// <summary>
        /// Wrap Node for dictionary. Necessary for maintaining link list relationships
        /// </summary>
        [System.Serializable]
        public struct NodeSave
        {
            public string Name;
            public string ParentName;
            /// <summary>
            /// NodeSave Constructor. Build a wrapper
            /// </summary>
            /// <param name="node">Node to convert</param>
            public NodeSave (Node node)
            {
                Name = node.name;
                ParentName = node.IsRoot ? "" : node.Parent.name;
            }
            /// <summary>
            /// Convert List to Node list. WIll fill link list relations
            /// NOTE: Must have parents appear before children in list. Must have all nodes referenced in list
            /// </summary>
            /// <param name="nodeSaves">Domain of nodes to connect</param>
            /// <returns>All converted nodes</returns>
            public static HashSet<Node> Convert(List<NodeSave> nodeSaves )
            {
                HashSet<Node> nodes = new();
                foreach(NodeSave nodeSave in nodeSaves)
                {
                    Node parent = null;
                    foreach(Node possibleparent in nodes)
                    {
                        if (possibleparent.name == nodeSave.ParentName) parent = possibleparent;
                    }
                    nodes.Add(Node.CreateInstance(nodeSave.Name, parent));
                }
                return nodes;
            }
        }
        #endregion

        static readonly string filePath = Application.persistentDataPath + Path.AltDirectorySeparatorChar + "CC"+ Path.AltDirectorySeparatorChar+ "SoundSystem" + Path.AltDirectorySeparatorChar;
        static readonly string fileName ="SoundSettings.json";

        #region Save/Write
        /// <summary>
        /// Save all information held in Domain dictionary
        /// </summary>
        public static void SaveAllDomains()
        {
            try
            {
                Core.Utilities.IO.IO.AssureDirectory(filePath);
                using (StreamWriter file = File.CreateText(filePath + fileName))
                {
                    file.Write(JsonUtility.ToJson(Settings.GetDomainDictionaryWrapper()));
                }
            }
            catch
            {
                Debug.LogError("Saving of Sound Settings Failed");
            }
        }
        #endregion

        /// <summary>
        /// Load all information held in Settings
        /// </summary>
        [RuntimeInitializeOnLoadMethod]
        public static void LoadAllDomains()
        {
            try
            {
                Core.Utilities.IO.IO.AssureDirectory(filePath);
                if (File.Exists(filePath + fileName))
                {
                    string fileContents;
                    using (StreamReader file = new StreamReader(filePath + fileName))
                    {
                        fileContents = file.ReadToEnd();
                    }
                    Settings settings = JsonUtility.FromJson<Settings>(fileContents);
                    Dictionary<string, HashSet<Node>> loadedSettingsDictionary = new();
                    foreach (DomainSave domainSave in settings.DomainSaves)
                    {
                        (string domainName, HashSet<Node> nodes) = domainSave.Convert();
                        loadedSettingsDictionary.Add(domainName, nodes);
                    }
                    Domain.SetDomains(loadedSettingsDictionary);
                }
            }
            catch ( System.Exception e)
            {
                Debug.LogError("Loading of Sound Settings Failed");
                Debug.LogError(e.ToString());
            }
        }


    }
}