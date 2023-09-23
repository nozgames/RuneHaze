/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

using NoZ;
using UnityEngine;
using UnityEngine.UIElements;

using RuneHaze.UI;

namespace RuneHaze
{
    public class Game : MonoBehaviour, IModuleLoaderProvider
    {
        [SerializeField] private ModuleLoader _moduleLoader;
        [SerializeField] private VisualTreeFactory _uiFactory;
        [SerializeField] private PanelSettings _panelSettings;
        [SerializeField] private GameObject _playerTest;
        [SerializeField] private Arena _arena;
        [SerializeField] private Camera _camera;
        
        [Header("Vignette")]
        [SerializeField] private Vector2 _healthRange;
        [SerializeField] private Vector2 _healthIntensity;
        
        public static Game Instance { get; private set; }

        private UIMain _main;
        private UIPlay _play;

        public event System.Action<bool> Paused; 
        
        public Player Player { get; private set; }
        
        public VisualElement Root { get; private set; }

        public ModuleLoader ModuleLoader => _moduleLoader;
        
        private bool _paused;

        public bool IsPaused
        {
            get => _paused;
            set
            {
                _paused = value;
                Time.timeScale = _paused ? 0.0f : 1.0f;
                Paused?.Invoke(_paused);
            }
        }
        
        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            _moduleLoader.LoadModules();

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
            
            _moduleLoader.UnloadModules();
        }

        public void Play()
        {
            ArenaSystem.Instance.LoadArena(_arena);

            Player = Instantiate(_playerTest).GetComponent<Player>();
            Player.Health.Changed.AddListener(OnPlayerHealthChanged);
            Player.Health.Death.AddListener(OnPlayerDeath);
            
            WaveSystem.Instance.StartWave(0);

            _main.SetDisplay(false);
            _play = UIView.Instantiate<UIPlay>();
            Root.Add(_play);
        }

        private void OnPlayerHealthChanged(Entity attacker, int amount)
        {
            PostProcModule.Instance.SetVignette(
                PostProcModule.VignetteChannel.Health,
                Player.Health.Percent.Remap(_healthRange, _healthIntensity));
        }

        private void OnPlayerDeath(Entity arg0)
        {
            Stop();
        }

        public void Stop()
        {
            if (ArenaSystem.Instance.Current == null)
                return;

            WaveSystem.Instance.StopWave();
            
            Destroy(Player.gameObject);
            Player = null;
            
            ArenaSystem.Instance.UnloadArena();

            PostProcModule.Instance.SetVignette(PostProcModule.VignetteChannel.Health, 0.0f);
            
            _play.Dispose();
            _play = null;

            IsPaused = false;
            
            _main.SetDisplay(true);
        }

        private void OnWaveComplete()
        {
            if (!WaveSystem.Instance.NextWave())
                Stop();
        }
    }
}
