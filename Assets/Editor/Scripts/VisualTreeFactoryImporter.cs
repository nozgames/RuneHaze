/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine.UIElements;

namespace NoZ.RuneHaze.UI
{
    [ScriptedImporter(2, "uxmlf", importQueueOffset:2000)]
    public class VisualTreeFactoryImporter : ScriptedImporter
    {
        private static readonly Regex ImportRegex = new Regex(@"@import(\s+url\s*\(\s*\""([/\.\d\w]*)\""\)(?:\s+as\s+(\w[\w\d_]*))?\s*)+;");

        public override void OnImportAsset(AssetImportContext ctx)
        {
            var text = File.ReadAllText(Path.Combine(Path.GetDirectoryName(Application.dataPath), ctx.assetPath));
            var dir = Path.GetDirectoryName(ctx.assetPath);
            var matches = ImportRegex.Matches(text);

            var entries = new List<VisualTreeFactory.Entry>();
            var inherit = new List<VisualTreeFactory>();
            foreach(Match match in matches)
            {
                if (match.Groups.Count < 3 || match.Groups[2].Captures.Count <= 0)
                {
                    Debug.LogWarning($"Missing or malformatted url '{match.Value}'");
                    continue;
                }

                var captures = match.Groups[2].Captures;
                var url = captures[0].Value;
                if(url.EndsWith(".uxml"))
                {
                    var entry = ParseEntry(ctx, url, dir, match);
                    if (null != entry)
                        entries.Add(entry);
                }
                else if (url.EndsWith(".uxmlf"))
                {
                    var inheritFactory = ParseInherit(ctx, url);
                    if (null != inheritFactory)
                        inherit.Add(inheritFactory);
                }
                else
                {
                    Debug.LogWarning($"Unsupported import file type '{url}'");
                    continue;
                }
            }

            var factory = UI.VisualTreeFactory.CreateInstance(inherit.ToArray(), entries.ToArray());
            ctx.AddObjectToAsset("factory", factory);
            ctx.SetMainObject(factory);
        }

        private VisualTreeFactory ParseInherit(AssetImportContext ctx, string url)
        {
            url = url.Substring(1);
            ctx.DependsOnArtifact(url);

            var inheritFactory = AssetDatabase.LoadAssetAtPath<UI.VisualTreeFactory>(url);
            if (null == inheritFactory && !File.Exists(url))
            {
                Debug.LogWarning($"Missing asset for '{url}' in factory '{ctx.assetPath}'");
                return null;
            }

            return inheritFactory;
        }

        private VisualTreeFactory.Entry ParseEntry(AssetImportContext ctx, string url, string dir, Match match)
        {
            var captures = match.Groups[2].Captures;
            var assetPath = url.StartsWith("./") ? (dir + "/" + url.Substring(2)) : url.Substring(1);
            var asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(assetPath);
            if (asset == null)
            {
                Debug.LogWarning($"Missing asset for '{assetPath} in factory '{ctx.assetPath}'");
                return null;
            }

            var styles = new List<StyleSheet>();

            // Additional sheets?
            if (captures.Count > 1)
            {
                for (int captureIndex = 1; captureIndex < captures.Count; captureIndex++)
                {
                    var capture = captures[captureIndex];
                    if (capture.Value.EndsWith(".uss"))
                    {
                        if (!LoadStyleSheet(ctx, capture.Value, styles))
                        {
                            Debug.LogWarning($"Missing asset for '{capture.Value} in factory '{ctx.assetPath}'");
                            continue;
                        }
                    }
                    else if (capture.Value.EndsWith(".tss"))
                    {
                        if (!LoadTheme(ctx, capture.Value, styles))
                        {
                            Debug.LogWarning($"Missing asset for '{capture.Value} in factory '{ctx.assetPath}'");
                            continue;
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"Unsupported import file type '{url}'");
                        continue;
                    }
                }
            }

            // Try to load a sheet with the same name
            if (!LoadTheme(ctx, Path.ChangeExtension(url, ".tss"), styles))
                LoadStyleSheet(ctx, Path.ChangeExtension(url, ".uss"), styles);

            var entry = new VisualTreeFactory.Entry();
            entry.Asset = asset;
            entry.Styles = styles.Count > 0 ? styles.ToArray() : null;
            return entry;
        }

        private bool LoadStyleSheet(AssetImportContext ctx, string url, List<StyleSheet> sheets)
        {
            var dir = Path.GetDirectoryName(ctx.assetPath);
            var stylePath = url.StartsWith("./") ? (dir + "/" + url.Substring(2)) : url.Substring(1);
            var styleAsset = AssetDatabase.LoadAssetAtPath<StyleSheet>(stylePath);
            if (styleAsset == null)
                return false;

            if (sheets.Contains(styleAsset))
                return true;

            sheets.Add(styleAsset);

            return true;
        }

        private bool LoadTheme(AssetImportContext ctx, string url, List<StyleSheet> sheets)
        {
            var dir = Path.GetDirectoryName(ctx.assetPath);
            var stylePath = url.StartsWith("./") ? (dir + "/" + url.Substring(2)) : url.Substring(1);
            var styleAsset = AssetDatabase.LoadAssetAtPath<ThemeStyleSheet>(stylePath);
            if (styleAsset == null)
                return false;

            if (sheets.Contains(styleAsset))
                return true;

            sheets.Add(styleAsset);

            return true;
        }
    }
}
