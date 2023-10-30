using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Collections;

namespace CC.SoundSystem
{
    public static class DomainSaveUtilities
    {
        #region Save Structs

        [System.Serializable]
        public class Settings
        {
            public DomainSave[] DomainSaves;

            public Settings()
            {
                string[] domainNames = Domain.GetAll();
                DomainSaves = new DomainSave[domainNames.Length];
                for(int i = 0; i < domainNames.Length; i++)
                {
                    DomainSaves[i] = new DomainSave(domainNames[i]);
                }
            }
        }

        [System.Serializable]
        public struct DomainSave
        {
            public string Name;
            public List<NodeSave> NodeSaves;

            public DomainSave(string domainName)
            {
                Name = domainName;
                NodeSaves = new();
                DomainSave domainSave = this;
                //Assure that order will always have the parent earlier in the list
                NodeUtilities.ForEachChildDepthFirst(Domain.GetRoots(domainName), (Node node, int level, int depth) => { domainSave.NodeSaves.Add(new NodeSave(node)); });
            }

/*            public void Overwrite()
            {
                Domain.ClearDomain(Name);
                for (int i = 0; i < NodeSaves.Count; i++)
                {
                    NodeSaves[i].CreateInstance(Name);
                }
            }*/
            public (string, HashSet<Node>) Convert()
            {
                return (Name, NodeSave.Convert(NodeSaves));
            }

        }
        [System.Serializable]
        public struct NodeSave
        {
            public string Name;
            public string ParentName;
            public NodeSave (Node node)
            {
                Name = node.name;
                ParentName = node.IsRoot ? "" : node.Parent.name;
            }

/*            public void CreateInstance(string domainName)
            {
                Domain.AddNode(domainName, Node.CreateInstance(Name,Domain.GetNode(domainName, ParentName)));
            }*/
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
        public static void SaveAllDomains()
        {
            Write(JsonUtility.ToJson(new Settings()));
        }

        private static void Write(string json)
        {
            try
            {
                CC.Core.Utilities.IO.IO.AssureDirectory(filePath);
                using (StreamWriter file = File.CreateText(filePath + fileName))
                {
                    file.Write(json);
                }
            }
            catch
            {
                Debug.LogError("Saving of Sound Settings Failed");
            }
        }
        #endregion

        [RuntimeInitializeOnLoadMethod]
        public static void LoadAllDomains()
        {
            try
            {
                CC.Core.Utilities.IO.IO.AssureDirectory(filePath);
                if (System.IO.File.Exists(filePath + fileName))
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