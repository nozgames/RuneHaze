/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

using UnityEngine;

namespace NoZ.RuneHaze.Effects
{
    public class Print : EffectComponent
    {
        [SerializeField] private string _applyText;
        [SerializeField] private string _releaseText;
        [SerializeField] private string _removeText;

        public override void Apply(EffectComponentContext context)
        {
            if (!string.IsNullOrEmpty(_applyText))
                Debug.Log(_applyText);
        }

        public override void Remove(EffectComponentContext context)
        {
            if (!string.IsNullOrEmpty(_removeText))
                Debug.Log(_removeText);
        }

        public override void Release(EffectComponentContext context)
        {
            if (!string.IsNullOrEmpty(_releaseText))
                Debug.Log(_releaseText);
        }
    }
}
