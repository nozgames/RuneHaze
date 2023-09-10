/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

using UnityEngine;

namespace RuneHaze
{
    [CreateAssetMenu(menuName = "RuneHaze/Character/Attack")]
    public class AttackFactory : CharacterModifierFactory
    {
        [SerializeField] private float _cooldown;
        [SerializeField] private float _range;
        [SerializeField] private string _animation;
        [SerializeField] private AttackHandler _prefab;
        [SerializeField] private bool _requireTarget = true;
        
        public bool DoesRequireTarget => _requireTarget;
        
        public override CharacterModifier Create(Character character, float amount, float duration)
        {
            return new Attack(character, this, amount);
        }
        
        public bool CheckRange(Character character, Character target) =>
            Vector3.Distance(character.transform.position, target.transform.position) < _range;
        
        public bool CheckCooldown(float timeSinceLastAttack) =>
            timeSinceLastAttack > _cooldown;

        public AttackHandler InstantiateAttack(Character character, Character target, float baseDamage)
        {
            // TODO animation
            
            // TODO pooling
            var handler = Instantiate(_prefab, character.transform.position, Quaternion.LookRotation(character.transform.forward));
            handler.Do(character, target, baseDamage);
            return handler;
        }
    }
}
