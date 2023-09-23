/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

using UnityEngine.UIElements;
using UnityEngine.Scripting;

namespace RuneHaze.UI
{
    public class UIPause : UIView
    {
        [Preserve]
        public new class UxmlFactory : UxmlViewFactory<UIPause, UxmlTraits> { }    

        [Bind] private VisualElement _resumeButton;
        [Bind] private VisualElement _mainMenuButton;
        [Bind] private VisualElement _playerStatsContainer;
        
        private UIStats _playerStats;

        protected override void Bind()
        {
            base.Bind();
            
            _resumeButton.AddManipulator(new Clickable(OnResume));
            _mainMenuButton.AddManipulator(new Clickable(OnMainMenu));
         
            _playerStats = UIStats.Instantiate(Game.Instance.Player);
            _playerStatsContainer.Add(_playerStats);
            
            InputModule.Instance.MenuButton += OnResume;
        }

        protected override void OnDispose()
        {
            base.OnDispose();
            
            InputModule.Instance.MenuButton -= OnResume;
        }
        
        private void OnMainMenu()
        {
            Game.Instance.Stop();
            Dispose();
        }

        private void OnResume()
        {
            Game.Instance.IsPaused = false;
            Dispose();
        }
    }
}
