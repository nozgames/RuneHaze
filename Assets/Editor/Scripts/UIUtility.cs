/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

namespace RuneHaze
{
    public static class UIUtility
    {
        private const string CreateUIViewMenuItem = "Assets/Create/NoZ/UI/View";
        private const string CreateUIControlMenuItem = "Assets/Create/NoZ/UI/Control";
        private const string CreateViewFactoryMenuItem = "Assets/Create/NoZ/UI/Factory";
        private static readonly string TemplatePath = Path.Combine(Application.dataPath, "Editor", "Templates");
        private static readonly Regex ClassToUss = new (@"([A-Z]|\d+)");

        private static string UssShortName(string name)
        {
            // We preface our user interfaces with `UI` alot so this will make sure the uss name doesnt include it
            if (name.StartsWith("UI"))
                name = name.Substring(2);

            return name;
        }

        public static string NameToUss(string name)
        {
            if (char.IsLower(name[0]))
                name = char.ToUpper(name[0]) + name.Substring(1);

            return ClassToUss.Replace(name, "-$1").Substring(1).ToLower();
        }

        private static string GetSchemaPath(string path)
        {
            var relative = "";

            while (!path.EndsWith("Assets"))
            {
                relative += "../";
                path = Path.GetDirectoryName(path);
            }

            return relative + "UIElementsSchema/UIElements.xsd";
        }

        private class DoCreateUIScript : EditorUtility.DoCreateScriptAsset
        {
            protected override string Preprocess(string pathName, string value)
            {
                var result = base.Preprocess(pathName, value);

                var name = Path.GetFileNameWithoutExtension(pathName);
                var shortname = UssShortName(name);

                result = result.Replace("#SCRIPTNAME_SHORT#", shortname);
                result = result.Replace("#USSNAME#", NameToUss(shortname));

                return result;
            }
        }

        private class DoCreateUIView : DoCreateUIScript
        {
            public override void Action(int instanceId, string pathName, string resourceFile)
            {
                base.Action(instanceId, pathName, resourceFile);

                var scriptName = Path.GetFileNameWithoutExtension(pathName);
                var scriptNameShort = UssShortName(scriptName);
                var ussname = NameToUss(scriptNameShort);
                var schemaPath = GetSchemaPath(pathName);
                var variables = new System.Tuple<string, string>[]
                {
                    new System.Tuple<string, string>("#NAMESPACE#", CompilationPipeline.GetAssemblyRootNamespaceFromScriptPath(pathName)),
                    new System.Tuple<string, string>("#SCRIPTNAME_SHORT#", scriptNameShort),
                    new System.Tuple<string, string>("#SCRIPTNAME#", scriptName),
                    new System.Tuple<string, string>("#USSNAME#", ussname),
                    new System.Tuple<string, string>("#SCHEMAPATH#", schemaPath)
                };

                var ussPath = Path.ChangeExtension(pathName, ".uss");
                var uxmlPath = Path.ChangeExtension(pathName, ".uxml");
                EditorUtility.CreateFileFromTemplate("Assets/Editor/Templates/UIView.uxml.template", uxmlPath, variables);
                EditorUtility.CreateFileFromTemplate("Assets/Editor/Templates/UIView.uss.template", ussPath, variables);

                AssetDatabase.ImportAsset(uxmlPath);
                AssetDatabase.ImportAsset(ussPath);

                // Add uxml to closest factory
                EditorUtility.AddTextToAssetInParent(pathName, typeof(UI.VisualTreeFactory), $"@import url(\"/{uxmlPath}\");");

                AssetDatabase.Refresh();
            }
        }

        private class DoCreateFactory : DoCreateUIScript
        {
            public override void Action(int instanceId, string pathName, string resourceFile)
            {
                base.Action(instanceId, pathName, resourceFile);

                AssetDatabase.ImportAsset(pathName);

                EditorUtility.AddTextToAssetInParent(Path.GetDirectoryName(Path.GetDirectoryName(pathName)), typeof(UI.VisualTreeFactory), $"@import url(\"/{pathName}\");");
                AssetDatabase.Refresh();
            }
        }

        [MenuItem(CreateUIViewMenuItem, priority = int.MinValue)]
        private static void CreateUIView()
        {
            EditorUtility.CreateScriptFromTemplate<DoCreateUIView>(Path.Combine(TemplatePath, "UIView.cs.template"), "UINewView.cs");
        }

        [MenuItem(CreateUIControlMenuItem, priority = int.MinValue + 1)]
        private static void CreateUIControl()
        {
            EditorUtility.CreateScriptFromTemplate<DoCreateUIScript>(Path.Combine(TemplatePath, "UIControl.cs.template"), "UINewControl.cs");
        }

        [MenuItem(CreateViewFactoryMenuItem, priority = int.MinValue + 2)]
        private static void CreateViewVactory()
        {
            EditorUtility.CreateScriptFromTemplate<DoCreateFactory>(Path.Combine(TemplatePath, "VisualTreeFactory.uxmlf.template"), "NewFactory.uxmlf");
        }
    }
}
