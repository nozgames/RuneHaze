/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace RuneHaze
{
    [CreateAssetMenu(menuName = "RuneHaze/Modules/PostProc")]
    public class PostProcModule : Module<PostProcModule>
    {
        public enum VignetteChannel
        {
            Health = 0,
            Menu = 1
        }
        
        [System.Serializable]
        private class VignetteOverride
        {
            public float Intensity;
            public Color Color;
            
            public float RuntimeIntensity { get; set; }
        }
        
        [SerializeField] private VolumeProfile _postProcessProfile;
        [SerializeField] private VignetteOverride _vignetteHeath;
        [SerializeField] private VignetteOverride _vignetteMenu;
        
        private Vignette _vignette;
        private DepthOfField _dof;
        private ColorAdjustments _colorAdjustments;
        private VignetteOverride[] _vignetteOverrides;

        public bool IsDesaturated
        {
            get => _colorAdjustments.active;
            set => _colorAdjustments.active = value;
        }
        
        public bool IsBlurEnabled
        {
            get => _dof.active;
            set => _dof.active = value;
        }

        public void SetVignette(VignetteChannel channel, float intensity)
        {
            _vignetteOverrides[(int)channel].RuntimeIntensity = intensity;
            UpdateVignette();
        }

        private void UpdateVignette()
        {
            var vignetteIndex = _vignetteOverrides.Length - 1;
            for(; vignetteIndex >= 0; vignetteIndex--)
            {
                var vignetteOverride = _vignetteOverrides[vignetteIndex];
                var runtimeIntensity = vignetteOverride.RuntimeIntensity * vignetteOverride.Intensity;;
                if (runtimeIntensity <= float.Epsilon)
                    continue;

                _vignette.intensity.Override(Mathf.Min(runtimeIntensity, 1.0f));
                _vignette.color.Override(vignetteOverride.Color);
                return;
            }
            
            _vignette.intensity.Override(0);
        }

        public override void Load()
        {
            base.Load();

            if(!_postProcessProfile.TryGet(out _vignette))
                throw new System.NullReferenceException(nameof(_vignette));
            
            if(!_postProcessProfile.TryGet(out _dof))
                throw new System.NullReferenceException(nameof(_dof));

            if(!_postProcessProfile.TryGet(out _colorAdjustments))
                throw new System.NullReferenceException(nameof(_colorAdjustments));
            
            _vignetteOverrides = new[]
            {
                _vignetteHeath,
                _vignetteMenu
            };
            
            IsBlurEnabled = false;
            _vignette.intensity.Override(0);
            _colorAdjustments.active = false;
        }
        
        public override void Unload()
        {
            base.Unload();
            
            _vignette.intensity.Override(0);
            IsBlurEnabled = false;
            _colorAdjustments.active = false;
        }
    }
}
