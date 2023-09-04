/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Scripting;

namespace RuneHaze.UI
{
    public class UIPlay : UIView
    {
        [Preserve]
        public new class UxmlFactory : UxmlViewFactory<UIPlay, UxmlTraits> { }    

        protected override void Bind()
        {
            base.Bind();
        }
    }
}
