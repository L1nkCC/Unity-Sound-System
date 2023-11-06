using UnityEngine;
using System.IO;
namespace CC.Core.Utilities.IO
{
    /// Author: L1nkCC
    /// Created: 10/24/2023
    /// Last Edited: 10/25/2023
    /// 
    /// <summary>
    /// Support for IO interactions
    /// </summary>
    public static class IO
    {
        public static readonly string Resources = Path.GetFullPath(Application.dataPath)+ Path.AltDirectorySeparatorChar + "Resources";
        /// <summary>
        /// Get path relative to the Application.dataPath. AKA: the asset folder
        /// </summary>
        /// <param name="path">Full path</param>
        /// <returns>The relative path of the file from the Asset folder</returns>
        public static string GetRelativeUnityPath(this string path)
        {
            return Path.GetRelativePath(Directory.GetParent(Application.dataPath).FullName, path);
        }
        /// <summary>
        /// Creates a directory if it does not exist
        /// </summary>
        /// <param name="directory">The directory to assure</param>
        public static void AssureDirectory(string directory)
        {
            if (!System.IO.Directory.Exists(directory))
            {
                System.IO.Directory.CreateDirectory(directory);
            }
        }
    }
}
