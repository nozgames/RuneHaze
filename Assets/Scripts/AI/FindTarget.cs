/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

using UnityEngine;

#if UNITY_EDITOR
using Newtonsoft.Json.Linq;
using UnityEditor.AssetImporters;
#endif

namespace NoZ.RuneHaze
{
    public class FindTarget : Lobe
    {
        [SerializeField] private TargetFinder _targetFinder = null;

        public override float CalculateScore(Actor source, IThinkState state)
        {
            if (_targetFinder.FindTargets(source) == 0)
                return 0.0f;

            if (_targetFinder.Targets[0] == source.Target)
                return 0.0f;

            return 1.0f;
        }

        public override void Think(Actor source, IThinkState state)
        {
            if (_targetFinder.Count == 0)
                return;

            source.SetDestination(new Destination(_targetFinder.Targets[0]));
        }
        
#if UNITY_EDITOR
        protected override void OnImport(AssetImportContext ctx, JObject token)
        {
            base.OnImport(ctx, token);

            if (token["targetFinder"] is { } targetFinderToken)
                _targetFinder = TargetFinder.Import(ctx, targetFinderToken);
        }
#endif        
    }
}