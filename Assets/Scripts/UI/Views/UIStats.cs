/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

using UnityEngine.UIElements;
using UnityEngine.Scripting;

namespace RuneHaze.UI
{
    public class UIStats : UIView
    {
        [Preserve]
        public new class UxmlFactory : UxmlViewFactory<UIStats, UxmlTraits> { }

        [Bind] private ScrollView _content;
        
        public static UIStats Instantiate(Character character)
        {
            return UIView.Instantiate<UIStats>().Bind(character);
        }
        
        private UIStats Bind(Character character)
        {
            _content.Clear();
            
            foreach (var stat in character.Stats)
                _content.Add(UIStatCell.Instantiate(character, stat));

            return this;
        }
    }
}
