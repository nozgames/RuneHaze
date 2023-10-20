/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

using UnityEngine;
using UnityEngine.InputSystem;

namespace NoZ.RuneHaze
{
    public class InputManager : Module<InputManager>, IModule
    {
        [Header("UX")]
        [SerializeField] private InputAction _pause = null;
        
        [Header("Player1")]
        [SerializeField] private InputAction _playerMove = null;
        [SerializeField] private InputAction _playerLook = null;
        [SerializeField] private InputAction _playerMoveKeyboard = null;
        [SerializeField] private InputAction _playerAttack = null;
        
        private bool _controller;

        public event System.Action<bool> ControllerChanged;
        public event System.Action PlayerAttack;
        public event System.Action MenuButton;

        public event System.Action<PlayerButton> OnPlayerButtonUp;
        public event System.Action<PlayerButton> OnPlayerButtonDown;
        
        public void Load()
        {
            _playerMove.Enable();
            _playerMove.performed += (ctx) =>
            {
                var value = ctx.ReadValue<Vector2>();
                // if (value.sqrMagnitude < 0.1f)
                //     return;
                
                IsUsingController = true;
                PlayerMove = new Vector3(value.x, 0, value.y);
            };
            _playerMove.canceled += (ctx) =>
            {
                if (IsUsingController)
                    PlayerMove = Vector3.zero;
            };

            _playerMoveKeyboard.Enable();
            _playerMoveKeyboard.performed += (ctx) =>
            {
                IsUsingController = false;
                
                var value = ctx.ReadValue<Vector2>();
                PlayerMove = new Vector3(value.x, 0, value.y);
            };
            _playerMoveKeyboard.canceled += (ctx) =>
            {
                if (!IsUsingController)
                    PlayerMove = Vector3.zero;
            };

            _pause.Enable();
            _pause.performed += (ctx) => MenuButton?.Invoke();
            
            _playerAttack.Enable();
            _playerAttack.performed += (ctx) =>
            {
                OnPlayerButtonDown?.Invoke(PlayerButton.Primary);
                OnPlayerButtonUp?.Invoke(PlayerButton.Primary);
                PlayerAttack?.Invoke();
            };
            
            _playerLook.Enable();
            _playerLook.performed += (ctx) => PlayerLook = ctx.ReadValue<Vector2>();
            _playerLook.canceled += (ctx) => PlayerLook = Vector2.zero;            
        }

        public void Unload()
        {
        }
        
        public bool IsUsingController
        {
            get => _controller;
            private set
            {
                if (_controller == value)
                    return;
                
                _controller = value;
                ControllerChanged?.Invoke(_controller);
            }
        }
        
        public Vector3 PlayerMove { get; private set; }
        
        public Vector2 PlayerLook { get; private set; }
        
        public Vector2 PlayerPointer => Mouse.current.position.ReadValue();
    }
}
