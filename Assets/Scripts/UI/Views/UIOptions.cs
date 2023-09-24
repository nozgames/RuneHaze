/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

using UnityEngine.UIElements;
using UnityEngine.Scripting;

namespace NoZ.RuneHaze.UI
{
    public class UIOptions : UIView
    {
        [Preserve]
        public new class UxmlFactory : UxmlViewFactory<UIOptions, UxmlTraits> { }    

        [Bind] private VisualElement _backButton;
        
        protected override void Bind()
        {
            base.Bind();
            
            _backButton.AddManipulator(new Clickable(OnBack));
        }

        private void OnBack(EventBase obj)
        {
            RemoveFromHierarchy();
        }
    }
}
