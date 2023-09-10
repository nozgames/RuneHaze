/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

namespace RuneHaze
{
    public class CharacterStatValue
    {
        public CharacterStat Stat { get; private set; }

        public float BaseValue { get; }
        
        public float Value { get; private set; }
        
        public float Multiply { get; set; }
        
        public float Add { get; set; }
        
        public CharacterStatValue(CharacterStat stat)
        {
            Stat = stat;
            BaseValue = stat.Min;
            Reset();
            UpdateValue();
        }

        public void Reset()
        {
            Multiply = 1.0f;
            Add = 0.0f;
        }

        public void UpdateValue()
        {
            Value = (BaseValue + Add) * Multiply;
        }
    }
}
