/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

using UnityEngine.UIElements;

namespace NoZ.RuneHaze.UI
{
    public class UxmlViewFactory<TView, TTraits> : UxmlFactory<TView, TTraits> where TView : UIView, new() where TTraits : UxmlTraits, new()
    {
        public override VisualElement Create(IUxmlAttributes bag, CreationContext cc)
        {
            // When the view is being created from within its own factory we need to just allocate the view
            TView view = null;
            if (typeof(UIView) == typeof(TView) || cc.visualTreeAsset.name == typeof(TView).Name || VisualTreeFactory.Instance == null)
            {
                view = new TView();
                this.GetTraits().Init(view, bag, cc);
            }
            // Otherwise we need to instantiate the view so we properly create the factory instance
            else
            {
                view = VisualTreeFactory.Instance.InstantiateWithoutBind<TView>();
                this.GetTraits().Init(view, bag, cc);
                UIView.BindHierarchy(view);
            }

            return view;
        }
    }
}
