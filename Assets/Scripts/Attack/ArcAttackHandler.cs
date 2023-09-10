/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

using UnityEngine;
using UnityEngine.Assertions;

namespace RuneHaze
{
    public class ArcAttackHandler : AttackHandler
    {
        private static readonly Collider [] _colliders = new Collider[128];
        
        [SerializeField] private float _angle = 60.0f;

        private float _arcCos;
        
        private void Awake()
        {
            _arcCos = Mathf.Cos(_angle * Mathf.Deg2Rad * 0.5f);
        }
        
        public override void Do(Character attacker, Character target, float baseDamage)
        {
            Assert.IsNotNull(attacker);
            Assert.IsNotNull(target);
            
            var attackerPosition = attacker.transform.position;
            var targetDir = (target.transform.position - attackerPosition).normalized;
            var count = Physics.OverlapSphereNonAlloc(attackerPosition, 5.0f, _colliders, 1 << target.gameObject.layer);
            for (var i = 0; i < count; i++)
            {
                var colliderCharacter = _colliders[i].GetComponentInParent<Character>();
                if (colliderCharacter == null)
                    continue;
                
                var colliderCharacterDir = (colliderCharacter.transform.position - attackerPosition).normalized;
                if (Vector3.Dot(colliderCharacterDir, targetDir) > _arcCos)
                    continue;
                
                DoDamage(attacker, target, baseDamage);    
            }
        }
    }
}
