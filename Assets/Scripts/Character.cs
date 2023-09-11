/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

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
        
        [Header("Renderers")]
        [SerializeField] private Renderer[] _renderers;
        
        [Space]
        [SerializeField] private CharacterStat[] _stats = null;

        [Space]
        [SerializeField] private Rune[] _defaultRunes = null;
        
        #region Events
        public event System.Action<Character> PreUpdateEvent;
        public event System.Action<Character> PostUpdateEvent;
        public event System.Action<Character> UpdateStatsEvent;
        public event System.Action<Character> PostUpdateStatsEvents;
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
        
        public IEnumerable<CharacterStat> Stats => _stats;

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

            UpdateStats();
        }
        
        public CharacterStatValue GetStatValue(CharacterStat stat)
        {
            for (var i = 0; i < _stats.Length; i++)
                if (_stats[i] == stat)
                    return _statValues[i];
            
            return null;
        }

        private void UpdateStats()
        {
            foreach (var stat in _statValues)
                stat.Reset();
            
            UpdateStatsEvent?.Invoke(this);
            
            foreach (var stat in _statValues)
                stat.UpdateValue();

            PostUpdateStatsEvents?.Invoke(this);
        }
        
        public void AddRune(Rune rune)
        {
            _runes.Add(rune);

            foreach (var modifier in rune.Modifiers)
                AddModifier(modifier.Create(this));
        }

        public void AddModifier(CharacterModifier modifier)
        {
            _modifiers.Add(modifier);

            UpdateStats();
        }

        public void RemoveModifier(CharacterModifier modifier)
        {
            _modifiers.Remove(modifier);
            
            UpdateStats();
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
            }

            _animator.SetFloat("Speed", IsMoving ? _speed : 0.0f);

            if (LookAt.sqrMagnitude > 0.1f)
                _rotation = Quaternion.LookRotation(LookAt, Vector3.up);

            transform.rotation = transform.rotation.SmoothDamp(_rotation, ref _rotationSmooth, _rotationDampen);
            
            PostUpdateEvent?.Invoke(this);
        }
        
        public void PlayAnimation(string animationName)
        {
            if (string.IsNullOrEmpty(animationName))
                return;

            _animator.SetTrigger(animationName);
        }

        public void OnDamage(Entity source, int amount)
        {
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

//            PlayAnimation(_deathAnimation);

            Tween.Group(gameObject)
                .Element(transform.TweenLocalScale(0.0f).EaseInCubic().Duration(0.30f))
                .Element(transform.TweenPosition(transform.position + direction * 1.5f).EaseOutCubic().Duration(0.35f))
                .DestroyOnStop()
                .Play();
        }

        public float DistanceTo(Character character)
        {
            var delta = transform.position - character.transform.position;
            delta.y = 0;
            return delta.magnitude;
        }
    }
}
