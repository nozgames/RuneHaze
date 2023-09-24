/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

using System.IO;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;
using UnityEditor.AssetImporters;

using Noz.RuneHaze.EditorUtilities;

namespace NoZ.RuneHaze.UI
{
    [ScriptedImporter(2, "target", importQueueOffset:2000)]
    public class TargetFinderImporter : ScriptedImporter
    {
        [MenuItem("Assets/Create/RuneHaze/TargetFinder")]
        private static void Create()
        {
            EditorUtilities.EditorUtility.CreateAssetFromTemplate<EditorUtilities.EditorUtility.DoCreateAsset>("Assets/Editor/Templates/TargetFinder.target.template", "New Target Finder.target");
        }
        
        public override void OnImportAsset(AssetImportContext ctx)
        {
            var text = File.ReadAllText(Path.Combine(Path.GetDirectoryName(Application.dataPath), ctx.assetPath));
            var json = JObject.Parse(text);
            ctx.SetMainObject(ImportUtility.ImportScriptableObject(ctx, typeof(TargetFinder), json));
        }
    }
}