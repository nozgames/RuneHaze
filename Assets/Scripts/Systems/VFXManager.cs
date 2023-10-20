/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.VFX;

namespace NoZ.RuneHaze
{
    public class VFXManager : Module<VFXManager>, IModule
    {
        [SerializeField] private int _maxPoolSize = 128;

        private LinkedPool<VisualEffect> _pool;
        private GameObject _recycled;

        private List<VisualEffect> _playing = new List<VisualEffect>();


        public void Load()
        {
            _recycled = new GameObject("VFX");
            _recycled.hideFlags = HideFlags.DontSave;
            _recycled.SetActive(false);
            _pool = new LinkedPool<VisualEffect>(
                createFunc: PoolCreate,
                actionOnGet: PoolGet,
                actionOnRelease: PoolRelease,
                actionOnDestroy: PoolDestroy,
                maxSize: _maxPoolSize
            );
        }

        public void Unload()
        {
            _pool.Clear();
            _pool.Dispose();
            
            Destroy(_recycled);
            _recycled = null;
        }

        private void PoolDestroy(VisualEffect ve)
        {
            Destroy(ve.gameObject);
        }

        private void PoolRelease(VisualEffect ve)
        {
            //ve.gameObject.SetActive(false);
            ve.visualEffectAsset = null;
            ve.transform.SetParent(_recycled.transform);
        }

        private void PoolGet(VisualEffect ve)
        {
        }

        private VisualEffect PoolCreate()
        {
            var go = new GameObject();
            var ve = go.AddComponent<VisualEffect>();
            ve.initialEventName = "";
            //go.AddComponent<DestroyAfterVisualEffect>();
            go.transform.SetParent(_recycled.transform);
            go.name = "VFX";
            return ve;
        }

        /// <summary>
        /// Play a VisualEffect and attach it to the given transform
        /// </summary>
        public VisualEffect Play(VisualEffectAsset asset, Transform transform, bool attach) =>
            Play(asset, transform, attach, Vector3.zero, Vector3.zero, Vector3.one);

        /// <summary>
        /// Play a VisualEffect at the given <paramref name="position"/> and <paramref name="rotation"/>
        /// </summary>
        public VisualEffect Play(VisualEffectAsset asset, Transform transform, bool attach, Vector3 position, Vector3 rotation, Vector3 scale)
        {
            if (asset == null)
                return null;
            
            var ve = _pool.Get();
            var vet = ve.transform;
            
            vet.SetParent(transform);
            vet.localPosition = position;
            vet.localEulerAngles = rotation;
            vet.localScale = scale;
            ve.visualEffectAsset = asset;
            ve.GetComponent<Renderer>().rendererPriority = 1;
            
            if (!attach)
                vet.SetParent(null);
            
            ve.Play();
            return ve;
        }

        /// <summary>
        /// Release an effect back to the pool
        /// </summary>
        public void Release (VisualEffect ve)
        {
            ve.Stop();

            if (!ve.HasAnySystemAwake())
                _pool.Release(ve);
        }

        public void LateUpdate()
        {
            for (int i = _playing.Count - 1; i >= 0; i--)
            {
                if (!_playing[i].HasAnySystemAwake())
                {
                    _pool.Release(_playing[i]);
                    _playing.RemoveAt(i);
                }
            }
        }
    }
}
