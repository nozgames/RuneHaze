/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

using UnityEngine;

namespace NoZ.RuneHaze
{
    public class Damage : MonoBehaviour
    {
        [SerializeField] private int _amount = 1;
        
        public void Do(Entity from, Entity to)
        {
            var health = to.Health;
            if (null == health)
                return;

            health.Damage(from, _amount);
        }
    }
}
