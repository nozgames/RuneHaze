/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

using UnityEngine;

namespace RuneHaze
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float _speed = 10.0f;
        
        public LayerMask HitMask { get; set; }

        private void Update()
        {
            transform.position += transform.forward * (_speed * Time.deltaTime);

            if (ArenaSystem.Instance.IsOutOfBounds(transform.position))
                Destroy(gameObject);   
        }
        
        private void OnCollisionEnter(Collision other)
        {
            var enemy = other.collider.GetComponent<Avatar>();
            if (null != enemy)
            {
                Debug.Log("Hit!");
            }

            Destroy(gameObject);
        }
    }
}
