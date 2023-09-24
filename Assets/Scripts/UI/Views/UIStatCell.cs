/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Scripting;

namespace NoZ.RuneHaze.UI
{
    public class UIStatCell : UIView
    {
        private const string UssNegative = "negative";
        private const string UssPositive = "positive";
        
        [Preserve]
        public new class UxmlFactory : UxmlViewFactory<UIStatCell, UxmlTraits> { }

        [Bind] private Image _statIcon;
        [Bind] private Label _statName;
        [Bind] private Label _statValue;

        private Actor _actor;
        private CharacterStat _stat;
        
        public static UIStatCell Instantiate(Actor actor, CharacterStat stat)
        {
            return UIView.Instantiate<UIStatCell>().Bind(actor, stat);
        }
        
        private UIStatCell Bind(Actor actor, CharacterStat stat)
        {
            _actor = actor;
            _stat = stat;

            OnPostUpdateStats(_actor);

            _actor.PostUpdateStatsEvents += OnPostUpdateStats;
            
            return this;
        }

        protected override void OnDispose()
        {
            base.OnDispose();
            
            _actor.PostUpdateStatsEvents -= OnPostUpdateStats;
        }

        private void OnPostUpdateStats(Actor actor)
        {
            var value = actor.GetStatValue(_stat).Value;
            EnableInClassList(UssNegative, value < 0);
            EnableInClassList(UssPositive, value > 0);
            
            _statName.text = _stat.name;
            _statValue.text = value.ToString();
        }
    }
}
