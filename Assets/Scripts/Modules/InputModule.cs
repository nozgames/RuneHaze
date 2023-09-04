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
        
        public Vector2 PlayerMove { get; private set; }
        
        public override void Load()
        {
            _playerMove.Enable();
            _playerMove.performed += (ctx) => PlayerMove = ctx.ReadValue<Vector2>();
            _playerMove.canceled += (ctx) => PlayerMove = Vector2.zero;
        }
    }
}
