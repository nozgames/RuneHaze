/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

using UnityEngine;

namespace RuneHaze
{
    [CreateAssetMenu(menuName = "RuneHaze/Modules/ArenaSystem")]
    public class ArenaSystem : Module<ArenaSystem>
    {
        [SerializeField] private Vector2 _arenaSize = new(10, 10);
        
        public bool IsOutOfBounds(Vector3 position) =>
            position.x < -_arenaSize.x || position.x > _arenaSize.x ||
            position.z < -_arenaSize.y || position.z > _arenaSize.y;
    }
}
