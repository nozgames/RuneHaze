/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

using UnityEngine;

namespace RuneHaze
{
    /// <summary>
    /// Modifies a character by adding an attack
    /// </summary>
    public class Attack : CharacterModifier
    {
        private float _timeSinceLastAttack;
        
        public AttackFactory Factory { get; }
        
        public Attack(Character character, AttackFactory factory, float amount, float duration=-1.0f) : base(character, amount, duration: duration)
        {
            Factory = factory;
            Character.PostUpdateEvent += OnPostUpdate;
        }
        
        public override void Dispose()
        {
            Character.PostUpdateEvent -= OnPostUpdate;
        }

        private void OnPostUpdate(Character character)
        {
            _timeSinceLastAttack += Time.deltaTime;
            
            if (Factory.DoesRequireTarget && character.Target == null ||
                !Factory.CheckCooldown(_timeSinceLastAttack) ||
                (Factory.DoesRequireTarget && !Factory.CheckRange(character, character.Target)))
                return;
            
            Factory.InstantiateAttack(character, character.Target, Amount);
            
            _timeSinceLastAttack = 0.0f;
        }
    }
}
