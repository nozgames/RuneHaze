/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

using UnityEngine;

namespace RuneHaze
{
    public class Player : Character
    {
        [SerializeField] private LayerMask _targetMask;
        
        private InputModule _inputModule = null;
        private readonly Collider[] _colliders = new Collider[32];
        private CharacterStatValue _range;
        
        protected override void Awake()
        {
            base.Awake();
            
            _range = GetStatValue(StatSystem.Instance.RangeStat);
        }
        
        protected override void Start()
        {
            _inputModule = InputModule.Instance;
            base.Start();
        }

        protected override void Update()
        {
            LookAt = MovementDirection = new Vector3(_inputModule.PlayerMove.x, 0, _inputModule.PlayerMove.y);

            Target = GetClosestEnemy();

            base.Update();
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
                _range.Value,
                _colliders,
                _targetMask);
            
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

