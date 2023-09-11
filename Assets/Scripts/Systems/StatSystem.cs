/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

using UnityEngine;

namespace RuneHaze
{
    [CreateAssetMenu(menuName = "RuneHaze/Modules/StatSystem")]
    public class StatSystem : Module<StatSystem>
    {
        [SerializeField] private CharacterStat _damage;
        [SerializeField] private CharacterStat _range;
        [SerializeField] private CharacterStat _attackSpeed;

        public CharacterStat DamageStat => _damage;
        
        public CharacterStat RangeStat => _range;
        
        public CharacterStat AttackSpeed => _attackSpeed;
    }
}
