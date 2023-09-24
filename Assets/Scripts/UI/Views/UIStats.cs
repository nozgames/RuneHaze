/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

using UnityEngine.UIElements;
using UnityEngine.Scripting;

namespace NoZ.RuneHaze.UI
{
    public class UIStats : UIView
    {
        private const string UssRowEven = "row-even";
        private const string UssRowOdd = "row-odd";
        
        [Preserve]
        public new class UxmlFactory : UxmlViewFactory<UIStats, UxmlTraits> { }

        [Bind] private ScrollView _content;
        
        public static UIStats Instantiate(Actor actor)
        {
            return UIView.Instantiate<UIStats>().Bind(actor);
        }
        
        private UIStats Bind(Actor actor)
        {
            _content.Clear();

#if false            
            var statIndex = 0;
            foreach (var stat in actor.Stats)
            {
                var cell = UIStatCell.Instantiate(actor, stat);
                cell.AddToClassList(statIndex++ % 2 == 0 ? UssRowEven : UssRowOdd);
                _content.Add(cell);
            }
#endif

            return this;
        }
    }
}
