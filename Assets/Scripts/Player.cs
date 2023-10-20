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
        private PlayerButton _button = PlayerButton.None;
        private bool _buttonPressed = false;
        private double _buttonRepeatTime = 0;

        public override Vector3 FacingDirection =>
            InputManager.Instance.IsUsingController 
                ? base.FacingDirection 
                : (ScreenToWorld(InputManager.Instance.PlayerLook) - transform.position).ZeroY().normalized;

        private Vector3 ScreenToWorld(Vector2 position)
        {
            var ray = CameraManager.Instance.Camera.ScreenPointToRay(position);
            new Plane(Vector3.up, Vector3.zero).Raycast(ray, out var distance);
            var worldPosition = ray.GetPoint(distance);
            return worldPosition;
        }
        
        protected override void OnInstantiate()
        {
            base.OnInstantiate();

            name = $"Player";

            // Controller = GameManager.Instance.Players.Where(p => p.OwnerClientId == OwnerClientId).FirstOrDefault();

            NavAgent.updateRotation = false;

            InputManager.Instance.OnPlayerButtonDown += OnButtonDown;
            InputManager.Instance.OnPlayerButtonUp += OnButtonUp;

            Signal.Dispatch(new PlayerSpawned { Player = this });
        }

        // protected override void OnDisable()
        // {
        //     base.OnDisable();
        //
        //     OnNetworkDespawn();
        // }
        //
        // public void OnNetworkDespawn()
        // {
        //     base.OnNetworkDespawn();
        //
        //     Signal.Dispatch(new PlayerDeSpawned { Player = this });
        //
        //     InputManager.Instance.OnPlayerButtonDown -= OnButtonDown;
        //     InputManager.Instance.OnPlayerButtonUp -= OnButtonUp;
        // }

        private void OnButtonDown (PlayerButton button)
        {
            if (IsBusy)
            {
                _pendingButton = _button;
            }
            else
            {
                _buttonRepeatTime = Time.timeAsDouble + _buttonRepeat;
                _buttonPressed = true;
                _button = button;
            }
        }

        private void OnButtonUp (PlayerButton button)
        {
            _buttonPressed = false;
        }
        
        protected override void OnAbilityEnd()
        {
            base.OnAbilityEnd();

            if (!_buttonPressed)
            {
                _button = PlayerButton.None;
                SetDestination(Destination.None);
            }
        }

        protected override void Update()
        {
            base.Update();

            State = ActorState.Active;
            
            SetDestination(new Destination(transform.position + InputManager.Instance.PlayerMove));
            
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
