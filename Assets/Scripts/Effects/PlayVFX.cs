/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

using UnityEngine;
using UnityEngine.VFX;

namespace NoZ.RuneHaze.Effects
{
    public class PlayVFX : EffectComponent
    {
        [SerializeField] private VisualEffectAsset _vfx = null;
        [SerializeField] private bool _attach = true;
        [SerializeField] private ActorSlot _slot = ActorSlot.None;
        [SerializeField] private Tag _tag = null;
        [SerializeField] private Vector3 _rotate;
        [SerializeField] private Vector3 _translate;
        [SerializeField] private Vector3 _scale = Vector3.one;

        public override Tag Tag => _tag;

        public override void Apply(EffectComponentContext context)
        {
            var slotTransform = context.Target.GetSlotTransform(_slot);
            if (slotTransform == null)
                context.Target.GetSlotTransform(ActorSlot.None);

            context.UserData = VFXManager.Instance.Play(_vfx, slotTransform, _attach, _translate, _rotate, _scale);
        }

        public override void Remove(EffectComponentContext context)
        {
            Release(context);
        }

        public override void Release(EffectComponentContext context)
        {
            if(context.UserData != null)
            {
                var vfx = context.UserData as VisualEffect;
                VFXManager.Instance.Release(context.UserData as VisualEffect);
                context.UserData = null;
            }                
        }
    }
}
