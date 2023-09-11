/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

namespace RuneHaze
{
    public class CharacterStatModifier : CharacterModifier
    {
        private readonly float _multiply;
        private readonly float _add;
        private readonly CharacterStat _stat;
        
        public CharacterStatModifier(Character character, CharacterStat stat, float multiply, float add) : base(character)
        {
            _stat = stat;
            _multiply = multiply;
            _add = add;
            Character.UpdateStatsEvent += OnUpdateStats;
        }
        
        public override void Dispose()
        {
            Character.UpdateStatsEvent -= OnUpdateStats;

            base.Dispose();
        }

        private void OnUpdateStats(Character character)
        {
            var value = character.GetStatValue(_stat);
            if (value == null)
                return;
            
            value.Multiply += _multiply;
            value.Add += _add;
        }
    }
}
