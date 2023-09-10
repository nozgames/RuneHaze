/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

using UnityEngine;

namespace RuneHaze
{
    [CreateAssetMenu(menuName = "RuneHaze/Character/Stat")]
    public class CharacterStat : ScriptableObject
    {
        [SerializeField] private float _min = 0.0f;
        [SerializeField] private float _max = 0.0f;
        [SerializeField] private bool _percent = false;
        
        public float Min => _min;
        public float Max => _min;

        public bool IsPercent => _percent;
    }
}
