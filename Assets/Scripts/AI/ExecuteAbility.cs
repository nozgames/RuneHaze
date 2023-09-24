/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/


using UnityEngine.Scripting;

namespace NoZ.RuneHaze
{
    [Preserve]
    public class ExecuteAbility : Lobe<ExecuteAbility.ThinkState>
    {
        public class ThinkState : IThinkState
        {
            public int BestAbility = -1;

            public void OnAlloc(Actor actor) { }
            public void OnRelease() { }
        }

        public override float CalculateScore(Actor actor, IThinkState abstractState)
        {
            if (actor.IsBusy)
                return 0.0f;

            var state = (ThinkState)abstractState;
            var bestScore = float.MinValue;
            for (var i = 0; i < actor.Abilities.Length; i++)
            {
                if (actor.Abilities[i] == null)
                    continue;

                var score = actor.Abilities[i].CalculateScore(actor);
                if (score <= bestScore)
                    continue;

                bestScore = score;
                state.BestAbility = i;
            }

            return bestScore;
        }

        public override void Think(Actor actor, IThinkState abstractState)
        {
            var state = (ThinkState)abstractState;
            if (state.BestAbility < 0)
                return;

            var ability = actor.Abilities[state.BestAbility];
            actor.ExecuteAbility(ability);
            state.BestAbility = -1;
        }
    }
}