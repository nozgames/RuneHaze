/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

using System.Collections.Generic;
using UnityEngine;

namespace NoZ.RuneHaze
{
    [CreateAssetMenu(menuName = "RuneHaze/Modules/EnemySystem")]
    public class EnemySystem : Module<EnemySystem>, IModule
    {
        private List<Actor> _enemies;
        
        public IEnumerable<Actor> Enemies => _enemies;
        
        public void Load()
        {
            _enemies = new();
        }

        public void Unload()
        {
        }

        public void Add(Actor character)
        {
            _enemies.Add(character);
        }

        public void Remove(Actor character)
        {
            _enemies.Remove(character);
        }

        public void Update()
        {
            var avatarCount = _enemies.Count;
            for (var avatarIndex = 1; avatarIndex < avatarCount; avatarIndex++)
            {
                var avatar = _enemies[avatarIndex];
                var move = Vector3.zero;
                for (var otherIndex = 0; otherIndex < avatarIndex; otherIndex++)
                {
                    var otherAvatar = _enemies[otherIndex];
                    var positionDelta = avatar.transform.position - otherAvatar.transform.position;
                    var distance = positionDelta.magnitude;
                    var acceptableDistance = avatar.Radius + otherAvatar.Radius;
                    
                    if (distance < acceptableDistance)
                    {
                        var strength = Mathf.Max(1.0f - (distance / acceptableDistance), 0.2f);
                        var moveDir = positionDelta.normalized;
                        if (moveDir.sqrMagnitude < 0.1f)
                            moveDir = Vector3.right; // new Vector3(Random.value - 0.5f, 0, Random.value - 0.5f).normalized;

                        move += moveDir * avatar.Speed * Time.deltaTime * strength; 
                    }
                }

                avatar.transform.position += move;
            }
        }
    }
}
