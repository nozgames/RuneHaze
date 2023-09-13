/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

using UnityEngine;

namespace RuneHaze
{
    public static class MathExtensions
    {
        public static Quaternion SmoothDamp(this Quaternion current, Quaternion target, ref Vector3 currentVelocity,
            float smoothTime)
        {
            var c = current.eulerAngles;
            var t = target.eulerAngles;
            return Quaternion.Euler(
                Mathf.SmoothDampAngle(c.x, t.x, ref currentVelocity.x, smoothTime),
                Mathf.SmoothDampAngle(c.y, t.y, ref currentVelocity.y, smoothTime),
                Mathf.SmoothDampAngle(c.z, t.z, ref currentVelocity.z, smoothTime)
            );
        }

        public static float Remap(this float value, Vector2 sourceRange, Vector2 targetRange)
        {
            var sourceMin = Mathf.Min(sourceRange.x, sourceRange.y);
            var sourceMax = Mathf.Max(sourceRange.x, sourceRange.y);
            var targetMin = Mathf.Min(targetRange.x, targetRange.y);
            var targetMax = Mathf.Max(targetRange.x, targetRange.y);

            value = Mathf.Clamp(value, sourceMin, sourceMax);

            if (sourceRange.x > sourceRange.y)
                return (value - sourceMin) / (sourceMax - sourceMin) * (targetMin - targetMax) + targetMax;

            return (value - sourceMin) / (sourceMax - sourceMin) * (targetMax - targetMin) + targetMin;
        }
        
        public static Vector3 ToXZ(this Vector2 v) => new (v.x, 0, v.y);
        
        public static Vector3 ZeroY(this Vector3 v) => new (v.x, 0, v.z);
    }
}
