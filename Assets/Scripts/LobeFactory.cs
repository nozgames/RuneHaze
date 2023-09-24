/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace NoZ.RuneHaze
{
    public static class LobeFactory
    {
        private static readonly Dictionary<string, Type> s_lobeTypes = new(); 
        
        static LobeFactory()
        {
            var lobeTypes = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => typeof(Lobe).IsAssignableFrom(t))
                .ToArray();

            foreach (var type in lobeTypes)
                s_lobeTypes[type.Name] = type;
        }

        public static Lobe CreateInstance(AssetImportContext ctx, string typeName)
        {
            if (!s_lobeTypes.TryGetValue(typeName, out var type))
                return null;

            return (Lobe)ScriptableObject.CreateInstance(type);
        }        
    }
}

#endif