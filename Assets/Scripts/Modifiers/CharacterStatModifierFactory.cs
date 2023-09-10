/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

using UnityEngine;

namespace RuneHaze
{
    [CreateAssetMenu(menuName = "RuneHaze/Character/Stat Modifier")]
    public class CharacterStatModifierFactory : CharacterModifierFactory
    {
        [SerializeField] private CharacterStat _stat;
        
        public override CharacterModifier Create(Character character, float amount, float duration)
        {
            return new CharacterStatModifier(character, _stat, amount, duration);
        }
    }
}
