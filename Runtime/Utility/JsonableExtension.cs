using System;
using System.IO;
using Kadinche.Kassets.Variable;
using UnityEngine;

namespace Kadinche.Kassets
{
    public static class JsonableExtension
    {
        
#if UNITY_EDITOR
        private static readonly string DefaultPath = Application.dataPath;
#else
        private static readonly string DefaultPath = Application.persistentDataPath;
#endif
        
        private const string DefaultExtension = ".json";

        /// <summary>
        /// Load a json file from the given path.
        /// </summary>
        /// <param name="jsonable">Reference to variable to load</param>
        /// <param name="fullPath">Path to a directory where the json file exist. Path must include the json file.</param>
        public static void LoadFromJson(this IJsonable jsonable, string fullPath)
        {
            if (!File.Exists(fullPath))
            {
                Debug.LogError($"Failed to load from json. File not found: {fullPath}");
                return;
            }
            
            var jsonString = File.ReadAllText(fullPath);
            jsonable.FromJsonString(jsonString);
        }
        
        /// <summary>
        /// Load a json file from Unity's default data directory.
        /// </summary>
        /// <param name="jsonable">Reference to variable to load</param>
        public static void LoadFromJson(this IJsonable jsonable)
        {
            if (jsonable is ScriptableObject so)
            {
                LoadFromJson(jsonable, DefaultPath, so.name);
            }
        }
        
        /// <summary>
        /// Load a json file from a directory.
        /// </summary>
        /// <param name="jsonable">Reference to variable to load</param>
        /// <param name="directory">Path to a directory where the json file exist</param>
        /// <param name="filename">Name of the json file</param>
        public static void LoadFromJson(this IJsonable jsonable, string directory, string filename)
        {
            if (!Directory.Exists(directory))
            {
                Debug.LogError($"Failed to load from json. Directory not found: {directory}");
                return;
            }
            
            var fn = filename.Split('.').Length < 2 ? filename + DefaultExtension : filename;
            var fullPath = Path.Combine(directory, fn);
            LoadFromJson(jsonable, fullPath);
        }
        
        /// <summary>
        /// Load a json file with custom extension from a directory.
        /// </summary>
        /// <param name="jsonable">Reference to variable to load</param>
        /// <param name="directory">Path to a directory where the json file exist</param>
        /// <param name="filename">Name of the json file</param>
        /// <param name="extension">Custom extension of the json file</param>
        public static void LoadFromJson(this IJsonable jsonable, string directory, string filename, string extension)
        {
            var ext = extension[0] == '.' ? extension : "." + extension;
            var filenameExt = $"{filename}{ext}";
            LoadFromJson(jsonable, directory, filenameExt);
        }
        
        /// <summary>
        /// Save a variable into json file.
        /// </summary>
        /// <param name="jsonable"></param>
        public static void SaveToJson(this IJsonable jsonable)
        {
            if (jsonable is ScriptableObject so)
            {
                SaveToJson(jsonable, DefaultPath, so.name);
            }
        }
        
        /// <summary>
        /// Save a variable into json file to the given path.
        /// </summary>
        /// <param name="jsonable">Variable to save.</param>
        /// <param name="fullPath">Path to a directory where the json file would exist. Path must include the json filename and extension.</param>
        public static void SaveToJson(this IJsonable jsonable, string fullPath)
        {
            var jsonString = jsonable.ToJsonString();
            File.WriteAllText(fullPath, jsonString);
        }

        /// <summary>
        /// Save a variable into json file to the given directory.
        /// </summary>
        /// <param name="jsonable">Variable to save.</param>
        /// <param name="directory">Path to a directory where the json file would exist</param>
        /// <param name="filename">Name of the json file</param>
        public static void SaveToJson(this IJsonable jsonable, string directory, string filename)
        {
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            var fn = filename.Split('.').Length < 2 ? filename + DefaultExtension : filename;
            var fullPath = Path.Combine(directory, fn);
            SaveToJson(jsonable, fullPath);
        }
        
        /// <summary>
        /// Save a variable into json file with custom extension into the given directory.
        /// </summary>
        /// <param name="jsonable">Variable to save.</param>
        /// <param name="directory">Path to a directory where the json file would exist</param>
        /// <param name="filename">Name of the json file</param>
        /// <param name="extension">Custom extension of the json file</param>
        public static void SaveToJson(this IJsonable jsonable, string directory, string filename, string extension)
        {
            var ext = extension[0] == '.' ? extension : "." + extension;
            var filenameExt = $"{filename}{ext}";
            SaveToJson(jsonable, directory, filenameExt);
        }
        
        /// <summary>
        /// Check whether a json file exists.
        /// </summary>
        /// <param name="fullPath">Full path to the json file</param>
        /// <returns>true if json file exist.</returns>
        public static bool IsJsonFileExist(string fullPath)
        {
            return File.Exists(fullPath);
        }

        /// <summary>
        /// Check whether a json file exist in a directory.
        /// </summary>
        /// <param name="directory">Path to a directory where the json file would exist</param>
        /// <param name="filename">Name of the json file</param>
        /// <returns>true if json file exist.</returns>
        public static bool IsJsonFileExist(string directory, string filename)
        {
            if (!Directory.Exists(directory)) return false;
            var fn = filename.Split('.').Length < 2 ? filename + DefaultExtension : filename;
            var path = Path.Combine(directory, fn);
            return IsJsonFileExist(path);
        }
        
        /// <summary>
        /// Check whether a json file exist in a directory.
        /// </summary>
        /// <param name="_">Throw parameter. Used for method invocation.</param>
        /// <param name="directory">Path to a directory where the json file would exist</param>
        /// /// <param name="filename">Name of the json file</param>
        /// <returns></returns>
        public static bool IsJsonFileExist(this IJsonable _, string directory, string filename)
        {
            return IsJsonFileExist(directory, filename);
        }
        
        /// <summary>
        /// Check whether a json file exist.
        /// </summary>
        /// <param name="jsonable"></param>
        /// <returns></returns>
        public static bool IsJsonFileExist(this IJsonable jsonable)
        {
            return jsonable is ScriptableObject so && IsJsonFileExist(DefaultPath, so.name);
        }
    }
}