/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

#if UNITY_EDITOR

using System;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json.Linq;
using NoZ.Audio;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.VFX;

namespace Noz.RuneHaze.EditorUtilities
{
    public static class ImportUtility
    {
        public static GameObject ImportPrefab(AssetImportContext ctx, JToken json)
        {
            if (json.ToObject<string>() is not { } prefabPath)
            {
                Debug.LogError("expected prefab path");
                return null;
            }
                
                
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            if (null == prefab)
                Debug.LogError($"prefab not found: {prefabPath}");

            ctx.DependsOnSourceAsset(prefabPath);
            
            return prefab;
        }

        private static object DeserializeField(AssetImportContext ctx, JToken json, object obj, Type fieldType, string fieldName)
        {
            if (fieldType == typeof(Vector2))
            {
                if (json is not JArray { Count: 2 } jsonArray)
                {
                    Debug.LogError($"{obj.GetType().Name}.{fieldName}: expected array of 2 numbers");
                    return null;
                }

                return new Vector2(jsonArray[0].ToObject<float>(), jsonArray[1].ToObject<float>());
            }

            if (fieldType == typeof(Vector3))
            {
                if (json is JArray { Count: 3 } jsonArray3)
                    return new Vector3(jsonArray3[0].ToObject<float>(), jsonArray3[1].ToObject<float>(), jsonArray3[2].ToObject<float>());

                if (json is JValue jsonValue)
                    return Vector3.one * jsonValue.ToObject<float>();
                
                Debug.LogError($"{obj.GetType().Name}.{fieldName}: invalid Vector3");
                return null;
            }
            
            // Arrays
            if (fieldType.IsArray)
            {
                
                if (json is not JArray jsonArray)
                {
                    Debug.LogError($"{obj.GetType().Name}.{fieldName}: expected array");
                    return null;
                }

                var elementType = fieldType.GetElementType();
                Assert.IsNotNull(elementType);
                var array = Array.CreateInstance(elementType, jsonArray.Count);
                for (var i = 0; i < jsonArray.Count; i++)
                    array.SetValue(DeserializeField(ctx, jsonArray[i], obj, elementType, fieldName), i);

                return array;
            }
            
            if (fieldType == typeof(AudioShader) && json.Type == JTokenType.String)
            {
                var clip = AssetDatabase.LoadAssetAtPath<AudioClip>(json.ToObject<string>());
                if (clip != null)
                {
                    var shader = AudioShader.Create(clip);
                    ctx.AddObjectToAsset(shader.name, shader);
                    return shader;
                }
            }

            
            if (fieldType == typeof(AnimationClip) || 
                fieldType == typeof(VisualEffectAsset) || 
                fieldType == typeof(AudioClip))
                return LoadAssetAtPath(fieldType, json.ToObject<string>());
            
            if (typeof(ScriptableObject).IsAssignableFrom(fieldType))
                return ImportScriptableObject(ctx, fieldType, json);
            
            if (fieldType == typeof(GameObject))
                return ImportPrefab(ctx, json);

            var isStruct = fieldType.IsValueType && !fieldType.IsPrimitive && !fieldType.IsEnum;
            if ((fieldType.IsClass || isStruct) && fieldType != typeof(string))
            {
                var classObject = Activator.CreateInstance(fieldType);
                ImportProperties(ctx, classObject, json as JObject);
                return classObject;
            }

            try
            {
                return json.ToObject(fieldType);
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.LogError($"{fieldType.Name}: {json.ToString()}");
                
                throw;
            }
        }
        
        private static void SetValue(AssetImportContext ctx, FieldInfo field, object obj, JToken token)
        {
            var value = DeserializeField(ctx, token, obj, field.FieldType, field.Name);
            if (value == null)
                return;
            
            try
            {
                field.SetValue(obj, value);
            }
            catch
            {
                Debug.LogError($"{obj.GetType().Name}.{field.Name}: invalid value type {value.GetType().Name}");
            }
        }

        private static string GetJsonName(string name)
        {
            name = name.StartsWith("_") ? name[1..] : name;
            if (char.IsUpper(name[0]))
                name = char.ToLower(name[0]) + name[1..];

            return name;
        }
        
        public static void ImportProperties(AssetImportContext ctx, object obj, JObject json)
        {
            for (var type = obj.GetType(); type != null; type = type.BaseType)
            {
                var fields = type
                    .GetFields(BindingFlags.Instance|BindingFlags.NonPublic|BindingFlags.Public)
                    .Where(f => f.IsPublic || f.GetCustomAttribute<SerializeField>() != null)
                    .ToArray();

                foreach (var field in fields)
                {
                    var token = json.GetValue(GetJsonName(field.Name));
                    if (token == null)
                        continue;

                    SetValue(ctx, field, obj, token);
                }
            }
        }

        public static ScriptableObject ImportScriptableObject(AssetImportContext ctx, Type type, JToken json)
        {
            if (json is JObject jsonObject)
            {
                var scriptableObjectType = type;
                if (scriptableObjectType.IsAbstract)
                {
                    var typeName = jsonObject["type"]?.ToObject<string>();
                    scriptableObjectType = TypeCache.GetTypesDerivedFrom(type)
                        .FirstOrDefault(t => t.Name == typeName);
                        
                    if(scriptableObjectType == null)    
                    {
                        Debug.LogError($"missing or invalid type name '{typeName}'");
                        return null;
                    }
                }
                    
                var scriptableObject = ScriptableObject.CreateInstance(scriptableObjectType);
                var name = json.ToObject<JObject>()["name"]?.ToObject<string>() ?? scriptableObject.GetType().Name;
                scriptableObject.name = name;
                ImportProperties(ctx, scriptableObject, json.ToObject<JObject>());
                ctx.AddObjectToAsset(name, scriptableObject);
                return scriptableObject;
            }
            else if (json.ToObject<string>() is string path)
            {
                var asset = (ScriptableObject)AssetDatabase.LoadAssetAtPath(path, type);
                if (null == asset)
                    Debug.LogError($"{type.Name} not found: {path}");

                ctx.DependsOnSourceAsset(AssetDatabase.GetAssetPath(asset));
                
                return asset;
            }
                
            Debug.LogError($"{type.Name} failed to import");
                
            return null;
        }

        private static UnityEngine.Object LoadAssetAtPath(Type type, string path)
        {
            var parts = path.Split(":");
            if (parts.Length == 1)
                return AssetDatabase.LoadAssetAtPath(path, type);
            
            var assets = AssetDatabase.LoadAllAssetsAtPath(parts[0]);
            if (assets == null || assets.Length == 0)
            {
                Debug.LogWarning($"{path}: not found");
                return null;
            }

            return assets.FirstOrDefault(a => a.name == parts[1] && type.IsInstanceOfType(a));
        }
    }
}

#endif