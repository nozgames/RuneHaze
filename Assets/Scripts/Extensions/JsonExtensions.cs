/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

using Newtonsoft.Json.Linq;

namespace NoZ.RuneHaze
{
    public static class JsonExtension
    {
        public static float GetFloat(this JObject json, string key, float defaultValue = 0.0f) =>
            json[key]?.ToObject<float>() ?? defaultValue;            
    }
}
