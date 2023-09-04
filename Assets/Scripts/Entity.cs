/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

using UnityEngine;

namespace RuneHaze
{
    public class Entity : MonoBehaviour
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
    }
}
