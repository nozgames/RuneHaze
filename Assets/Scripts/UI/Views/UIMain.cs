/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

using UnityEngine.Device;
using UnityEngine.UIElements;
using UnityEngine.Scripting;

namespace RuneHaze.UI
{
    public class UIMain : UIView
    {
        [Preserve]
        public new class UxmlFactory : UxmlViewFactory<UIMain, UxmlTraits> { }    

        [Bind] private VisualElement _playButton;
        [Bind] private VisualElement _optionsButton;
        [Bind] private VisualElement _quitButton;
        
        protected override void Bind()
        {
            base.Bind();

            _playButton.AddManipulator(new Clickable(Play));
            _optionsButton.AddManipulator(new Clickable(OnOptions));
            _quitButton.AddManipulator(new Clickable(Quit));
        }

        private void OnOptions()
        {
            Game.Instance.Root.Add(UIView.Instantiate<UIOptions>());
        }

        protected override void OnDisplayBegin()
        {
            base.OnDisplayBegin();
        }

        protected override void OnDisplayEnd()
        {
            base.OnDisplayBegin();
        }

        private void Play()
        {
            Game.Instance.Play();
        }

        private void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
