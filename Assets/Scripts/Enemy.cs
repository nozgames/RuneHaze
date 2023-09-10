/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

using UnityEngine;

namespace RuneHaze
{
    public class Enemy : Character
    {
        [SerializeField] private float _attackRange = 1.0f;
        [SerializeField] private float _attackCooldown = 1.0f;

        private float _attackTimer = 0.0f;
        
        protected override void Start()
        {
            base.Start();
            transform.localScale = Vector3.one * Random.Range(0.9f, 1.1f);
        }
        
        protected override void OnEnable()
        {
            base.OnEnable();
            
            EnemySystem.Instance.Add(this);
        }

        protected override void OnDisable()
        {
            EnemySystem.Instance.Remove(this);
            
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

            _attackTimer += Time.deltaTime;
            if (_attackTimer < _attackCooldown)
                return;
            
            var distance = delta.sqrMagnitude;
            if (distance < _attackRange * _attackRange)
            {
                _attackTimer = 0.0f;
                player.Health.Damage(this, 1);
                Animator.SetTrigger("Attack");
            }
        }

        public override void OnDeath(Entity source)
        {
            EnemySystem.Instance.Remove(this);            
            
            base.OnDeath(source);
        }
    }
}
