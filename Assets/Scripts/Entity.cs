/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

using System;
using UnityEngine;

namespace RuneHaze
{
    public class Entity : MonoBehaviour, IDisposable
    {
        [SerializeField] private Health _health = null;

        public Health Health => _health;
        
        protected virtual void Start()
        {
        }
        
        protected virtual void OnEnable()
        {
        }

        protected virtual void OnDisable()
        {
        }

        public virtual void Dispose()
        {
            Destroy(gameObject);
        }
    }
}
