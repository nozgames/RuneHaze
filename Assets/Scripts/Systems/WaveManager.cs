/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

using System.Linq;
using UnityEngine;

namespace NoZ.RuneHaze
{
    public class WaveManager : Module<WaveManager>, IModule
    {
        [SerializeField] private float _waveStartDelay = 1.0f;
        
        public event System.Action<int> WaveTimeChanged;
        public event System.Action WaveComplete;
        public event System.Action WaveFailed;

        public event System.Action<int> WaveStarted;

        public Wave Current { get; private set; }
        
        private float _waveStartTime;
        private int _remainingTime;
        private float _waveSpawnInterval;
        private float _remainingTimeUntilNextSpawn;
        private int _waveSpawnCount;
        private int _waveIndex;
        
        public void Load()
        {
        }

        public void Unload()
        {
        }
        
        public void StartWave(int waveIndex)
        {
            var wave = ArenaManager.Instance.Current.GetWave(waveIndex);
            if (wave == null)
                return;

            _waveIndex = waveIndex;
            
            Current = wave;

            _waveStartTime = Time.time;
            _remainingTime = wave.Duration;

            _waveSpawnCount = wave.GetRandomSpawnCount();
            _waveSpawnInterval = (wave.Duration - _waveStartDelay) / _waveSpawnCount;
            _remainingTimeUntilNextSpawn = _waveStartDelay;
            
            WaveStarted?.Invoke(waveIndex);
        }

        public bool NextWave()
        {
            if (_waveIndex + 1 < ArenaManager.Instance.Current.WaveCount)
            {
                StartWave(_waveIndex + 1);
                return true;
            }

            return false;
        }

        public void StopWave()
        {
            Current = null;
        }
        
        public void Update()
        {
            if (Current == null)
                return;
            
            var elapsedTime = Time.time - _waveStartTime;
            var remainingTime = (int)(Current.Duration - elapsedTime);
            if (remainingTime != _remainingTime)
            {
                _remainingTime = remainingTime;
                WaveTimeChanged?.Invoke(_remainingTime);

                if (remainingTime <= 0)
                {
                    foreach (var enemy in EnemyManager.Instance.Enemies.ToArray())
                        enemy.Dispose();
                    
                    WaveComplete?.Invoke();
                }
            }
            
            _remainingTimeUntilNextSpawn -= Time.deltaTime;
            if (_remainingTimeUntilNextSpawn < 0)
            {
                _remainingTimeUntilNextSpawn += _waveSpawnInterval;

                var enemyDefinition = WaveManager.Instance.Current.GetRandomEnemy();
                var position = ArenaManager.Instance.GetRandomSpawnPosition(enemyDefinition);
                var playerLook = (Game.Instance.Player.transform.position - position).normalized; 
                ArenaManager.Instance.InstantiateActor(enemyDefinition, position, Quaternion.LookRotation(playerLook));
            }
        }
    }
}
