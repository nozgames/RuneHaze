/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

using UnityEngine;

namespace NoZ.RuneHaze
{
    public abstract class AbilityCondition : ScriptableObject
    {
        public abstract float CheckCondition(Actor source, Ability ability);
    }
}