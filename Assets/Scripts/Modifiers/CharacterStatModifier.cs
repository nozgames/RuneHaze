/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

namespace NoZ.RuneHaze
{
#if false    
    public class CharacterStatModifier : CharacterModifier
    {
        private readonly float _multiply;
        private readonly float _add;
        private readonly CharacterStat _stat;
        
        public CharacterStatModifier(Actor actor, CharacterStat stat, float multiply, float add) : base(actor)
        {
            _stat = stat;
            _multiply = multiply;
            _add = add;
            Actor.UpdateStatsEvent += OnUpdateStats;
        }
        
        public override void Dispose()
        {
            Actor.UpdateStatsEvent -= OnUpdateStats;

            base.Dispose();
        }

        private void OnUpdateStats(Actor actor)
        {
            var value = actor.GetStatValue(_stat);
            if (value == null)
                return;
            
            value.Multiply += _multiply;
            value.Add += _add;
        }
    }
#endif
}
