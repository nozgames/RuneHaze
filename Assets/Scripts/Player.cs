/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

using UnityEngine;

namespace NoZ.RuneHaze
{
    public enum PlayerButton
    {
        None,
        Primary,
        Secondary
    }

    public class Player : Actor
    {
        private static readonly int PlayerButtonCount = System.Enum.GetNames(typeof(PlayerButton)).Length;

        [Header("Player")]
        [SerializeField] private float _buttonRepeat = 0.25f;

        private PlayerButton _pendingButton;
        private Destination _pendingDestination;
        private PlayerButton _button = PlayerButton.Primary;
        private bool _buttonPressed = false;
        private double _buttonRepeatTime = 0;
        
        // public PlayerController Controller { get; private set; }

        public void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            name = $"Player";

            // Controller = GameManager.Instance.Players.Where(p => p.OwnerClientId == OwnerClientId).FirstOrDefault();

            NavAgent.updateRotation = false;

            // InputManager.Instance.OnPlayerButtonDown += OnButtonDown;
            // InputManager.Instance.OnPlayerButtonUp += OnButtonUp;

            Signal.Dispatch(new PlayerSpawned { Player = this });
        }

        public void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();

            Signal.Dispatch(new PlayerDeSpawned { Player = this });

            // InputManager.Instance.OnPlayerButtonDown -= OnButtonDown;
            // InputManager.Instance.OnPlayerButtonUp -= OnButtonUp;
        }

        private void OnButtonDown (PlayerButton button)
        {
            var look = InputManager.Instance.PlayerLook;
            var destination = new Destination(look);

            // if (Physics.Raycast(InputManager.Instance.PlayerLookRay, out var hit, 100.0f, -1))
            // {
            //     var actor = hit.collider.GetComponentInParent<Actor>();
            //     if (actor != null)
            //         destination = new Destination(actor);
            // }

            if(destination.Target == null)
                button = PlayerButton.None;

            OnButtonDown(button, destination);
        }

        private void OnButtonUp (PlayerButton button)
        {
            _buttonPressed = false;
        }

        private void OnButtonDown (PlayerButton button, Destination destination)
        {
            if (IsBusy)
            {
                _pendingButton = _button;
                _pendingDestination = destination;
            }
            else
            {
                _buttonRepeatTime = Time.timeAsDouble + _buttonRepeat;
                _buttonPressed = true;
                _button = button;
                SetDestination(destination);
            }
        }
        
        protected override void OnAbilityEnd()
        {
            base.OnAbilityEnd();

            if (_pendingDestination.IsValid)
            {
                OnButtonDown(_pendingButton, _pendingDestination);
                _pendingDestination = Destination.None;
                _pendingButton = PlayerButton.None;
            }

            if (!_buttonPressed)
            {
                _button = PlayerButton.None;
                SetDestination(Destination.None);
            }
        }

        protected override void Update()
        {
            base.Update();

            // If move button is held down then update the destination every repeat
            if (_buttonPressed && Destination.IsValid && !Destination.HasTarget && Time.timeAsDouble >= _buttonRepeatTime)
                OnButtonDown(PlayerButton.None, new Destination(InputManager.Instance.PlayerLook));
            else if (!IsBusy && _buttonPressed && Time.timeAsDouble >= _buttonRepeatTime)
                OnButtonDown(_button, Destination);

            Game.Instance.ListenAt(transform);

            CameraManager.Instance.Focus(transform);
        }

        /// <summary>
        /// Returns true if the given player button was pressed
        /// </summary>
        public bool WasButtonPressed (PlayerButton button) => !IsBusy && _button == button;

        protected override void OnHealthChanged()
        {
            base.OnHealthChanged();

            Health = Mathf.Max(Health, 1.0f);
        }
    }
}
