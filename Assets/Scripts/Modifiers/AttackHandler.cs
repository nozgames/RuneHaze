/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

using UnityEngine;
using UnityEngine.Assertions;

namespace RuneHaze
{
    public abstract class AttackHandler : MonoBehaviour
    {
        [Tooltip("Modifiers to apply to self when the attack is executed")]
        [SerializeField] private CharacterModifierApplicator[] _selfModifiers;

        [Tooltip("Modifiers to apply to the target when the attack hits")]
        [SerializeField] private CharacterModifierApplicator[] _targetModifiers;
        
        public abstract void Do(Character attacker, Character target, float range, float baseDamage);
        
        protected void ApplyModifiers(Character attacker, Character target)
        {
            if (_targetModifiers != null)
                foreach (var modifier in _targetModifiers)
                    modifier.Apply(target);

            if (_selfModifiers != null)
                foreach (var modifier in _selfModifiers)
                    modifier.Apply(attacker);
        }

        protected void DoDamage(Character attack, Character target, float baseDamage)
        {
            Assert.IsNotNull(attack);
            
            var damage = AttackSystem.Instance.CalculateDamage(attack, target, baseDamage);
            if (damage <= 0)
                return;
            
            target.Health.Damage(attack, damage);
        }
    }
}
