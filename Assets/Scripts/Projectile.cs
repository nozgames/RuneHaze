/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

using UnityEngine;

namespace NoZ.RuneHaze
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float _speed = 10.0f;
        
        public LayerMask HitMask { get; set; }
        
        public Entity Owner { get; set; } 

        public UnityEngine.Events.UnityEvent<Entity,Entity> OnHit = new();

        private void Update()
        {
            transform.position += transform.forward * (_speed * Time.deltaTime);

            if (ArenaSystem.Instance.IsOutOfBounds(transform.position))
                Destroy(gameObject);   
        }
        
        private void OnCollisionEnter(Collision other)
        {
            var entity = other.collider.GetComponent<Entity>();
            if (null != entity)
                OnHit.Invoke(Owner, entity);

            Destroy(gameObject);
        }
    }
}
