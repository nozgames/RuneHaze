/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

namespace RuneHaze
{
    public class Health : MonoBehaviour
    {
        [SerializeField] private int _max = 100;

        public UnityEngine.Events.UnityEvent<Entity,int> Changed = new();
        [FormerlySerializedAs("TargetDied")] public UnityEngine.Events.UnityEvent<Entity> Death = new();

        public int Max => _max;
        
        public int Current { get; private set; }

        private void Awake()
        {
            Current = _max;
        }

        public void Damage(Entity from, int amount)
        {
            Assert.IsTrue(amount >= 0);
            
            amount = Mathf.Min(amount, Current);
            
            Current -= amount;
            
            Changed.Invoke(from, -amount);
            
            if (Current <= 0)
            {
                Current = 0;
                Death.Invoke(from);
            }
        }
        
        public void Heal(Entity from, int amount)
        {
            Assert.IsTrue(amount >= 0);
            
            amount = Mathf.Min(amount, _max-Current);
            Changed.Invoke(from, amount);
            
            Current += amount;
        }
    }
}
