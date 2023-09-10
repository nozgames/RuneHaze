/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

using UnityEngine;

namespace RuneHaze
{
    /// <summary>
    /// Used to apply character modifiers to characters
    /// </summary>
    [System.Serializable]
    public class CharacterModifierApplicator
    {
        [HideInInspector]
        public string Name;
            
        public CharacterModifierFactory Factory;
        public float Amount = 1;
        public float Duration = -1.0f;
     
        public CharacterModifier Apply(Character character)
        {
            var modifier = Factory.Create(character, Amount, Duration);
            character.AddModifier(modifier);
            return modifier;
        }
        
        public void OnValidate()
        {
            Name = Factory ? $"{Factory.name} ({Amount})" : "None";
        }
    }
}
