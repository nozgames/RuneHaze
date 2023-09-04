/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace RuneHaze.UI
{
    public class VisualTreeFactory : ScriptableObject
    {
        [System.Serializable]
        public class Entry
        {
            public VisualTreeAsset Asset;
            public StyleSheet[] Styles;
        }

        [SerializeField] private VisualTreeFactory[] _inherit;
        [SerializeField] private Entry[] _entries;

        /// <summary>
        /// Get/Set the top level visual tree factory
        /// </summary>
        public static VisualTreeFactory Instance { get; set; }

        public Dictionary<string, Entry> _entriesByName;

        /// <summary>
        /// Instantiate a view of the given type <typeparamref name="TElement"/>
        /// </summary>
        public TElement Instantiate<TElement>() where TElement : VisualElement =>
            Instantiate(typeof(TElement), typeof(TElement).Name) as TElement;

        /// <summary>
        /// Instantiate a view of the given type <typeparamref name="TElement"/> named <paramref name="name"/>
        /// </summary>
        public TElement Instantiate<TElement>(string name) where TElement : VisualElement =>
            Instantiate(typeof(TElement), name) as TElement;

        /// <summary>
        /// Instantiate a view of the given type <typeparamref name="TElement"/> and do not perform the bind operation
        /// </summary>
        public TElement InstantiateWithoutBind<TElement>() where TElement : VisualElement =>
            Instantiate(typeof(TElement), typeof(TElement).Name, bind:false) as TElement;

        /// <summary>
        /// Instantiate a view of the given <paramref name="type"/>
        /// </summary>
        public VisualElement Instantiate(System.Type type) =>
            Instantiate(type, type.Name);

        /// <summary>
        /// Instantiate a view of the given <paramref name="type"/> named <paramref name="name"/>
        /// </summary>
        public VisualElement Instantiate(System.Type type, string name) =>
            Instantiate(type, name, true);

        private VisualElement Instantiate(System.Type type, string name, bool bind)
        {
            var entry = FindEntry(name);
            if (entry == null)
                throw new System.ArgumentException($"Missing factory for '{name}");

            return Instantiate(type, entry.Asset, entry.Styles, bind);
        }

        public static TElement Instantiate<TElement>(VisualTreeAsset asset) where TElement : VisualElement =>
            Instantiate(typeof(TElement), asset) as TElement;

        public static VisualElement Instantiate(System.Type type, VisualTreeAsset asset)
        {
            var entry = Instance.FindEntry(asset.name);
            return Instantiate(type, asset, entry?.Styles, true);
        }

        private static VisualElement Instantiate(System.Type type, VisualTreeAsset asset, StyleSheet[] styles, bool bind)
        {
            if (asset.importedWithErrors)
                throw new System.Exception($"{asset.name}.uxml cannot be instantiated because it had errors while importing");

            try
            {
                var root = asset.CloneTree().Children().FirstOrDefault().Query().Where(type.IsInstanceOfType).First();
                root.RemoveFromHierarchy();

                if (styles != null)
                    foreach (var style in styles)
                        root.styleSheets.Add(style);

                if (bind && root is UIView view)
                    UIView.BindHierarchy(view);

                return root;
            }
            catch(System.Exception ex)
            {
                Debug.LogException(ex);
                throw new System.Exception($"Root of type '{type}' not found in {asset.name}.uxml");
            }
        }

        private Entry FindEntry(string name)
        {
            if (_entriesByName.TryGetValue(name, out var entry))
                return entry;

            if(_inherit != null)
                for(int i=_inherit.Length-1; i>=0; i--)
                {
                    entry = _inherit[i].FindEntry(name);
                    if (null != entry)
                        return entry;
                }

            return null;
        }

        public static VisualTreeFactory CreateInstance(VisualTreeFactory[] inherit, Entry[] entries)
        {
            var factory = VisualTreeFactory.CreateInstance<VisualTreeFactory>();
            factory._entries = entries;
            factory._inherit = inherit;
            return factory;
        }

        private void OnEnable()
        {
            if (_entries == null)
                return;

            _entriesByName = new();
            foreach (var entry in _entries)
                if (entry != null)
                    _entriesByName[entry.Asset.name] = entry;
        }
    }
}
