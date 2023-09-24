/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

using UnityEngine;
using NoZ;

namespace NoZ.RuneHaze
{
    [CreateAssetMenu(menuName = "RuneHaze/Modules/ArenaSystem")]
    public class ArenaSystem : Module<ArenaSystem>, IModule
    {
        public Bounds Bounds { get; private set; }
        
        private GameObject _arenaInstance;
        
        public Arena Current { get; private set; }

        public void Load()
        {
        }

        public void Unload()
        {
        }
        
        public bool IsOutOfBounds(Vector3 position) =>
            !Bounds.Contains(position);

        public Vector3 GetRandomSpawnPosition(Actor enemy)
        {
            var x = Random.Range(-Bounds.min.x * 0.5f + enemy.Radius, Bounds.max.x * 0.5f + enemy.Radius); 
            var z = Random.Range(-Bounds.min.z * 0.5f + enemy.Radius, Bounds.max.z * 0.5f + enemy.Radius);
            return new Vector3(x, 0, z);
        }

        public Vector3 ConstrainPosition(Vector3 position, float radius)
        {
            var minX = Bounds.min.x + radius;
            var maxX = Bounds.max.x - radius;
            var minZ = Bounds.min.z + radius;
            var maxZ = Bounds.max.z - radius;
            
            position.x = Mathf.Clamp(position.x, minX, maxX);
            position.z = Mathf.Clamp(position.z, minZ, maxZ);

            return position;
        }
        
        public void LoadArena(Arena arena)
        {
            Current = arena;

            Bounds = new Bounds(Vector3.zero + Vector3.up * 5, new Vector3(arena.Size.x, 10, arena.Size.y));
            CameraManager.Instance.Bounds = new Bounds(Vector3.zero + Vector3.up * 5, new Vector3(arena.CameraSize.x, 10, arena.CameraSize.y));;
            _arenaInstance = arena.Instantiate();
        }

        public void UnloadArena()
        {
            if (null == Current)
                return;
            
            Destroy(_arenaInstance);
            _arenaInstance = null;

            Current = null;
        }
        
        public Entity InstantiateEntity(Entity prefab, Vector3 position, Quaternion rotation)
        {
            var instance = Instantiate(prefab, position, rotation);
            instance.transform.SetParent(_arenaInstance.transform);
            return instance;
        }
    }
}
