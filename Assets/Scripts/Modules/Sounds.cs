/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

using NoZ;
using UnityEngine;

namespace RuneHaze
{
    public class Sounds : Module<Sounds>, IModule
    {
        [SerializeField] private AudioShader _tick;
        [SerializeField] private AudioShader _waveComplete;

        public override string Category => "Assets";
        
        public static AudioShader Tick => Instance._tick;
        public static AudioShader WaveComplete => Instance._waveComplete;
        
        public void Load()
        {
        }

        public void Unload()
        {
        }
    }
}
