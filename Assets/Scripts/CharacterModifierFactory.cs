/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

using UnityEngine;

namespace NoZ.RuneHaze
{
    /// <summary>
    /// A character modifier factory is a scriptable object responsible for creating
    /// a character modifier at runtime. Character modifiers can register for many
    /// different lifecycle events on the character allowing significant
    /// customization of the character.
    /// </summary>
    public abstract class CharacterModifierFactory : ScriptableObject
    {
        [SerializeField] private float _duration = -1.0f;
        
        public abstract CharacterModifier Create(Actor actor);
    }
}
