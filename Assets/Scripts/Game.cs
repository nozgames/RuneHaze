/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

using UnityEngine;
using UnityEngine.UIElements;

using RuneHaze.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

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
        
        [Header("Vignette")]
        [SerializeField] private VolumeProfile _postProcessProfile;
        [SerializeField] private Color _healthColor;
        [SerializeField] private Vector2 _healthRange;
        [SerializeField] private Vector2 _healthIntensity;
        
        public static Game Instance { get; private set; }

        private UIMain _main;
        private UIPlay _play;
        private Vignette _vignette;
        
        public Player Player { get; private set; }
        
        public VisualElement Root { get; private set; }

        private bool _paused;

        public bool IsPaused
        {
            get => _paused;
            set
            {
                _paused = value;
                Time.timeScale = _paused ? 0.0f : 1.0f;
            }
        }
        
        private void Awake()
        {
            Instance = this;

            if(!_postProcessProfile.TryGet(out _vignette))
                throw new System.NullReferenceException(nameof(_vignette));
        }

        private void Start()
        {
            _vignette.intensity.Override(0);        
            
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
            Player.Health.Changed.AddListener(OnPlayerHealthChanged);
            Player.Health.Death.AddListener(OnPlayerDeath);

            _vignette.intensity.Override(0);
            
            WaveSystem.Instance.StartWave(0);

            _main.SetDisplay(false);
            _play = UIView.Instantiate<UIPlay>();
            Root.Add(_play);
        }

        private void OnPlayerHealthChanged(Entity attacker, int amount)
        {
            var intensity = Player.Health.Percent.Remap(_healthRange, _healthIntensity);
            _vignette.intensity.Override(intensity);
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
