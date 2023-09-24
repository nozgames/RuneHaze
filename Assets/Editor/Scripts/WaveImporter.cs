/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

using System.IO;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;
using UnityEditor.AssetImporters;

namespace NoZ.RuneHaze.UI
{
    [ScriptedImporter(2, "wave", importQueueOffset:2000)]
    public class WaveImporter : ScriptedImporter
    {
        [MenuItem("Assets/Create/RuneHaze/Wave")]
        private static void Create()
        {
            EditorUtilities.EditorUtility.CreateAssetFromTemplate<EditorUtilities.EditorUtility.DoCreateAsset>("Assets/Editor/Templates/Wave.wave.template", "New Wave.wave");
        }
        
        public override void OnImportAsset(AssetImportContext ctx)
        {
            var text = File.ReadAllText(Path.Combine(Path.GetDirectoryName(Application.dataPath), ctx.assetPath));
            var json = JObject.Parse(text);
            ctx.SetMainObject(Wave.Import(ctx, json));
        }
    }
}