/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.AssetImporters;
using Newtonsoft.Json.Linq;
#endif

namespace NoZ.RuneHaze
{
    public interface IThinkState
    {
        void OnAlloc(Actor actor);

        void OnRelease();
    }

    /// <summary>
    /// Defines an abstract Brain Lobe to control an actor
    /// </summary>
    public abstract class Lobe : ScriptableObject
    {
        [Tooltip("Base score for the lobe")]
        [SerializeField] private float _baseScore = 1.0f;

        /// <summary>
        /// Base score of the lobe, multiplied against the calculated score
        /// </summary>
        public float BaseScore => _baseScore;

        /// <summary>
        /// Calculate a score for this lobe, if the lobe is later chosen any state that was 
        /// calculated during this call can be reused
        /// </summary>
        public abstract float CalculateScore(Actor actor, IThinkState abstractState);

        /// <summary>
        /// Think for a single frame.
        /// </summary>
        public abstract void Think(Actor actor, IThinkState abstractState);

        /// <summary>
        /// Called after CalculateScore was called but this lobe was not chosen to be active
        /// </summary>
        public virtual void DontThink(Actor actor, IThinkState state) { }

        /// <summary>
        /// Create an optional think state to be passed to think
        /// </summary>
        public virtual IThinkState AllocThinkState(Actor actor) => null;

        /// <summary>
        /// Optionally release a think state that was previously created from this brain
        /// </summary>
        public virtual void ReleaseThinkState(IThinkState state) { }
        
#if UNITY_EDITOR
        protected virtual void OnImport(AssetImportContext ctx, JObject token)
        {
            _baseScore = token["baseScore"]?.Value<float>() ?? 1.0f;
        }
        
        public static Lobe Import(AssetImportContext ctx, JToken token)
        {
            if (token is JValue)
            {
                var path = token.Value<string>();
                if (string.IsNullOrEmpty(path))
                    return null;
                
                var lobe = AssetDatabase.LoadAssetAtPath<Lobe>(path);
                if (lobe != null)
                    ctx.DependsOnSourceAsset(AssetDatabase.GetAssetPath(lobe));
                
                return lobe;
            }
            else if (token is JObject json)
            {
                var typeName = json["type"]?.Value<string>();
                if (string.IsNullOrEmpty(typeName))
                {
                    Debug.LogError("Lobe type is missing");
                    return null;
                }

                var name = json["name"]?.Value<string>() ?? typeName;
                if (string.IsNullOrEmpty(name))
                {
                    Debug.LogError("Lobe name is missing");
                    return null;
                }
                
                var lobe = LobeFactory.CreateInstance(ctx, typeName);
                if (lobe == null)
                    return null;

                lobe.name = name;
                lobe.OnImport(ctx, json);
                ctx.AddObjectToAsset(lobe.name, lobe);
                return lobe;
            }

            return null;
        }
#endif        
    }

    /// <summary>
    /// Defines an abstract Lobe that uses a pooled Think State 
    /// </summary>
    public abstract class Lobe<TThinkState> : Lobe where TThinkState : class, IThinkState, new()
    {
        private readonly List<TThinkState> _pool = new();

        /// <summary>
        /// Create an optional think state to be passed to think
        /// </summary>
        public override IThinkState AllocThinkState(Actor actor)
        {
            TThinkState state = default;

            if (_pool.Count > 0)
            {
                state = _pool[^1];
                _pool.RemoveAt(_pool.Count - 1);
            }
            else
                state = new TThinkState();

            state.OnAlloc(actor);

            return state;
        }

        /// <summary>
        /// Release a think state that was created with this brain
        /// </summary>
        public override void ReleaseThinkState(IThinkState state)
        {
            _pool.Add(state as TThinkState);
            state.OnRelease();
        }
    }
}
