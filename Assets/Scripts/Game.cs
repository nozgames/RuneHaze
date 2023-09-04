/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

using System;
using UnityEngine;

namespace RuneHaze
{
    public class Game : MonoBehaviour
    {
        [SerializeField] private Module[] _modules;
        [SerializeField] private GameObject _playerTest;
        [SerializeField] private GameObject _enemyTest;
        
        public static Game Instance { get; private set; }

        public Player Player { get; private set; }
        
        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            foreach (var module in _modules)
                module.LoadInstance();

            Play();
        }

        private void Update()
        {
            SwarmSystem.Instance.Update();
        }

        private void OnApplicationQuit()
        {
            for (var i = _modules.Length - 1; i >= 0; i--)
                _modules[i].UnloadInstance();
        }

        private void Play()
        {
            Player = Instantiate(_playerTest).GetComponent<Player>();
            
            for (int i=0; i<6; i++)
                Instantiate(_enemyTest);
        }
    }
}
