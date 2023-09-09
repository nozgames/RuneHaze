/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RuneHaze
{
    public class ModelPostProcessor : AssetPostprocessor
    {
        private void OnPostprocessMeshHierarchy(GameObject root)
        {
            if (root.name != "Armature")
                return;

            var rootBone = root.transform.Find("Root");
            if (rootBone == null)
                return;

            root.name = "Root";
            
            var children = new List<Transform>();
            for(var childIndex=0; childIndex<rootBone.transform.childCount; childIndex++)
                children.Add(rootBone.transform.GetChild(childIndex));
            
            foreach(var child in children)
                child.SetParent(root.transform, true);
            
            UnityEngine.Object.DestroyImmediate(rootBone.gameObject);
        }
    }
}
