/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

using UnityEngine.UIElements;
using UnityEngine.Scripting;

namespace RuneHaze.UI
{
    public class UIStats : UIView
    {
        private const string UssRowEven = "row-even";
        private const string UssRowOdd = "row-odd";
        
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

            var statIndex = 0;
            foreach (var stat in character.Stats)
            {
                var cell = UIStatCell.Instantiate(character, stat);
                cell.AddToClassList(statIndex++ % 2 == 0 ? UssRowEven : UssRowOdd);
                _content.Add(cell);
            }

            return this;
        }
    }
}
