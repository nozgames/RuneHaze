/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

using UnityEngine;

namespace RuneHaze
{
    public class Player : Character
    {
        [SerializeField] private Projectile _projectilePrefab = null;
        [SerializeField] private float _projectileFireRate = 1.0f;
        [SerializeField] private float _projectileRadius = 5.0f;
        [SerializeField] private LayerMask _projectileMask = 0;
        
        private InputModule _inputModule = null;
        private readonly Collider[] _colliders = new Collider[32];
        private float _projectileFireTimer = 0.0f;
        
        protected override void Start()
        {
            _inputModule = InputModule.Instance;
            base.Start();
        }

        protected override void Update()
        {
            LookAt = MovementDirection = new Vector3(_inputModule.PlayerMove.x, 0, _inputModule.PlayerMove.y);

            var closestEnemy = GetClosestEnemy();
            var attack = false;
            if (closestEnemy != null)
            {
                var enemyDirection = closestEnemy.transform.position - transform.position;
                enemyDirection.y = 0;

                if (enemyDirection.sqrMagnitude < 4.0f)
                {
                    attack = true;
                    LookAt = enemyDirection;
                }

                enemyDirection = enemyDirection.normalized;
            }
            
            Target = closestEnemy;

            base.Update();
            
            _projectileFireTimer += Time.deltaTime;
            if (!attack || _projectileFireTimer < _projectileFireRate)
                return;
            
            Animator.SetTrigger("Attack");
            _projectileFireTimer = 0.0f;
            //
            // if (closestEnemy == null)
            //     return;
            
            
            //
            // _projectileFireTimer = 0.0f;
            // var projectile = Instantiate(
            //     _projectilePrefab,
            //     transform.position + Vector3.up * 0.5f,
            //     Quaternion.LookRotation((closestEnemy.transform.position - position).normalized));
            // projectile.Owner = this;
            // projectile.HitMask = _projectileMask;
        }

        protected void LateUpdate()
        {
            CameraSystem.Instance.Focus(transform);
        }

        private Enemy GetClosestEnemy()
        {
            var position = transform.position;
            var enemyCount = Physics.OverlapCapsuleNonAlloc(
                position,
                position + Vector3.up * 2.0f,
                _projectileRadius,
                _colliders,
                _projectileMask);
            
            var closestEnemy = default(Enemy);
            var closestDistanceSqr = float.MaxValue;
            for (var enemyIndex=0; enemyIndex<enemyCount; ++enemyIndex)
            {
                var enemy = _colliders[enemyIndex].GetComponent<Enemy>();
                if (null == enemy)
                    continue;

                var distanceSqr = (enemy.transform.position - position).sqrMagnitude;
                if (distanceSqr < closestDistanceSqr)
                {
                    closestDistanceSqr = distanceSqr;
                    closestEnemy = enemy;
                }
            }
            
            return closestEnemy;
        }
    }
}

