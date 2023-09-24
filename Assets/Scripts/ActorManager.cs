/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

using UnityEngine;

namespace NoZ.RuneHaze
{
    public class ActorManager : Module<ActorManager>, IModule
    {
        [SerializeField] private Effect _defaultActorEffect = null;
        [SerializeField] private NoZ.Animations.AnimationEvent _abilityBeginEvent = null;
        [SerializeField] private NoZ.Animations.AnimationEvent _abilityEndEvent = null;
        
        public Effect DefaultActorEffect => _defaultActorEffect;
        
        public NoZ.Animations.AnimationEvent AbilityBeginEvent => _abilityBeginEvent;
        public NoZ.Animations.AnimationEvent AbilityEndEvent => _abilityEndEvent;        
        
        public void Load()
        {
        }

        public void Unload()
        {
        }
    }
}
