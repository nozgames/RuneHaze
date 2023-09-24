/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

using UnityEngine;

namespace NoZ.RuneHaze
{
#if false    
    public class Enemy : Actor
    {
        private Vector3 _lastLookAt;
        private CharacterStatValue _range;

        protected override void Awake()
        {
            base.Awake();
            
            _range = GetStatValue(StatSystem.Instance.RangeStat);
        }
        
        protected override void Start()
        {
            base.Start();
            transform.localScale = Vector3.one * Random.Range(0.9f, 1.1f);
        }
        
        protected override void OnEnable()
        {
            base.OnEnable();
            
            EnemySystem.Instance.Add(this);
        }

        protected override void OnDisable()
        {
            EnemySystem.Instance.Remove(this);
            
            base.OnDisable();
        }
        
        protected override void Update()
        {
            var player = Game.Instance.Player;

            Target = player;
            
            var delta = (player.transform.position - transform.position);
            var lookDir = delta.normalized;
            MovementDirection = delta.sqrMagnitude > _range.Value * _range.Value 
                ? lookDir
                : Vector3.zero;
            LookAt = lookDir;
            
            base.Update();
        }

        public override void OnDeath(Entity source)
        {
            EnemySystem.Instance.Remove(this);            
            
            base.OnDeath(source);
        }
    }
#endif
}
