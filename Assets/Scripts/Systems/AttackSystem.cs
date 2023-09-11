/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace RuneHaze
{
    [CreateAssetMenu(menuName = "RuneHaze/Modules/AttackSystem")]
    public class AttackSystem : Module<AttackSystem>
    {
        private static readonly Collider [] _colliders = new Collider[128];
        
        public int CalculateDamage(Character attacker, Character target, float baseDamage)
        {
            Assert.IsNotNull(StatSystem.Instance);
            Assert.IsNotNull(StatSystem.Instance.DamageStat);
            Assert.IsNotNull(attacker);
            
            var damage = attacker.GetStatValue(StatSystem.Instance.DamageStat).Value * baseDamage;
            return Mathf.Max((int)damage, 1);
        }
        
        public void DoAttack(Character self, Character target, Attack attack)
        {
            self.PlayAnimation(attack.Animation);
            
            switch (attack.Shape)
            {
                case AttackShape.Self:
                    foreach (var arcTarget in GetTargetsInArc(self, target, attack.Range, attack.Angle))
                        DoAttackInternal(self, arcTarget, attack);
                    break;
                
                case AttackShape.Target:
                    foreach (var arcTarget in GetTargetsInArc(self, target, attack.Range, attack.Angle))
                        DoAttackInternal(self, arcTarget, attack);
                    break;
                
                case AttackShape.Arc:
                    foreach (var arcTarget in GetTargetsInArc(self, target, attack.Range, attack.Angle))
                        DoAttackInternal(self, arcTarget, attack);
                    break;
            }
        }

        private void DoAttackInternal(Character self, Character target, Attack attack)
        {
            Assert.IsNotNull(attack);

            UnityEngine.Debug.Log($"Attack: {self.name} => {target.name}");
            
            var damage = Instance.CalculateDamage(self, target, attack.Damage);
            if (damage <= 0)
                return;

            UnityEngine.Debug.Log($"Attack: {self.name} => {target.name} ({damage} damage");
            
            target.Health.Damage(self, damage);
        }

        private static IEnumerable<Character> GetTargetsInArc(Character self, Character target, float range, float angle)
        {
            var attackerPosition = self.transform.position;
            var targetDir = (target.transform.position - attackerPosition).normalized;
            var count = Physics.OverlapSphereNonAlloc(attackerPosition, range, _colliders, 1 << target.gameObject.layer);
            var arcCos = Mathf.Cos((90.0f - angle * 0.5f) * Mathf.Deg2Rad);
            for (var i = 0; i < count; i++)
            {
                var colliderCharacter = _colliders[i].GetComponentInParent<Character>();
                if (colliderCharacter == null)
                    continue;

                var colliderCharacterDir = (colliderCharacter.transform.position - attackerPosition).normalized;
                if (Vector3.Dot(colliderCharacterDir, targetDir) < arcCos)
                    continue;

                yield return colliderCharacter;
            }
        }
    }
}
