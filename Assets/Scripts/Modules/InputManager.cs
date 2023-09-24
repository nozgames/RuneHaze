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

        public void Load()
        {
            _playerMove.Enable();
            _playerMove.performed += (ctx) =>
            {
                var value = ctx.ReadValue<Vector2>();
                // if (value.sqrMagnitude < 0.1f)
                //     return;
                
                IsUsingController = true;
                PlayerMove = value;
            };
            _playerMove.canceled += (ctx) =>
            {
                if (IsUsingController)
                    PlayerMove = Vector2.zero;
            };

            _playerMoveKeyboard.Enable();
            _playerMoveKeyboard.performed += (ctx) =>
            {
                IsUsingController = false;
                PlayerMove = ctx.ReadValue<Vector2>();
            };
            _playerMoveKeyboard.canceled += (ctx) =>
            {
                if (!IsUsingController)
                    PlayerMove = Vector2.zero;
            };

            _pause.Enable();
            _pause.performed += (ctx) => MenuButton?.Invoke();
            
            _playerAttack.Enable();
            _playerAttack.performed += (ctx) => PlayerAttack?.Invoke();
            
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
        
        public Vector2 PlayerMove { get; private set; }
        
        public Vector2 PlayerLook { get; private set; }
        
        public Vector2 PlayerPointer => Mouse.current.position.ReadValue();
    }
}
