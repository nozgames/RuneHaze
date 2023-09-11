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
        [SerializeField] private float _multiply = 0.0f;
        [SerializeField] private float _add = 0.0f;
        
        public override CharacterModifier Create(Character character)
        {
            return new CharacterStatModifier(character, _stat, _multiply, _add);
        }
    }
}
