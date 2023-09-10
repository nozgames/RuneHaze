/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

using System;

namespace RuneHaze
{
    /// <summary>
    /// A run is a collection of character modifiers that are applied to the character while
    /// the rune is in the in the character's possession. 
    /// </summary>
    public class Rune : IDisposable
    {
        public Character Character { get; }
        public CharacterModifier[] Modifiers { get; }
        
        public Rune(Character character, CharacterModifier[] modifiers)
        {
            Character = character;
            Modifiers = modifiers;
        }
        
        public void Dispose()
        {
        }
    }
}
