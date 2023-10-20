/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

using NoZ.Audio;
using UnityEngine;

namespace NoZ.RuneHaze.Effects
{
    public class PlaySound : EffectComponent
    {
        [SerializeField] private AudioShader _sound;

        public override void Apply(EffectComponentContext context)
        {
            AudioManager.Instance.PlaySound(_sound,context.Target.gameObject);
        }

        public override void Remove(EffectComponentContext context)
        {
        }

        public override void Release(EffectComponentContext context)
        {
        }
    }
}
