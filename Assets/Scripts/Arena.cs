/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

using UnityEngine;

namespace RuneHaze
{
    [CreateAssetMenu(menuName = "RuneHaze/Arena")]
    public class Arena : ScriptableObject
    {
        [SerializeField] private GameObject _prefab;
        [SerializeField] private Wave[] _waves;
        
        public int WaveCount => _waves.Length;

        public Wave GetWave(int waveIndex) => _waves[waveIndex];
        
        public GameObject Instantiate()
        {
            return Instantiate(_prefab);
        }
    }
}
