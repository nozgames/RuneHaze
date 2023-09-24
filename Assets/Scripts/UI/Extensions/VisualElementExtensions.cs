/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

using UnityEngine.UIElements;

namespace NoZ.RuneHaze.UI
{
    public static class VisualElementExtensions
    {
        public static UxmlTraits GetTraits<TCreatedType, TTraits>(this UxmlFactory<TCreatedType, TTraits> factory)
            where TCreatedType : VisualElement, new() where TTraits : UxmlTraits, new() =>
            factory.m_Traits;
    
        /// <summary>
        /// Returns true if the element is being displayed in the heirarchy
        /// </summary>
        public static bool IsHierarchyDisplayed(this VisualElement element) => element.isHierarchyDisplayed;
        
        /// <summary>
        /// Returns true if the element is set to be displayed
        /// </summary>
        public static bool IsDisplayed(this VisualElement element) =>
            element.style.display != DisplayStyle.None;        
        
        /// <summary>
        /// Hide / Show an element
        /// </summary>
        public static void SetDisplay(this VisualElement element, bool display)
        {
            var localDisplay = display ? DisplayStyle.Flex : DisplayStyle.None;
            if (localDisplay == element.style.display)
                return;

            element.style.display = localDisplay;
        }
    }
}
