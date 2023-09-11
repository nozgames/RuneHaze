/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

using UnityEngine;

namespace RuneHaze
{
    /// <summary>
    /// Adds an attack to the character as a character modifier and manages attack cooldown
    /// </summary>
    public class AttackModifier : CharacterModifier
    {
        private float _timeSinceLastAttack;
        private CharacterStatValue _attackSpeed;
        
        public Attack Attack { get; }
        
        public AttackModifier(Character character, Attack attack) : base(character)
        {
            Attack = attack;
            _attackSpeed = character.GetStatValue(StatSystem.Instance.AttackSpeed);
            Character.PostUpdateEvent += OnPostUpdate;
            Character.UpdateStatsEvent += OnUpdateStats;
        }

        public override void Dispose()
        {
            Character.PostUpdateEvent -= OnPostUpdate;
            Character.UpdateStatsEvent -= OnUpdateStats;
        }

        private void OnUpdateStats(Character character)
        {
            var range = character.GetStatValue(StatSystem.Instance.RangeStat);
            if (range != null)
                range.Min = Attack.Range;
        }

        private void OnPostUpdate(Character character)
        {
            _timeSinceLastAttack += Time.deltaTime * _attackSpeed.Value;
            
            if (Attack.DoesRequireTarget && character.Target == null ||
                !Attack.CheckCooldown(_timeSinceLastAttack) ||
                (Attack.DoesRequireTarget && !Attack.CheckRange(character, character.Target)))
                return;
            
            AttackSystem.Instance.DoAttack(character, character.Target, Attack);
            
            _timeSinceLastAttack = 0.0f;
        }
    }
}
