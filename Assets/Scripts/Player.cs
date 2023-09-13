/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

using UnityEngine;

namespace RuneHaze
{
    public class Player : Character
    {
        private InputModule _inputModule;
        private Vector3 _lastLookAt;
        
        protected override void Start()
        {
            _inputModule = InputModule.Instance;
            _inputModule.PlayerAttack += OnAttack;
            base.Start();
        }

        public override void Dispose()
        {
            base.Dispose();

            _inputModule.PlayerAttack -= OnAttack;
        }

        private void OnAttack()
        {
            IsAttacking = true;
        }

        protected override void Update()
        {
            _lastLookAt = LookAt;

            // No actions during global cooldown
            MovementDirection = GlobalCooldown > 0.0f ?
                Vector3.zero :
                _inputModule.PlayerMove.ToXZ();
            
            // Look towards the movement direction or the last look at direction
            LookAt = MovementDirection;
            if (LookAt.sqrMagnitude < 0.01f)
                LookAt = _lastLookAt;
            
            Target = GetClosestEnemy();

            base.Update();
        }

        protected void LateUpdate()
        {
            CameraSystem.Instance.Focus(transform);

            IsAttacking = false;
        }

        private Enemy GetClosestEnemy()
        {
            var position = transform.position;
            // var enemyCount = Physics.OverlapCapsuleNonAlloc(
            //     position,
            //     position + Vector3.up * 2.0f,
            //     100.0f,
            //     _colliders,
            //     _targetMask);
            
            var closestEnemy = default(Enemy);
            var closestDistanceSqr = float.MaxValue;
            foreach (var enemy in EnemySystem.Instance.Enemies)
            {
                //var enemy = _colliders[enemyIndex].GetComponent<Enemy>();
                // if (null == enemy)
                //     continue;
                
                var delta = enemy.transform.position - position;
                var dot = Vector3.Dot(delta.normalized, LookAt);
                var distanceSqr = delta.sqrMagnitude * -dot;
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

