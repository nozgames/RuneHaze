/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

using System;

#if false
namespace NoZ.RuneHaze
{
    /// <summary>
    /// A character modifier is a modifier that is applied to a character to change the character
    /// from it's default state.
    /// </summary>
    public abstract class CharacterModifier : IDisposable
    {
        public Actor Actor { get; }
        
        protected CharacterModifier(Actor actor)
        {
            Actor = actor;
        }

        public virtual void Dispose()
        {
            Actor.RemoveModifier(this);
        }
    }
}

#endif