/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

using System;

namespace RuneHaze
{
    /// <summary>
    /// A character modifier is a modifier that is applied to a character to change the character
    /// from it's default state.
    /// </summary>
    public abstract class CharacterModifier : IDisposable
    {
        public Character Character { get; }
        public float Amount { get; }
        public float Duration { get; }
        
        protected CharacterModifier(Character character, float amount, float duration)
        {
            Character = character;
            Amount = amount;
            Duration = duration;
        }

        public virtual void Dispose()
        {
            Character.RemoveModifier(this);
        }
    }
}
