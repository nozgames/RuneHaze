/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

using System;
using System.Collections.Generic;
using UnityEngine;

#if false
namespace NoZ.RuneHaze
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
        [SerializeField] private LayerMask _targetMask;
        
        [Space]
        [SerializeField] private CharacterModifierFactory[] _selfModifiers;
        
        [Space]
        [SerializeField] private CharacterModifierFactory[] _targetModifiers;
        
        public bool DoesRequireTarget => _requireTarget;

        public string Animation => _animation ?? "Attack";

        public float Range => _range;
        
        public float Angle => _angle;
        
        public float Damage => _damage;
        
        public float GlobalCooldown => _globalCooldown;

        public LayerMask TargetMask => _targetMask;
        
        public AttackShape Shape => _shape;
        
        public IEnumerable<CharacterModifierFactory> SelfModifiers => _selfModifiers ?? Array.Empty<CharacterModifierFactory>();
        
        public IEnumerable<CharacterModifierFactory> TargetModifiers => _targetModifiers ?? Array.Empty<CharacterModifierFactory>();
        
        public override CharacterModifier Create(Actor actor)
        {
            return new AttackModifier(actor, this);
        }
        
        public bool CheckRange(Actor actor, Actor target) =>
            Vector3.Distance(actor.transform.position, target.transform.position) < actor.GetStatValue(StatSystem.Instance.RangeStat).Value;
        
        public bool CheckCooldown(float timeSinceLastAttack) =>
            timeSinceLastAttack > _cooldown;
    }
}
#endif