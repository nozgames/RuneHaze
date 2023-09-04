/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

using System;
using NoZ.Animations;
using UnityEngine;

namespace RuneHaze
{
    public class Avatar : MonoBehaviour
    {
        [SerializeField] private float _speed = 10.0f;
        [SerializeField] private float _radius = 1.0f;
        [SerializeField] private float _rotationDampen = 0.05f;
        [SerializeField] private BlendedAnimationController _animationController = null;
        
        [Header("Animations")]
        [SerializeField] private AnimationShader _idleAnimation;
        [SerializeField] private AnimationShader _runAnimation;
        
        private Quaternion _rotation = Quaternion.identity;
        private Vector3 _rotationSmooth;
        
        public Vector3 MovementDirection { get; protected set; }
        
        public Vector3 LookAt { get; protected set; }

        public bool IsMoving { get; private set; }

        public float Radius => _radius;
        public float Speed => _speed;

        protected virtual void Start()
        {
            _rotation = transform.rotation;
            PlayAnimation(_idleAnimation);
        }

        protected virtual void OnEnable()
        {
        }

        protected virtual void OnDisable()
        {
        }
        
        protected virtual void Update()
        {
            var movement = MovementDirection;
            transform.position += movement * _speed * Time.deltaTime;
            
            var moving = MovementDirection.sqrMagnitude > 0.01f;
            if (IsMoving != moving)
            {
                IsMoving = moving;
                if (IsMoving)
                    _animationController.Play(_runAnimation);
                else
                    _animationController.Play(_idleAnimation);
            }

            if (LookAt.sqrMagnitude > 0.1f)
                _rotation = Quaternion.LookRotation(LookAt, Vector3.up);

            transform.rotation = transform.rotation.SmoothDamp(_rotation, ref _rotationSmooth, _rotationDampen);
        }
        
        public void PlayAnimation(AnimationShader animation)
        {
            _animationController.Play(animation);
        }
    }
}
