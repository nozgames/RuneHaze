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
        public int CalculateDamage(Character attacker, Character target, float baseDamage)
        {
            Assert.IsNotNull(StatSystem.Instance);
            Assert.IsNotNull(StatSystem.Instance.DamageStat);
            Assert.IsNotNull(attacker);
            
            var damage = attacker.GetStatValue(StatSystem.Instance.DamageStat).Value * baseDamage;
            return Mathf.Max((int)damage, 1);
        }
    }
}
