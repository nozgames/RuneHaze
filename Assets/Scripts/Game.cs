/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

using UnityEngine;
using UnityEngine.UIElements;

using RuneHaze.UI;

namespace RuneHaze
{
    public class Game : MonoBehaviour
    {
        [SerializeField] private Module[] _modules;
        [SerializeField] private VisualTreeFactory _uiFactory;
        [SerializeField] private PanelSettings _panelSettings;
        [SerializeField] private GameObject _playerTest;
        [SerializeField] private GameObject _enemyTest;
        
        public static Game Instance { get; private set; }

        private UIMain _main;
        private UIPlay _play;
        
        public Player Player { get; private set; }
        
        public VisualElement Root { get; private set; }
        
        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            foreach (var module in _modules)
                module.LoadInstance();

            VisualTreeFactory.Instance = _uiFactory;

            // Create a document for the root ui
            var doc = gameObject.AddComponent<UIDocument>();
            doc.panelSettings = _panelSettings;
            Root = doc.rootVisualElement.parent;

            _main = UIView.Instantiate<UI.UIMain>();
            Root.Add(_main);
        }

        private void Update()
        {
            SwarmSystem.Instance.Update();
        }

        private void OnApplicationQuit()
        {
            for (var i = _modules.Length - 1; i >= 0; i--)
                _modules[i].UnloadInstance();
        }

        public void Play()
        {
            _main.SetDisplay(false);
            _play = UIView.Instantiate<UIPlay>();
            Root.Add(_play);
            
            Player = Instantiate(_playerTest).GetComponent<Player>();
            
            for (int i=0; i<6; i++)
                Instantiate(_enemyTest);
        }
    }
}
