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
        
        private GameObject _arenaInstance;
        
        public Arena Current { get; private set; }
        
        public bool IsOutOfBounds(Vector3 position) =>
            position.x < -_arenaSize.x * 0.5f || position.x > _arenaSize.x * 0.5f ||
            position.z < -_arenaSize.y * 0.5f || position.z > _arenaSize.y * 0.5f;

        public Vector3 GetRandomSpawnPosition(Enemy enemy)
        {
            var x = Random.Range(-_arenaSize.x * 0.5f + enemy.Radius, _arenaSize.x * 0.5f + enemy.Radius); 
            var z = Random.Range(-_arenaSize.y * 0.5f + enemy.Radius, _arenaSize.y * 0.5f + enemy.Radius);
            return new Vector3(x, 0, z);
        }
        
        public void LoadArena(Arena arena)
        {
            Current = arena;
            
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
