/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

using System;
using System.Collections.Generic;
using UnityEngine;

#if false
namespace NoZ.RuneHaze
{
    [CreateAssetMenu(menuName = "RuneHaze/Character/Rune")]
    public class Rune : ScriptableObject
    {
        [SerializeField] private string _displayName;
        [SerializeField] private CharacterModifierFactory[] _modifiers;
        
        public IEnumerable<CharacterModifierFactory> Modifiers => _modifiers ?? Array.Empty<CharacterModifierFactory>();
    }
}

#endif