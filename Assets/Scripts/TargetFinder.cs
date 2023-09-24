/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.AssetImporters;
using Newtonsoft.Json.Linq;
#endif

namespace NoZ.RuneHaze
{
    public abstract class TargetFinder : ScriptableObject
    {
        protected static readonly Collider[] s_colliders = new Collider[128];
        
        private class SelfTargetFinder : TargetFinder
        {
            protected override void AddTargets(Actor source) => Add(source);
        }

        private class EmptyTargetFinder : TargetFinder
        {
            protected override void AddTargets(Actor source) { }
        }

        private class DefaultTargetFinder : TargetFinder
        {
            protected override void AddTargets(Actor source) => Add(source.Target);
        }

        private static SelfTargetFinder _selfTarget;
        private static DefaultTargetFinder _defaultTarget;
        private static EmptyTargetFinder _emptyTarget;

        private Actor _source = null;
        private int _frame = 0;
        private List<Actor> _targets = new List<Actor>();

        /// <summary>
        /// List of cached targets
        /// </summary>
        public List<Actor> Targets => _targets;

        /// <summary>
        /// Number of cached targets
        /// </summary>
        public int Count => _targets.Count;

        /// <summary>
        /// True if the TargetFinder has any cached targets
        /// </summary>
        public bool HasTargets => _targets.Count > 0;

        /// <summary>
        /// Clear all cached targets for this target finder
        /// </summary>
        public void Clear ()
        {
            _frame = 0;
            _source = null;
            _targets.Clear();
        }

        private static TargetFinder GetSelfTargetFinder(Actor source)
        {
            if (_selfTarget == null)
                _selfTarget = CreateInstance<SelfTargetFinder>();

            _selfTarget.Clear();
            _selfTarget.FindTargets(source);

            return _selfTarget;
        }

        private static TargetFinder GetEmptyTargetFinder()
        {
            if(_emptyTarget == null)
                _emptyTarget = CreateInstance<EmptyTargetFinder>();

            return _emptyTarget;
        }

        private static TargetFinder GetDefaultTargetFinder (Actor source)
        {
            if(_defaultTarget == null)
                _defaultTarget = CreateInstance<DefaultTargetFinder>();

            _defaultTarget.Clear();
            _defaultTarget.FindTargets(source);

            return _defaultTarget;
        }

        /// <summary>
        /// Find all targets for the given actor source
        /// </summary>
        /// <param name="source"></param>
        public int FindTargets (Actor source)
        {
            // Already cached the targets?
            if (source == _source && _frame == Time.frameCount)
                return _targets.Count;

            Clear();
            AddTargets(source);

            return _targets.Count;
        }

        /// <summary>
        /// Add a target to the cached targets list
        /// </summary>
        protected void Add(Actor target)
        {
            if (target == null)
                return;

            _targets.Add(target);
        }

        /// <summary>
        /// Implement to add targets for the given source
        /// </summary>
        protected abstract void AddTargets (Actor source);

        /// <summary>
        /// Find targets for the given source handling self targetting and inheritance
        /// </summary>
        public static TargetFinder FindTargets(Actor source, TargetType type, TargetFinder custom=null, TargetFinder inherit=null)
        {
            switch (type)
            {
                case TargetType.Custom:
                    if (custom != null)
                    {
                        custom.FindTargets(source);
                        return custom;
                    }

                    if (_emptyTarget != null)
                        _emptyTarget = new EmptyTargetFinder();
                    return _emptyTarget;

                default:
                case TargetType.Inherit:
                    return inherit != null ? inherit : GetDefaultTargetFinder(source);

                case TargetType.Self:
                    return GetSelfTargetFinder(source);
            }
        }
        
#if UNITY_EDITOR
        protected virtual void OnImport(AssetImportContext ctx, JObject token)
        {
        }
        
        private static readonly Dictionary<string, System.Type> s_targetFinderTypes = new(); 
        
        static TargetFinder()
        {
            var lobeTypes = System.AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => typeof(TargetFinder).IsAssignableFrom(t))
                .ToArray();

            foreach (var type in lobeTypes)
                s_targetFinderTypes[type.Name] = type;
        }

        private static TargetFinder CreateInstance(AssetImportContext ctx, string typeName)
        {
            if (!s_targetFinderTypes.TryGetValue(typeName, out var type))
                return null;

            return (TargetFinder)ScriptableObject.CreateInstance(type);
        }      
        
        public static TargetFinder Import(AssetImportContext ctx, JToken token)
        {
            if (token is JValue)
            {
                var path = token.Value<string>();
                if (string.IsNullOrEmpty(path))
                    return null;
                
                var targetFinder = AssetDatabase.LoadAssetAtPath<TargetFinder>(path);
                if (targetFinder != null)
                    ctx.DependsOnSourceAsset(AssetDatabase.GetAssetPath(targetFinder));
                
                return targetFinder;
            }
            else if (token is JObject json)
            {
                var typeName = json["type"]?.Value<string>();
                if (string.IsNullOrEmpty(typeName))
                {
                    Debug.LogError("Target type is missing");
                    return null;
                }

                var name = json["name"]?.Value<string>() ?? typeName;
                if (string.IsNullOrEmpty(name))
                {
                    Debug.LogError("Lobe name is missing");
                    return null;
                }
                
                var targetFinder = CreateInstance(ctx, typeName);
                if (targetFinder == null)
                    return null;

                targetFinder.name = name;
                targetFinder.OnImport(ctx, json);
                ctx.AddObjectToAsset(targetFinder.name, targetFinder);
                return targetFinder;
            }

            return null;
        }
#endif             
    }
}
