/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

using System;
using System.Collections.Generic;
using UnityEngine;

namespace RuneHaze
{
    [CreateAssetMenu(menuName = "RuneHaze/Character/Attack")]
    public class Attack : CharacterModifierFactory
    {
        [SerializeField] private float _cooldown;
        [SerializeField] private AttackShape _shape = AttackShape.Target;
        [SerializeField] private float _damage;
        [SerializeField] private float _globalCooldown = 0.5f;
        [SerializeField] private float _range;
        [SerializeField] private float _angle;
        [SerializeField] private string _animation;
        [SerializeField] private bool _requireTarget = true;

        [SerializeField] private CharacterModifierFactory[] _selfModifiers;
        [SerializeField] private CharacterModifierFactory[] _targetModifiers;
        
        public bool DoesRequireTarget => _requireTarget;

        public string Animation => _animation ?? "Attack";

        public float Range => _range;
        
        public float Angle => _angle;
        
        public float Damage => _damage;
        
        public float GlobalCooldown => _globalCooldown;
        
        public AttackShape Shape => _shape;
        
        public IEnumerable<CharacterModifierFactory> SelfModifiers => _selfModifiers ?? Array.Empty<CharacterModifierFactory>();
        
        public IEnumerable<CharacterModifierFactory> TargetModifiers => _targetModifiers ?? Array.Empty<CharacterModifierFactory>();
        
        public override CharacterModifier Create(Character character)
        {
            return new AttackModifier(character, this);
        }
        
        public bool CheckRange(Character character, Character target) =>
            Vector3.Distance(character.transform.position, target.transform.position) < character.GetStatValue(StatSystem.Instance.RangeStat).Value;
        
        public bool CheckCooldown(float timeSinceLastAttack) =>
            timeSinceLastAttack > _cooldown;
    }
}
