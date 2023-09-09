/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

using UnityEngine;

namespace RuneHaze
{
    [CreateAssetMenu(menuName = "RuneHaze/Modules/CameraSystem")]
    public class CameraSystem : Module<CameraSystem>
    {
        [SerializeField] private float _pitch = 60.0f;
        [SerializeField] private float _distance = 10.0f;
        [SerializeField] private float _zoom = 5.0f;
        
        public Camera Camera { get; set; }

        public Bounds Bounds { get; set; } = new Bounds(Vector3.zero, Vector3.one * 10);
        
        public void Focus(Transform focus)
        {
            if (Camera == null)
                return;
            
            var rotation = Quaternion.Euler(_pitch, 0, 0);
            var cameraTransform = Camera.transform;
            cameraTransform.rotation = rotation;
            
            var focusPosition = ConstrainCameraToWorldBounds(Camera, focus.position, Bounds, _zoom);
            cameraTransform.position = focusPosition + rotation * Vector3.back * _distance;
            
        }
        
        private static Vector2 OrthographicSize(Camera camera, float size)
        {
            size = Mathf.Max(size, 1.0f);
            
            var screenAspect = Screen.width / (float)Screen.height;
            var cameraHeight = size * 2;
            return new Vector3(cameraHeight * screenAspect, cameraHeight);
        }
        
        private static Vector3 ConstrainCameraToWorldBounds(Camera camera, Vector3 position, Bounds worldBounds, float zoom)
        {
            var cameraPos = position;
            var cameraZoom = zoom;
            var cameraSize = OrthographicSize(camera, cameraZoom);
            if (cameraSize.x > worldBounds.size.x)
            {
                cameraZoom = Mathf.Max(1.0f, cameraZoom * worldBounds.size.x / cameraSize.x);
                cameraSize = OrthographicSize(camera, cameraZoom);
            }
            
            if (cameraSize.y > worldBounds.size.z)
            {
                cameraZoom = Mathf.Max(1.0f, cameraZoom * worldBounds.size.z / cameraSize.y);
                cameraSize = OrthographicSize(camera, cameraZoom);
            }

            cameraSize *= 0.5f;
            var minX = worldBounds.min.x + cameraSize.x;
            var maxX = worldBounds.max.x - cameraSize.x;
            var minZ = worldBounds.min.z + cameraSize.y;
            var maxZ = worldBounds.max.z - cameraSize.y;
            
            cameraPos.x = Mathf.Clamp(cameraPos.x, minX, maxX);
            cameraPos.z = Mathf.Clamp(cameraPos.z, minZ, maxZ);

            camera.orthographicSize = cameraZoom;
            
            return cameraPos;
        }
    }
}
