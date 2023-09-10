/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

using UnityEngine;
using UnityEngine.Assertions;

namespace RuneHaze
{
    [CreateAssetMenu(menuName = "RuneHaze/Modules/AttackSystem")]
    public class AttackSystem : Module<AttackSystem>
    {
        [SerializeField] private CharacterStat _damageStat;
        
        public int CalculateDamage(Character attacker, Character target, float baseDamage)
        {
            Assert.IsNotNull(_damageStat);
            Assert.IsNotNull(attacker);
            
            var damage = attacker.GetStatValue(_damageStat).Value * baseDamage;
            return Mathf.Max((int)damage, 1);
        }
    }
}
