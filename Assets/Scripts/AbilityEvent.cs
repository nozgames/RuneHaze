/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

using UnityEngine;

namespace NoZ.RuneHaze
{
    public class AbilityEvent : ScriptableObject
    {
        [SerializeField] private NoZ.Animations.AnimationEvent _event;
        [SerializeField] private Effect[] _effects = null;

        public NoZ.Animations.AnimationEvent Event
        {
            get => _event;
            set => _event = value;
        }

        public Effect[] Effects => _effects;
    }
}