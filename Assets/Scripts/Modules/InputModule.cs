/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

using UnityEngine;
using UnityEngine.InputSystem;

namespace RuneHaze
{
    [CreateAssetMenu(menuName = "RuneHaze/Modules/Input")]
    public class InputModule : Module<InputModule>
    {
        [Header("Player")]
        [SerializeField] private InputAction _playerMove = null;
        [SerializeField] private InputAction _playerLook = null;
        [SerializeField] private InputAction _playerMoveKeyboard = null;

        private bool _controller;

        public event System.Action<bool> ControllerChanged;
        
        public bool IsUsingController
        {
            get => _controller;
            private set
            {
                if (_controller == value)
                    return;
                
                _controller = value;
                ControllerChanged?.Invoke(_controller);
                UnityEngine.Debug.Log($"Controller: {_controller}");
            }
        }
        
        public Vector2 PlayerMove { get; private set; }
        
        public Vector2 PlayerLook { get; private set; }
        
        public Vector2 PlayerPointer => Mouse.current.position.ReadValue();
        
        public override void Load()
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
            
            _playerLook.Enable();
            _playerLook.performed += (ctx) => PlayerLook = ctx.ReadValue<Vector2>();
            _playerLook.canceled += (ctx) => PlayerLook = Vector2.zero;
        }
    }
}
