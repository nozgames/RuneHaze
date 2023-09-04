/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

using UnityEngine;

namespace RuneHaze
{
    public class Damage : MonoBehaviour
    {
        [SerializeField] private int _amount = 1;
        
        public void DoDamage(Entity entity)
        {
            var health = entity.Health;
            if (null == health)
                return;

            health.Damage(_amount);
        }
    }
}
