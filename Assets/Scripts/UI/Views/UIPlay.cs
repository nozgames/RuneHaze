/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Scripting;

namespace RuneHaze.UI
{
    public class UIPlay : UIView
    {
        [Preserve]
        public new class UxmlFactory : UxmlViewFactory<UIPlay, UxmlTraits> { }

        [Bind] private Label _waveTimeRemaining;

        protected override void Bind()
        {
            base.Bind();

            WaveSystem.Instance.WaveTimeChanged += OnWaveTimeChanged;
        }

        protected override void OnDispose()
        {
            base.OnDispose();
            
            WaveSystem.Instance.WaveTimeChanged -= OnWaveTimeChanged;
        }
        
        private void OnWaveTimeChanged(int remaining)
        {
            _waveTimeRemaining.text = remaining.ToString();
        }
    }
}
