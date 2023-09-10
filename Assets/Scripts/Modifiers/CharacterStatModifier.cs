/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

namespace RuneHaze
{
    public class CharacterStatModifier : CharacterModifier
    {
        public CharacterStat Stat { get; }
        
        public CharacterStatModifier(Character character, CharacterStat stat, float amount, float duration) : base(character, amount, duration)
        {
            Stat = stat;
        }
    }
}
