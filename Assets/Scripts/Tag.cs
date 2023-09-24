/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

using UnityEngine;

namespace NoZ.RuneHaze
{
    [CreateAssetMenu(menuName = "RuneHaze/Tag")]
    public class Tag : ScriptableObject
    {
        private void OnEnable()
        {
            Id = Shader.PropertyToID(name);
        }

        public int Id { get; private set; } = -1;
    }
}
