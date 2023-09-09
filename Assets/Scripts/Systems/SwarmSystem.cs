/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

using System.Collections.Generic;
using UnityEngine;

namespace RuneHaze
{
    [CreateAssetMenu(menuName = "RuneHaze/Modules/SwarmSystem")]
    public class SwarmSystem : Module<SwarmSystem>
    {
        public List<Character> _avatars;
        
        public override void Load()
        {
            _avatars = new();
        }

        public void Add(Character character)
        {
            _avatars.Add(character);
        }

        public void Remove(Character character)
        {
            _avatars.Remove(character);
        }

        public void Update()
        {
            var avatarCount = _avatars.Count;
            for (var avatarIndex = 1; avatarIndex < avatarCount; avatarIndex++)
            {
                var avatar = _avatars[avatarIndex];
                var move = Vector3.zero;
                for (var otherIndex = 0; otherIndex < avatarIndex; otherIndex++)
                {
                    var otherAvatar = _avatars[otherIndex];
                    var positionDelta = avatar.transform.position - otherAvatar.transform.position;
                    var distance = positionDelta.magnitude;
                    var acceptableDistance = avatar.Radius + otherAvatar.Radius;
                    
                    if (distance < acceptableDistance)
                    {
                        var strength = 1.0f - (distance / acceptableDistance);
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
