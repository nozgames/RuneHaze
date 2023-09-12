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
        private float _timeSinceLookAt = 1.0f;
        private Vector3 _lastLookAt = Vector3.zero;
        
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
            _lastLookAt = LookAt;

            MovementDirection = new Vector3(_inputModule.PlayerMove.x, 0, _inputModule.PlayerMove.y);
            
            if (_inputModule.IsUsingController)
            {
                LookAt = new Vector3(_inputModule.PlayerLook.x, 0, _inputModule.PlayerLook.y);
            }
            else
            {
                // Get the position of the mouse in world space.
                var ray = CameraSystem.Instance.Camera.ScreenPointToRay(_inputModule.PlayerPointer);
                var plane = new Plane(Vector3.up, Vector3.zero);
                plane.Raycast(ray, out var distance);
                var point = ray.GetPoint(distance);
                point.y = 0;

                var look = point - transform.position;
                look.y = 0;
                LookAt = look.normalized;
            }

            if (LookAt.sqrMagnitude < 0.01f)
                LookAt = _lastLookAt; 
            
            Target = GetClosestEnemy();

#if false            
            var lookAtRealTarget = false;
            if (Target != null && Target.DistanceTo(this) < _range.Value)
            {
                var lookAt = Target.transform.position - transform.position;
                lookAt.y = 0;

                _lastLookAt = lookAt.normalized; // .transform.position;
                _timeSinceLookAt = 0.0f;
                lookAtRealTarget = true;
            }
            
            _timeSinceLookAt += Time.deltaTime;
            if (_timeSinceLookAt < 0.25f) 
            {
                // var lookAt = _lastLookAt - transform.position;
                // lookAt.y = 0;
                
//                if (!lookAtRealTarget && Vector3.Dot(lookAt, transform.forward) > 0.0f)
                    //LookAt = lookAt.normalized;
                    LookAt = _lastLookAt;
            }
#endif

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

