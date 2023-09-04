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
        public List<Avatar> _avatars;
        
        public override void Load()
        {
            _avatars = new();
        }

        public void Add(Avatar avatar)
        {
            _avatars.Add(avatar);
        }

        public void Remove(Avatar avatar)
        {
            _avatars.Remove(avatar);
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
                    var distanceSqr = positionDelta.sqrMagnitude;
                    var acceptableDistance = avatar.Radius + otherAvatar.Radius;
                    var acceptableDistanceSqr = acceptableDistance * acceptableDistance;
                    if (distanceSqr < acceptableDistanceSqr)
                    {
                        var moveDir = positionDelta.normalized;
                        if (moveDir.sqrMagnitude < 0.1f)
                            moveDir = Vector3.right; // new Vector3(Random.value - 0.5f, 0, Random.value - 0.5f).normalized;

                        move += moveDir * avatar.Speed * Time.deltaTime; 
                    }
                }

                avatar.transform.position += move;
            }
        }
    }
}
