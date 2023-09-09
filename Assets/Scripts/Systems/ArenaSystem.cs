/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

using UnityEngine;
using UnityEngine.Serialization;

namespace RuneHaze
{
    [CreateAssetMenu(menuName = "RuneHaze/Modules/ArenaSystem")]
    public class ArenaSystem : Module<ArenaSystem>
    {
        public Bounds Bounds { get; private set; }
        
        private GameObject _arenaInstance;
        
        public Arena Current { get; private set; }

        public bool IsOutOfBounds(Vector3 position) =>
            !Bounds.Contains(position);

        public Vector3 GetRandomSpawnPosition(Enemy enemy)
        {
            var x = Random.Range(-Bounds.min.x * 0.5f + enemy.Radius, Bounds.max.x * 0.5f + enemy.Radius); 
            var z = Random.Range(-Bounds.min.z * 0.5f + enemy.Radius, Bounds.max.z * 0.5f + enemy.Radius);
            return new Vector3(x, 0, z);
        }
        
        public void LoadArena(Arena arena)
        {
            Current = arena;

            Bounds = new Bounds(Vector3.zero + Vector3.up * 5, new Vector3(arena.Size.x, 10, arena.Size.y));
            CameraSystem.Instance.Bounds = Bounds;
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
        
        public void StartWave(int waveIndex)
        {
            var wave = Current.GetWave(waveIndex);
            if (wave == null)
                return;
            
            //wave.Start();
        }
        
        public Entity InstantiateEntity(Entity prefab, Vector3 position, Quaternion rotation)
        {
            var instance = Instantiate(prefab, position, rotation);
            instance.transform.SetParent(_arenaInstance.transform);
            return instance;
        }
    }
}
