/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Scripting;

namespace RuneHaze.UI
{
    public class UIStatCell : UIView
    {
        [Preserve]
        public new class UxmlFactory : UxmlViewFactory<UIStatCell, UxmlTraits> { }

        [Bind] private Image _statIcon;
        [Bind] private Label _statName;
        [Bind] private Label _statValue;

        private Character _character;
        private CharacterStat _stat;
        
        public static UIStatCell Instantiate(Character character, CharacterStat stat)
        {
            return UIView.Instantiate<UIStatCell>().Bind(character, stat);
        }
        
        private UIStatCell Bind(Character character, CharacterStat stat)
        {
            _character = character;
            _stat = stat;

            OnPostUpdateStats(_character);

            _character.PostUpdateStatsEvents += OnPostUpdateStats;
            
            return this;
        }

        protected override void OnDispose()
        {
            base.OnDispose();
            
            _character.PostUpdateStatsEvents -= OnPostUpdateStats;
        }

        private void OnPostUpdateStats(Character character)
        {
            _statName.text = _stat.name;
            _statValue.text = character.GetStatValue(_stat).Value.ToString();
        }
    }
}
