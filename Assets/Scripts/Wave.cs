/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

using UnityEngine;

namespace RuneHaze
{
    [CreateAssetMenu(menuName = "RuneHaze/Wave")]
    public class Wave : ScriptableObject, ISerializationCallbackReceiver
    {
        [System.Serializable]
        private class EnemySpawn
        {
            [HideInInspector]
            public string Name;
            public Enemy Enemy;
            public float Weight;
            
            public void OnValidate()
            {
                Name = Enemy ? $"{Enemy.name} ({Weight})" : "None";
            }
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

        public Enemy GetRandomEnemy()
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
        
        private void OnValidate()
        {
            if (_enemies == null) 
                return;
            
            foreach(var enemy in _enemies)
                enemy.OnValidate();
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
    }
}
