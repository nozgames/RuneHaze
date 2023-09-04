/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;

namespace RuneHaze
{
    public static class EditorUtility
    {
        public static readonly string TemplatePath = Path.Combine(UnityEngine.Application.dataPath, "Editor", "Templates");

        public static string Canonical(this string path) => path.Replace('\\', '/');

        /// <summary>
        /// Find the path of an asset of a given type in the given path or any parent path
        /// </summary>
        public static string FindAssetTypePathInParent(string path, System.Type type)
        {
            while (!path.EndsWith("Assets"))
            {
                path = path.Canonical();

                foreach (var a in AssetDatabase.FindAssets($"t:{type.Name}", new string[] { path }))
                {
                    var assetPath = AssetDatabase.GUIDToAssetPath(a);
                    if (Path.GetDirectoryName(assetPath).Canonical() != path)
                        continue;

                    return assetPath.Canonical();
                }

                path = System.IO.Path.GetDirectoryName(path);
            }

            return null;
        }

        /// <summary>
        /// Exposed from ProjectWindowUtil
        /// </summary>
        internal static UnityEngine.Object CreateScriptAssetWithContent(string pathName, string templateContent)
        {
            var method = typeof(ProjectWindowUtil).GetMethod("CreateScriptAssetWithContent", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            return (UnityEngine.Object)method.Invoke(null, new object[] { pathName, templateContent });
        }

        /// <summary>
        /// Exposed from ProjectWindowUtil
        /// </summary>
        internal static string PreprocessScriptAssetTemplate(string pathName, string resourceContent)
        {
            var method = typeof(ProjectWindowUtil).GetMethod("PreprocessScriptAssetTemplate", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            var result = (string)method.Invoke(null, new object[] { pathName, resourceContent });            
            return result;
        }

        public static UnityEngine.Object CreateScriptAssetFromTemplate(string pathName, string resourceFile, System.Func<string,string,string> preprocess=null)
        {
            string resourceContent = File.ReadAllText(resourceFile);
            string preprocessed = PreprocessScriptAssetTemplate(pathName, resourceContent);
            if (preprocess != null)
                preprocessed = preprocess.Invoke(pathName, preprocessed);
            return CreateScriptAssetWithContent(pathName, preprocessed);
        }

        public class DoCreateScriptAsset : UnityEditor.ProjectWindowCallback.EndNameEditAction
        {
            public override void Action(int instanceId, string pathName, string resourceFile)
            {
                UnityEngine.Object o = CreateScriptAssetFromTemplate(pathName, resourceFile, Preprocess);
                ProjectWindowUtil.ShowCreatedAsset(o);
            }

            protected virtual string Preprocess (string pathName, string value)
            {
                return value;
            }
        }

        public static void CreateScriptFromTemplate<TAction> (string templatePath, string defaultNewFileName) where TAction : DoCreateScriptAsset
        {
            if (templatePath == null)
                throw new System.ArgumentNullException("templatePath");

            if (!File.Exists(templatePath))
                throw new FileNotFoundException("The template file \"" + templatePath + "\" could not be found.", templatePath);

            if (string.IsNullOrEmpty(defaultNewFileName))
                defaultNewFileName = Path.GetFileName(templatePath);

            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(
                icon: Path.GetExtension(templatePath) switch
                {
                    ".cs" => EditorGUIUtility.IconContent("cs Script Icon").image as UnityEngine.Texture2D,
                    _ => EditorGUIUtility.IconContent("ScriptableObject Icon").image as UnityEngine.Texture2D,
                },
                instanceID: 0,
                endAction: UnityEngine.ScriptableObject.CreateInstance<TAction>(),
                pathName: defaultNewFileName,
                resourceFile: templatePath);
        }

        /// <summary>
        /// Create a file from a template
        /// </summary>
        /// <param name="templatePath">Path to the template file</param>
        /// <param name="path">Path to the file to create</param>
        /// <param name="variables">Variables replace in the template</param>
        public static void CreateFileFromTemplate(string templatePath, string path, System.Tuple<string, string>[] variables)
        {
            var contents = File.ReadAllText(templatePath);
            if (variables != null)
                foreach (var variable in variables)
                    contents = contents.Replace(variable.Item1, variable.Item2);

            File.WriteAllText(path, contents);
        }

        /// <summary>
        /// Add text to a text asset of the given type in the same directory or a parent directory
        /// </summary>
        /// <param name="pathName">Starting path of the asset</param>
        /// <param name="type">Type of asset to add too</param>
        /// <param name="text">Text to add</param>
        public static void AddTextToAssetInParent(string pathName, System.Type type, string text)
        {
            var assetPath  = EditorUtility.FindAssetTypePathInParent(pathName, type);
            if (null == assetPath)
                return;

            var contents = File.ReadAllText(assetPath);
            if (contents.Contains(text))
                return;

            if (!contents.EndsWith("\n"))
                contents = contents + "\n";

            contents = contents + text + "\r\n";

            File.WriteAllText(assetPath, contents);
        }

        private static readonly Regex s_FindTypeRegex = new (@"((?:\w[\w\d]*\.)*)(\w[\w\d]*)");

        /// <summary>
        /// Find a type by name that is optionally relative to a given namespace.  The type
        /// name can be namespace qualified as well.
        /// </summary>
        public static Type FindType(string typeName, Type baseType = null, string relativeNamespace = null)
        {
            if (baseType == null)
                baseType = typeof(object);

            var match = s_FindTypeRegex.Match(typeName);
            if (!match.Success || match.Groups.Count < 3)
                return null;

            var typeNamespace = match.Groups[1].Value;
            if (typeNamespace.Length > 0)
                typeNamespace = typeNamespace.Substring(0, typeNamespace.Length - 1);

            typeName = match.Groups[2].Value;

            return 
                FindType(typeNamespace, typeName, baseType, relativeNamespace) ??
                FindType(typeNamespace.Replace('_','.'), typeName, baseType, relativeNamespace);
        }
        
        private static Type FindType(string typeNamespace, string typeName, Type baseType = null, string relativeNamespace = null)
        {
            relativeNamespace ??= "";

            // The search namespace is the relativeNamespace plus the typenamespace.  For each
            // iteration through the loop we check the combination of the two and if the type
            // is not found we remove one level of the relativeNamespace until the type is either
            // found or there is no more relativeNamespace left.
            while (true)                
            {
                var currentNamespace = relativeNamespace == "" ?
                    typeNamespace :
                    relativeNamespace + (typeNamespace.Length > 0 ? "." + typeNamespace : "");

                var type = TypeCache.GetTypesDerivedFrom(baseType).FirstOrDefault(t => t.Namespace == currentNamespace && t.Name == typeName);
                if (type != null)
                    return type;

                var lastPeriod = relativeNamespace.LastIndexOf('.');
                if (lastPeriod == -1)
                    return null;

                relativeNamespace = relativeNamespace.Substring(0,lastPeriod);
            }
        }
    }
}
