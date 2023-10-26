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

            public void Overwrite()
            {
                Domain.ClearDomain(Name);
                for (int i = 0; i < NodeSaves.Count; i++)
                {
                    NodeSaves[i].CreateInstance(Name);
                }
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

            public void CreateInstance(string domainName)
            {
                Domain.AddNode(domainName, Node.CreateInstance(Name,Domain.GetNode(domainName, ParentName)));
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
                    foreach (DomainSave domainSave in settings.DomainSaves)
                    {
                        domainSave.Overwrite();
                    }
                }
            }
            catch
            {
                Debug.LogError("Loading of Sound Settings Failed");
            }
        }


    }
}