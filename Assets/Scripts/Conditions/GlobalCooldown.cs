/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

namespace NoZ.RuneHaze.Conditions
{
    public class GlobalCooldown : AbilityCondition
    {
        public override float CheckCondition(Actor source, Ability ability) =>
            source.GlobalCooldownRemaining > 0.0f ? 0.0f : 1.0f;
    }
}
