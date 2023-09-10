/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

using NoZ.Tweening;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Scripting;

namespace RuneHaze.UI
{
    public class UIPlay : UIView
    {
        [Preserve]
        public new class UxmlFactory : UxmlViewFactory<UIPlay, UxmlTraits> { }

        [Bind] private Label _waveLabel;
        [Bind] private Label _waveTimeRemaining;
        [Bind] private Label _playerHealthLabel;
        [Bind] private VisualElement _playerHealthBarFill;
        [Bind] private VisualElement _playerHealthBarChange;
        

        protected override void Bind()
        {
            base.Bind();

            Game.Instance.Player.Health.Changed.AddListener(OnPlayerHealthChanged);
            WaveSystem.Instance.WaveTimeChanged += OnWaveTimeChanged;
            WaveSystem.Instance.WaveStarted += OnWaveStarted;
        }

        private void OnPlayerHealthChanged(Entity attacker, int amount)
        {
            var player = Game.Instance.Player;
            _playerHealthLabel.text = $"{player.Health.Current} / {player.Health.Max}";

            var healthPercent = player.Health.Current / (float)player.Health.Max * 100.0f;
            _playerHealthBarFill.style.width = new StyleLength(
                new Length(healthPercent, LengthUnit.Percent));
            _playerHealthBarChange.style.left = new StyleLength(
                new Length(healthPercent, LengthUnit.Percent));

            var changeAmount = Mathf.Max(-amount, 0.0f);
            _playerHealthBarChange.style.width = new StyleLength(
                new Length(changeAmount / player.Health.Max * 100.0f, LengthUnit.Percent));
            Tween.Stop(_playerHealthBarChange.style);
            _playerHealthBarChange.style.TweenOpacity(1.0f, 0.0f).EaseInExponential().Duration(0.4f).Play();
        }

        protected override void OnDispose()
        {
            base.OnDispose();
            
            WaveSystem.Instance.WaveTimeChanged -= OnWaveTimeChanged;
            WaveSystem.Instance.WaveStarted -= OnWaveStarted;
        }

        private void OnWaveStarted(int obj)
        {
            _waveLabel.text = $"Wave {obj + 1}";
        }

        private void OnWaveTimeChanged(int remaining)
        {
            _waveTimeRemaining.text = remaining.ToString();
        }
    }
}
