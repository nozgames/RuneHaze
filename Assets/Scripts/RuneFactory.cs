/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

using System.Linq;
using UnityEngine;

namespace RuneHaze
{
    [CreateAssetMenu(menuName = "RuneHaze/Character/Rune")]
    public class RuneFactory : ScriptableObject
    {
        [SerializeField] private CharacterModifierApplicator[] _modifiers;
        
        public Rune Create(Character character)
        {
            return new Rune(character, _modifiers.Select(m => m.Apply(character)).ToArray());
        }
        
        private void OnValidate()
        {
            if (_modifiers == null) 
                return;
            
            foreach(var modifier in _modifiers)
                modifier.OnValidate();
        }
    }
}
