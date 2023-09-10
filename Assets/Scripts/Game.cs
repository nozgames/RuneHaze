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
        [SerializeField] private Arena _arena;
        [SerializeField] private Camera _camera;
        
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

            CameraSystem.Instance.Camera = _camera;
            
            WaveSystem.Instance.WaveComplete += OnWaveComplete;
            
            VisualTreeFactory.Instance = _uiFactory;

            // Create a document for the root ui
            var doc = gameObject.AddComponent<UIDocument>();
            doc.panelSettings = _panelSettings;
            Root = doc.rootVisualElement.parent;
            Root.AddToClassList("root");
            
            _main = UIView.Instantiate<UI.UIMain>();
            Root.Add(_main);
        }

        private void Update()
        {
            if (ArenaSystem.Instance.Current == null)
                return;
            
            EnemySystem.Instance.Update();
            WaveSystem.Instance.Update();
        }

        private void OnApplicationQuit()
        {
            Stop();
            
            for (var i = _modules.Length - 1; i >= 0; i--)
                _modules[i].UnloadInstance();
        }

        public void Play()
        {
            ArenaSystem.Instance.LoadArena(_arena);

            Player = Instantiate(_playerTest).GetComponent<Player>();
            Player.Health.Death.AddListener(OnPlayerDeath);

            WaveSystem.Instance.StartWave(0);

            _main.SetDisplay(false);
            _play = UIView.Instantiate<UIPlay>();
            Root.Add(_play);
        }

        private void OnPlayerDeath(Entity arg0)
        {
            Stop();
        }

        private void Stop()
        {
            if (ArenaSystem.Instance.Current == null)
                return;

            WaveSystem.Instance.StopWave();
            
            Destroy(Player.gameObject);
            Player = null;
            
            ArenaSystem.Instance.UnloadArena();

            _play.Dispose();
            _play = null;
            
            _main.SetDisplay(true);
        }

        private void OnWaveComplete()
        {
            if (!WaveSystem.Instance.NextWave())
                Stop();
        }
    }
}
