/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

using System;
using System.Collections.Generic;
using UnityEngine;
using NoZ.Animations;
using NoZ.Tweening;

namespace RuneHaze
{
    public class Character : Entity
    {
        [SerializeField] private float _speed = 10.0f;
        [SerializeField] private float _radius = 1.0f;
        [SerializeField] private float _rotationDampen = 0.05f;
        [SerializeField] private BlendedAnimationController _animationController = null;
        [SerializeField] private Animator _animator = null;
        
        [Header("Animations")]
        [SerializeField] private AnimationShader _idleAnimation;
        [SerializeField] private AnimationShader _runAnimation;
        [SerializeField] private AnimationShader _deathAnimation;
        [SerializeField] private AnimationShader _hitAnimation;
        
        [Header("Renderers")]
        [SerializeField] private Renderer[] _renderers;
        
        [Space]
        [SerializeField] private CharacterStat[] _stats = null;

        [Space]
        [SerializeField] private RuneFactory[] _defaultRunes = null;
        
        #region Events
        public event System.Action<Character> PreUpdateEvent;
        public event System.Action<Character> PostUpdateEvent;
        #endregion
        
        
        private List<Rune> _runes = new();
        private List<CharacterModifier> _modifiers = new();
        private Quaternion _rotation = Quaternion.identity;
        private Vector3 _rotationSmooth;
        private CharacterStatValue[] _statValues = null;
        
        public Animator Animator => _animator;
        
        public Character Target { get; protected set; }
        
        public Vector3 MovementDirection { get; protected set; }
        
        public Vector3 LookAt { get; protected set; }

        public bool IsMoving { get; private set; }
        
        public bool IsDead { get; private set; }

        public float Radius => _radius;
        public float Speed => _speed;

        public IEnumerable<Rune> Runes => _runes;

        protected override void Awake()
        {
            base.Awake();

            _statValues = new CharacterStatValue[_stats.Length];
            for (var i = 0; i < _stats.Length; i++)
                _statValues[i] = new CharacterStatValue(_stats[i]);
        }

        protected override void Start()
        {
            base.Start();

            foreach (var runeFactory in _defaultRunes)
                AddRune(runeFactory);
            
            _rotation = transform.rotation;
            PlayAnimation(_idleAnimation);
        }
        
        public CharacterStatValue GetStatValue(CharacterStat stat)
        {
            for (var i = 0; i < _stats.Length; i++)
                if (_stats[i] == stat)
                    return _statValues[i];
            
            return null;
        }
        
        public void AddRune(RuneFactory runeFactory)
        {
            _runes.Add(runeFactory.Create(this));
        }

        public void AddModifier(CharacterModifier modifier)
        {
            _modifiers.Add(modifier);
        }

        public void RemoveModifier(CharacterModifier modifier)
        {
            _modifiers.Remove(modifier);
        }
        
        protected virtual void Update()
        {
            if (IsDead)
                return;

            PreUpdateEvent?.Invoke(this);
            
            var movement = MovementDirection;
            transform.position = ArenaSystem.Instance.ConstrainPosition(transform.position + movement * _speed * Time.deltaTime, Radius);
            
            var moving = MovementDirection.sqrMagnitude > 0.01f;
            if (IsMoving != moving)
            {
                IsMoving = moving;
                // if (IsMoving)
                //     _animationController.Play(_runAnimation);
                // else
                //     _animationController.Play(_idleAnimation);
            }

            _animator.SetFloat("Speed", IsMoving ? _speed : 0.0f);

            if (LookAt.sqrMagnitude > 0.1f)
                _rotation = Quaternion.LookRotation(LookAt, Vector3.up);

            transform.rotation = transform.rotation.SmoothDamp(_rotation, ref _rotationSmooth, _rotationDampen);
            
            PostUpdateEvent?.Invoke(this);
        }
        
        public void PlayAnimation(AnimationShader animation, BlendedAnimationController.AnimationCompleteDelegate onComplete = null)
        {
            if (null == animation)
                return;
            
            //_animationController.Play(animation, onComplete: onComplete);
        }

        public void OnDamage(Entity source, int amount)
        {
            if (_hitAnimation != null)
                PlayAnimation(_hitAnimation, onComplete: () =>
                {
                    // if (IsMoving)
                    //     _animationController.Play(_runAnimation);
                    // else
                    //     _animationController.Play(_idleAnimation);                
                });
            
            if (_renderers != null)
            {
                var tween = Tween.Group(gameObject);
                foreach (var renderer in _renderers)
                {
                    var material = renderer.material;
                    //var flashId = Shader.PropertyToID("_Flash");
                    tween.Element(material.TweenFloat("_Flash", 1, 0).EaseOutExponential().Duration(0.2f));
                }

                tween.Play();
            }

        }

        public virtual void OnDeath(Entity source)
        {
            IsDead = true;

            GetComponent<Collider>().enabled = false;
            
            var direction = (transform.position - source.transform.position).normalized;

            PlayAnimation(_deathAnimation);

            Tween.Group(gameObject)
                .Element(transform.TweenLocalScale(0.0f).EaseInCubic().Duration(0.30f))
                .Element(transform.TweenPosition(transform.position + direction * 1.5f).EaseOutCubic().Duration(0.35f))
                .DestroyOnStop()
                .Play();
        }
    }
}
