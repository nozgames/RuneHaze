/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UIElements;

namespace RuneHaze.UI
{
    public class UIView : VisualElement, System.IDisposable
    {
        [Preserve]
        public new class UxmlFactory : UxmlViewFactory<UIView, UxmlTraits>  { }

        private UIView _parentView;
        private bool _hasGeometry;
        private bool _disposed;

        public bool IsDisposed => _disposed;

        /// <summary>
        /// Sort order of the view within the top level application.  Note that this value has
        /// no meaning when a view is not added directly to the UIApplication
        /// </summary>
        public virtual int SortOrder => 0;

        /// <summary>
        /// Returns true if Bind has been called on the UIView
        /// </summary>
        public bool IsBound { get; private set; }

        /// <summary>
        /// True if the view is currently being displayed
        /// </summary>
        public bool IsDisplayed { get; private set; }

        private void BindInternal()
        {
            if (IsBound)
                return;

            IsBound = true;

            // Bind all children first
            for (int childIndex = 0; childIndex < childCount; childIndex++)
                BindHierarchy(this[childIndex]);

            BindFields();
            Bind();

            // HACK HACK HACK:  This is a hack to fix a bug in 2022 that is causing
            // image custom styles to not repaint the image when they are changed.  This
            // code should be removed once it is determined that the bug has been fixed.
            foreach(var img in this.Query<Image>().Build())
            {
                img.RegisterCallback<CustomStyleResolvedEvent>(evt =>
                {
                    img.MarkDirtyRepaint();
                });
            }

            UpdateDisplay();
        }

        protected virtual void Bind() { }

        public static void BindHierarchy (VisualElement element)
        {
            if (element == null)
                return;

            // If the element is a view then the BindInternal method will handle
            // binding all of the children.
            if (element is UIView elementAsView)
            {
                elementAsView.BindInternal();
                return;
            }

            // Search through the hiearchy for UIView's and call bind on them
            for (int childIndex=0, childCount=element.childCount; childIndex < childCount; childIndex++)
                BindHierarchy(element[childIndex]);
        }

        protected override void ExecuteDefaultAction(EventBase evt)
        {
            base.ExecuteDefaultAction(evt);

            if (evt == null)
                return;

            if (evt.eventTypeId == EventBase<AttachToPanelEvent>.TypeId())
                OnAttachToPanel();
            else if (evt.eventTypeId == EventBase<DetachFromPanelEvent>.TypeId())
                OnDetachFromPanel(evt as DetachFromPanelEvent);
            else if (evt.eventTypeId == EventBase<GeometryChangedEvent>.TypeId())
                OnGeometryChanged(evt as GeometryChangedEvent);
            else if (evt.eventTypeId == EventBase<DisplayChangedEvent>.TypeId())
                OnDisplayChanged();
        }

        /// <summary>
        /// Instantiate a view of the given type using the singleton panel factory
        /// </summary>
        public static TView Instantiate<TView>() where TView : UIView => VisualTreeFactory.Instance.Instantiate<TView>();

        /// <summary>
        /// Instantiate a view using the given view type
        /// </summary>
        /// <param name="type">View type to instantiate</param>
        /// <returns>Instantiated view</returns>
        public static UIView Instantiate(System.Type type) => (UIView)VisualTreeFactory.Instance.Instantiate(type);


        public static void DisposeChildren(VisualElement element)
        {
            for (int childIndex = element.childCount - 1; childIndex >= 0; childIndex--)
            {
                var child = element[childIndex];
                if (child is UIView view)
                    view.Dispose();
                else
                {
                    DisposeChildren(child);

                    // Dispose of any disposable children
                    if (child is System.IDisposable disposable)
                        disposable.Dispose();
                }
            }
        }

        // TODO: we should cach the field that we need to set
        private void BindFields()
        {
            for (var type = GetType(); type != null && type != typeof(UIView); type = type.BaseType)
            {
                var fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
                foreach (var field in fields)
                {
                    var attr = field.GetCustomAttribute<BindAttribute>();
                    if (attr == null)
                        continue;

                    // Find the element with the matching name.  Note we cant use Q() here on ourself
                    // because we may pick up ourself if we have the same name.  Instead we search
                    // all children for the first child that matches.
                    var elementName = attr.Name ?? field.Name;
                    VisualElement element = null;
#if UNITY_EDITOR
                    int numElements = 0;
                    for (int i = 0, c = childCount; element == null && i < c; i++)
                    {
                        element = this[i].Q(elementName);
                        var query = this[i].Query<VisualElement>(elementName);
                        numElements += query.Build().Count();
                    }
                    
                    if (numElements > 1)
                        Debug.LogError($"{GetType()}: {numElements} elements with name '{elementName}'" +
                                       $"for bound field '{type.Name}.{field.Name}'\n");
#else
                    for (int i = 0, c = childCount; element == null && i < c; i++)
                        element = this[i].Q(elementName);
#endif

                    if ((element == null) && !attr.IsOptional)
                    {
                        Debug.LogError($"{GetType()}: missing named element '{name}.{elementName}' for field '{type.Name}.{field.Name}'");
                        continue;
                    }

                    // Make sure the field types match
                    if (element != null)
                    {
                        if (!field.FieldType.IsAssignableFrom(element.GetType()))
                        {
                            Debug.LogError($"{GetType()}: type mismatch of element '{elementName}' for field '{type.Name}.{field.Name}'");
                            continue;
                        }
                    }

                    field.SetValue(this, element);
                }
            }
        }

        protected virtual void OnGeometryChanged(GeometryChangedEvent evt)
        {
            _hasGeometry = true;
            UpdateDisplay();
        }

        protected virtual void OnParentDisplayChanged(DisplayChangedEvent evt)
        {
            UpdateDisplay();
        }

        protected virtual void OnDisplayChanged()
        {
            UpdateDisplay();
        }

        private void OnAttachToPanel()
        {
            Debug.Assert(_parentView == null);

            // Link up parent
            _parentView = GetFirstAncestorOfType<UIView>();
            if(_parentView != null)
                _parentView.RegisterCallback<DisplayChangedEvent>(OnParentDisplayChanged);

            UpdateDisplay();
        }

        private void OnDetachFromPanel(DetachFromPanelEvent evt)
        {
            if(_parentView != null)
            {
                _parentView.UnregisterCallback<DisplayChangedEvent>(OnParentDisplayChanged);
                _parentView = null;
            }

            UpdateDisplay(evt.destinationPanel);
        }

        private void UpdateDisplay()
        {
            UpdateDisplay(panel);
        }

        private void UpdateDisplay(IPanel destinationPanel)
        {
            // Displayed is the combined value of our displayed status and
            var displayed = !_disposed && IsBound && _hasGeometry && this.IsHierarchyDisplayed() && resolvedStyle.display != DisplayStyle.None && destinationPanel is not null;
            for (var p = _parentView; displayed && p != null; p = p._parentView)
                displayed = displayed && (p.IsHierarchyDisplayed() && p.resolvedStyle.display != DisplayStyle.None);

            if (IsDisplayed != displayed)
            {
                IsDisplayed = displayed;

                using var evt = DisplayChangedEvent.GetPooled();
                evt.IsDisplayed = IsDisplayed;
                evt.target = this;
                SendEvent(evt);

                if (displayed)
                    OnDisplayBegin();
                else
                    OnDisplayEnd();
            }
        }

        /// <summary>
        /// Called when a view is first displayed
        /// </summary>
        protected virtual void OnDisplayBegin() { }

        /// <summary>
        /// Called when a view is no longer being displayed
        /// </summary>
        protected virtual void OnDisplayEnd() { }

        /// <summary>
        /// Reset the view back to default state
        /// </summary>
        public virtual void Reset()
        {
            throw new System.NotImplementedException("Reset function has not been implemented for this view");
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;

            DisposeChildren(this);

            UpdateDisplay();

            RemoveFromHierarchy();

            OnDispose();
        }

        protected virtual void OnDispose()
        {
        }
    }

    /// <summary>
    /// Event sent when the display of a view is changed
    /// </summary>
    public class DisplayChangedEvent : EventBase<DisplayChangedEvent>
    {
        public bool IsDisplayed;
    }
}
