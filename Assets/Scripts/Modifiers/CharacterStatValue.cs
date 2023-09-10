/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

using UnityEngine;

namespace RuneHaze
{
    public class CharacterStatValue
    {
        private float _min;
        
        public CharacterStat Stat { get; private set; }

        public float BaseValue { get; }

        public float Min
        {
            get => _min;
            set => _min = Mathf.Max(_min, value);
        }
        
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
            Min = BaseValue;
        }

        public void UpdateValue()
        {
            Value = (Min + Add) * Multiply;
        }
    }
}
