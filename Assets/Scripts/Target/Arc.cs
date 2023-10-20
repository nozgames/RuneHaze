/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

using UnityEngine;

namespace NoZ.RuneHaze.Targets
{
    public class Arc : TargetFinder
    {
        [SerializeField] private int _targetCount = 1;
        [SerializeField] private float _targetRange = 0.5f;
        [SerializeField] private float _targetArc = 45.0f;
        [SerializeField] private ActorTypeMask _targetMask = ActorTypeMask.None;
        
        private float _targetArcScore;
        private float _targetArcCos;

        private void OnEnable()
        {
            _targetArcCos = Mathf.Cos(_targetArc * Mathf.Deg2Rad);
            _targetArcScore = 1.0f / (1.0f - _targetArcCos);
        }

        protected override void AddTargets (Actor source)
        {
            var count = Physics.OverlapSphereNonAlloc(source.transform.position, _targetRange, s_colliders, _targetMask.ToLayerMask());
            var forward = source.FacingDirection;
            for (var i = 0; i < _targetCount && count> 0; i++)
            {
                var bestScore = float.MaxValue;
                var bestTarget = (Actor)null;
                var bestIndex = 0;

                for (var j = 0; j < count; j++)
                {
                    var target = s_colliders[j].GetComponentInParent<Actor>();
                    if (target == null || target == source || target.IsDead)
                        continue;

                    var delta = (target.transform.position - source.transform.position).ZeroY();
                    var dot = Vector3.Dot(forward, delta.normalized);
                    if (dot < _targetArcCos)
                        continue;

                    var score = ((1.0f - dot) + 0.1f) * _targetArcScore * (delta.magnitude / _targetRange);
                    if (score < bestScore)
                    {
                        bestScore = score;
                        bestTarget = target;
                        bestIndex = j;
                    }
                }

                if (bestTarget != null)
                {
                    Add(bestTarget);
                    s_colliders[bestIndex] = s_colliders[count - 1];
                    count--;
                }
            }

            for (var j = 0; j < count; j++) s_colliders[j] = null;
        }
    }
}
