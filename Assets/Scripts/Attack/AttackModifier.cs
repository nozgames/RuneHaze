/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

using UnityEngine;

namespace NoZ.RuneHaze
{
    /// <summary>
    /// Adds an attack to the character as a character modifier and manages attack cooldown
    /// </summary>
    public class AttackModifier : CharacterModifier
    {
        private float _timeSinceLastAttack;
        private readonly CharacterStatValue _attackSpeed;
        
        public Attack Attack { get; }
        
        public AttackModifier(Actor actor, Attack attack) : base(actor)
        {
            Attack = attack;
            _attackSpeed = actor.GetStatValue(StatSystem.Instance.AttackSpeed);
            Actor.PostUpdateEvent += OnPostUpdate;
            Actor.UpdateStatsEvent += OnUpdateStats;
        }

        public override void Dispose()
        {
            Actor.PostUpdateEvent -= OnPostUpdate;
            Actor.UpdateStatsEvent -= OnUpdateStats;
        }

        private void OnUpdateStats(Actor actor)
        {
            var range = actor.GetStatValue(StatSystem.Instance.RangeStat);
            if (range != null)
                range.Min = Attack.Range;
        }

        private void OnPostUpdate(Actor actor)
        {
            _timeSinceLastAttack += Time.deltaTime * _attackSpeed.Value;
            
            if (Attack.GlobalCooldown > 0.0f && actor.GlobalCooldown > 0.0f)
                return;
            
            if (!actor.IsAttacking ||
                Attack.DoesRequireTarget && actor.Target == null ||
                !Attack.CheckCooldown(_timeSinceLastAttack) ||
                (Attack.DoesRequireTarget && !Attack.CheckRange(actor, actor.Target)))
                return;
            
            AttackSystem.Instance.DoAttack(actor, actor.Target, Attack);
            
            actor.GlobalCooldown = Attack.GlobalCooldown;
            
            _timeSinceLastAttack = 0.0f;
        }
    }
}
