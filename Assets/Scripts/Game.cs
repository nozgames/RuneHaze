/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

using UnityEngine;
using UnityEngine.UIElements;

using NoZ.RuneHaze.UI;

namespace NoZ.RuneHaze
{
    public class Game : MonoBehaviour, IModuleLoaderProvider
    {
        [SerializeField] private ModuleLoader _moduleLoader;
        [SerializeField] private VisualTreeFactory _uiFactory;
        [SerializeField] private PanelSettings _panelSettings;
        [SerializeField] private ActorDefinition _playerTest;
        [SerializeField] private Arena _arena;
        [SerializeField] private Camera _camera;
        [SerializeField] private AudioListener _audioListener;
        
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

            CameraManager.Instance.Camera = _camera;
            
            WaveManager.Instance.WaveComplete += OnWaveComplete;
            
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
            if (ArenaManager.Instance.Current == null)
                return;
            
            EnemyManager.Instance.Update();
            WaveManager.Instance.Update();
        }

        private void OnApplicationQuit()
        {
            Stop();
            
            _moduleLoader.UnloadModules();

#if UNITY_EDITOR            
            Signal.LogLeaks();
#endif            
        }

        public void Play()
        {
            ArenaManager.Instance.LoadArena(_arena);

            Player = (Player)ArenaManager.Instance.InstantiateActor(_playerTest, Vector3.zero, Quaternion.identity);
            // Player.Health.Changed.AddListener(OnPlayerHealthChanged);
            // Player.Health.Death.AddListener(OnPlayerDeath);
            
            WaveManager.Instance.StartWave(0);

            _main.SetDisplay(false);
            _play = UIView.Instantiate<UIPlay>();
            Root.Add(_play);
        }

        private void OnPlayerHealthChanged(Entity attacker, int amount)
        {
            // PostProcModule.Instance.SetVignette(
            //     PostProcModule.VignetteChannel.Health,
            //     Player.Health.Percent.Remap(_healthRange, _healthIntensity));
        }

        private void OnPlayerDeath(Entity arg0)
        {
            Stop();
        }

        public void Stop()
        {
            if (ArenaManager.Instance.Current == null)
                return;

            WaveManager.Instance.StopWave();
            
            Destroy(Player.gameObject);
            Player = null;
            
            ArenaManager.Instance.UnloadArena();

            PostProcModule.Instance.SetVignette(PostProcModule.VignetteChannel.Health, 0.0f);
            
            _play.Dispose();
            _play = null;

            IsPaused = false;
            
            _main.SetDisplay(true);
        }

        private void OnWaveComplete()
        {
            if (!WaveManager.Instance.NextWave())
                Stop();
        }
        
        public void ListenAt (Transform transform)
        {
            _audioListener.transform.position = transform.position;
            _audioListener.transform.rotation = transform.rotation;
        }
    }
}
