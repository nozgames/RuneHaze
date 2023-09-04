/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

using UnityEngine;

namespace RuneHaze
{
    [CreateAssetMenu(menuName = "RuneHaze/Modules/WaveSystem")]
    public class WaveSystem : Module<WaveSystem>
    {
        [SerializeField] private float _waveStartDelay = 1.0f;
        
        public event System.Action<int> WaveTimeChanged;
        public event System.Action WaveComplete;
        public event System.Action WaveFailed;

        public Wave Current { get; private set; }
        
        private float _waveStartTime;
        private int _remainingTime;
        private float _waveSpawnInterval;
        private float _remainingTimeUntilNextSpawn;
        private int _waveSpawnCount;
        
        public void StartWave(int waveIndex)
        {
            var wave = ArenaSystem.Instance.Current.GetWave(waveIndex);
            if (wave == null)
                return;

            Current = wave;

            _waveStartTime = Time.time;
            _remainingTime = wave.Duration;

            _waveSpawnCount = wave.GetRandomSpawnCount();
            _waveSpawnInterval = (wave.Duration - _waveStartDelay) / _waveSpawnCount;
            _remainingTimeUntilNextSpawn = _waveStartDelay;
        }

        public void StopWave()
        {
            Current = null;
        }
        
        public void Update()
        {
            var elapsedTime = Time.time - _waveStartTime;
            var remainingTime = (int)(Current.Duration - elapsedTime);
            if (remainingTime != _remainingTime)
            {
                _remainingTime = remainingTime;
                WaveTimeChanged?.Invoke(_remainingTime);
                
                if (remainingTime <= 0)
                    WaveComplete?.Invoke();
            }
            
            _remainingTimeUntilNextSpawn -= Time.deltaTime;
            if (_remainingTimeUntilNextSpawn < 0)
            {
                _remainingTimeUntilNextSpawn += _waveSpawnInterval;

                var enemy = WaveSystem.Instance.Current.GetRandomEnemy();
                var position = ArenaSystem.Instance.GetRandomSpawnPosition(enemy);
                var playerLook = (Game.Instance.Player.transform.position - position).normalized; 
                ArenaSystem.Instance.InstantiateEntity(enemy, position, Quaternion.LookRotation(playerLook));
            }
        }
    }
}
