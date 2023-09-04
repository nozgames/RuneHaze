/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

using UnityEngine;

namespace RuneHaze
{
    public abstract class Module : ScriptableObject
    {
        public abstract void LoadInstance();
        public abstract void UnloadInstance();
        
        public virtual void Load()
        {
        }

        public virtual void Unload()
        {
        }
    }

    public class Module<T> : Module where T : class
    {
        public static T Instance { get; private set; }
        
        public override void LoadInstance()
        {
            Instance = this as T;
            Load();
        }

        public override void UnloadInstance()
        {
            Unload();
            Instance = null;
        }
    }
}
