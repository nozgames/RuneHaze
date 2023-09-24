/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

using System;

namespace NoZ.RuneHaze.UI
{
    [AttributeUsage(AttributeTargets.Field)]
    public class BindAttribute : Attribute
    {
        public string Name { get; set; }
        public bool IsOptional { get; set; }
    }
}
