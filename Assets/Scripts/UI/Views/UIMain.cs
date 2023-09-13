/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

using UnityEngine.UIElements;
using UnityEngine.Scripting;

namespace RuneHaze.UI
{
    public class UIMain : UIView
    {
        [Preserve]
        public new class UxmlFactory : UxmlViewFactory<UIMain, UxmlTraits> { }    

        [Bind] private VisualElement _playButton;
        
        protected override void Bind()
        {
            base.Bind();

            _playButton.AddManipulator(new Clickable(Play));
        }
        
        protected override void OnDisplayBegin()
        {
            base.OnDisplayBegin();
            
            InputModule.Instance.PlayerAttack += Play;
        }

        protected override void OnDisplayEnd()
        {
            base.OnDisplayBegin();
            
            InputModule.Instance.PlayerAttack -= Play;
        }

        private void Play()
        {
            Game.Instance.Play();
        }
    }
}
