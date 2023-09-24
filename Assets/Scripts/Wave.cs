/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

using UnityEngine;

#if UNITY_EDITOR
using Newtonsoft.Json.Linq;
using Noz.RuneHaze.EditorUtilities;
using UnityEditor;
using UnityEditor.AssetImporters;
#endif

namespace NoZ.RuneHaze
{
    public class Wave : ScriptableObject, ISerializationCallbackReceiver
    {
        [System.Serializable]
        private class EnemySpawn
        {
            [SerializeField] public ActorDefinition Enemy;
            [SerializeField] public float Weight;
        }
        
        [Header("General")]
        [SerializeField] private int _duration = 1;
        
        [Header("Spawn")]
        [SerializeField] private int _spawnCountMin = 1;
        [SerializeField] private int _spawnCountMax = 1;

        [Header("Enemies")]
        [SerializeField] private EnemySpawn[] _enemies;

        public int Duration => _duration;
        
        private float _totalWeight;

        public int GetRandomSpawnCount() => Random.Range(_spawnCountMin, _spawnCountMax + 1);

        public ActorDefinition GetRandomEnemy()
        {
            var value = Random.Range(0, _totalWeight);
            foreach (var enemy in _enemies)
            {
                if (value < enemy.Weight)
                    return enemy.Enemy;
                
                value -= enemy.Weight;
            }

            return _enemies[^1].Enemy;
        }
        
        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
            if (_enemies == null)
                return;

            _totalWeight = 0;
            foreach (var enemy in _enemies)
                _totalWeight += enemy.Weight;
        }
        
#if UNITY_EDITOR
        public static Wave Import(AssetImportContext ctx, JToken token)
        {
            if (token is JValue)
            {
                var path = token.Value<string>();
                if (string.IsNullOrEmpty(path))
                    return null;
                
                var wave = AssetDatabase.LoadAssetAtPath<Wave>(path);
                ctx.DependsOnSourceAsset(AssetDatabase.GetAssetPath(wave));
                return wave;
            }
            else if (token is JObject json)
            {
                var wave = CreateInstance<Wave>();
                wave.name = "wave";
                ImportUtility.ImportProperties(ctx, wave, json);
                ctx.AddObjectToAsset(wave.name, wave);
                return wave;
            }

            return null;
        }
#endif
    }
}
