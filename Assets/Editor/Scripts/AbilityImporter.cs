/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

using System.IO;
using Newtonsoft.Json.Linq;
using Noz.RuneHaze.EditorUtilities;
using UnityEditor;
using UnityEngine;
using UnityEditor.AssetImporters;

namespace NoZ.RuneHaze.UI
{
    [ScriptedImporter(2, "ability", importQueueOffset:2000)]
    public class AbilityImporter : ScriptedImporter
    {
        [MenuItem("Assets/Create/RuneHaze/Ability")]
        private static void Create()
        {
            EditorUtilities.EditorUtility.CreateAssetFromTemplate<EditorUtilities.EditorUtility.DoCreateAsset>("Assets/Editor/Templates/Ability.ability.template", "New Ability.ability");
        }
        
        public override void OnImportAsset(AssetImportContext ctx)
        {
            var text = File.ReadAllText(Path.Combine(Path.GetDirectoryName(Application.dataPath), ctx.assetPath));
            var json = JObject.Parse(text);
            ctx.SetMainObject(ImportUtility.ImportScriptableObject(ctx, typeof(Ability), json));
        }
    }
}