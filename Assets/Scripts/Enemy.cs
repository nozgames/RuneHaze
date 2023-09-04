/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

using UnityEngine;

namespace RuneHaze
{
    public class Enemy : Character
    {
        [SerializeField] private float _attackRange = 1.0f;
        
        protected override void OnEnable()
        {
            base.OnEnable();
            
            SwarmSystem.Instance.Add(this);
        }

        protected override void OnDisable()
        {
            SwarmSystem.Instance.Remove(this);
            
            base.OnDisable();
        }
        
        protected override void Update()
        {
            var player = Game.Instance.Player;
            var delta = (player.transform.position - transform.position);
            var lookDir = delta.normalized;
            MovementDirection = delta.sqrMagnitude > _attackRange * _attackRange 
                ? lookDir
                : Vector3.zero;
            LookAt = lookDir;
            
            base.Update();
        }
    }
}
