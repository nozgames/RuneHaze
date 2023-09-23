/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

using NoZ;
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
        [Bind] private Image _waveTimeFill;
        [Bind] private VisualElement _waveTime;

        protected override void Bind()
        {
            base.Bind();

            Game.Instance.Paused += OnPaused;
            Game.Instance.Player.Health.Changed.AddListener(OnPlayerHealthChanged);
            WaveSystem.Instance.WaveTimeChanged += OnWaveTimeChanged;
            WaveSystem.Instance.WaveStarted += OnWaveStarted;
            InputModule.Instance.MenuButton += OnMenuButton;
        }
        
        protected override void OnDispose()
        {
            base.OnDispose();
            
            InputModule.Instance.MenuButton -= OnMenuButton;
            
            WaveSystem.Instance.WaveTimeChanged -= OnWaveTimeChanged;
            WaveSystem.Instance.WaveStarted -= OnWaveStarted;
            Game.Instance.Paused -= OnPaused;
        }

        private void OnPaused(bool paused)
        {
            this.SetDisplay(!paused);
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

        private void OnMenuButton()
        {
            if (Game.Instance.IsPaused)
                return;

            Game.Instance.IsPaused = true;
            Game.Instance.Root.Add(Instantiate<UIPause>());
        }
        
        private void OnWaveStarted(int obj)
        {
            _waveLabel.text = $"Wave {obj + 1}";
        }

        private void OnWaveTimeChanged(int remaining)
        {
            _waveTimeRemaining.text = remaining.ToString();

            var percentageRemaining = remaining / (float)WaveSystem.Instance.Current.Duration;
            _waveTimeFill.uv = new Rect(0, 0, 1.0f, percentageRemaining);
            _waveTimeFill.style.height = new StyleLength(new Length(percentageRemaining * 100.0f, LengthUnit.Percent));
            _waveTimeFill.MarkDirtyRepaint();
            
            if (remaining < 6)
            {
                if (remaining == 0)
                {
                    AudioManager.Instance.Play(Sounds.WaveComplete);
                    _waveTime.style.TweenScale(1.3f, 1.0f).Duration(0.75f).EaseOutCubic().Play();
                    _waveTime.style.TweenSequence()
                        .Element(_waveTime.style.TweenRotate(
                            new StyleRotate(new Rotate(new Angle(-30))),
                            new StyleRotate(new Rotate(new Angle(30))))
                            .PingPong()
                            .Duration(0.15f))
                        .Element(_waveTime.style.TweenRotate(
                                new StyleRotate(new Rotate(new Angle(-25))),
                                new StyleRotate(new Rotate(new Angle(25))))
                            .PingPong()
                            .Duration(0.2f))
                        .Element(_waveTime.style.TweenRotate(
                                new StyleRotate(new Rotate(new Angle(-20))),
                                new StyleRotate(new Rotate(new Angle(20))))
                            .PingPong()
                            .Duration(0.25f))
                        .Element(_waveTime.style.TweenRotate(
                                new StyleRotate(new Rotate(new Angle(-20))),
                                new StyleRotate(new Rotate(new Angle(0))))
                            .Duration(0.15f)
                            .EaseOutCubic())
                        .Play();
                }
                else
                {
                    AudioManager.Instance.Play(
                        Sounds.Tick,
                        volume: Mathf.Lerp(0.3f, 1.0f, 1.0f - remaining / 5.0f),
                        pitch: 1.0f);
                    _waveTime.style.TweenScale(1.3f, 1.0f).Duration(0.2f).EaseOutCubic().Play();
                }
            }
        }
    }
}
