using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace CC.SoundSystem
{
    public static class DomainSaveUtilities
    {
        [System.Serializable]
        public struct DomainSave
        {
            public string Name;
            public string[] NodeNames;

            public DomainSave(string domainName)
            {
                Name = domainName;
                NodeNames = Domain.GetNodeNames(domainName);
            }
        }

        static readonly string path = Application.persistentDataPath + Path.AltDirectorySeparatorChar + "CC/SoundSystem/SoundSettings/";
        const string EXT = ".json";

        public static void SaveAllDomains()
        {
            foreach (string domain in Domain.GetAll()) {
                WriteDomainJSON(domain);
            }
        }
        private static void WriteDomainJSON(string domainName)
        {
            Write(domainName, JsonUtility.ToJson(new DomainSave(domainName)));
        }

        private static void Write(string fileName, string json)
        {
            CC.Core.Utilities.IO.IO.AssureDirectory(path);
            using(StreamWriter file = File.CreateText(path + fileName + EXT))
            {
                file.Write(json);
            }
        }

        public static void LoadAllDomains()
        {
            CC.Core.Utilities.IO.IO.AssureDirectory(path);
            string[] domainJSONpaths = Directory.GetFiles(path, "*" + EXT);
            DomainSave[] domainJSON = new DomainSave[domainJSONpaths.Length];
            for(int i = 0; i < domainJSONpaths.Length; i++)
            {
                domainJSON[i] = JsonUtility.FromJson<DomainSave>(domainJSONpaths[i]);
            }
        }

    }
}