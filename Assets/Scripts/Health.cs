/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

using UnityEngine;

namespace RuneHaze
{
    public class Health : MonoBehaviour
    {
        [SerializeField] private int _max = 100;

        public UnityEngine.Events.UnityEvent<int> OnDamage = new();
        public UnityEngine.Events.UnityEvent OnDeath = new();
        
        private int _current;
        
        public int Max => _max;
        public int Current => _current;

        private void Awake()
        {
            _current = _max;
        }

        public void Damage(int amount)
        {
            amount = Mathf.Min(amount, _current);
            OnDamage.Invoke(amount);
            
            _current -= amount;
            if (_current <= 0)
            {
                _current = 0;
                OnDeath.Invoke();
            }
        }
    }
}
