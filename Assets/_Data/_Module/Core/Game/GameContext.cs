using _Module.Core.SaveGame;
using Core.AudioService;
using Core.Service;
using Core.VibrationService;
using Cysharp.Threading.Tasks;
using MessageEvent;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Serialization;

namespace Core.Game
{
    public class GameContext : MonoBehaviour
    {
        [SerializeField] private GamePresenter presenter;
        private GameObject currentLevel;

        public ISaveManager SaveManager { get; private set; }
        public GameData GameData { get; private set; }
        
        private void Awake()
        {
            Initialize();
        }

        private void Initialize()
        {
            Application.targetFrameRate = 60;
            InitGameData();
            InitServices();
        }

        private void InitGameData()
        {
            this.GameData = new GameData();
            this.SaveManager = new PlayerPrefSave(GameData);
            this.GameData.CurrentLevelOrder = PlayerPrefs
                .GetInt(nameof(GameData.CurrentLevelOrder), 1);
            this.GameData.Volume = PlayerPrefs
                .GetFloat(nameof(GameData.Volume), 1);
            this.GameData.Vibration = PlayerPrefs
                .GetInt(nameof(GameData.Vibration), 1) == 1;
        }
        

        private void InitServices()
        {
            IAudioService audioService = FindObjectOfType<NativeAudioService>();
            ServiceLocator.Register(audioService);
            ServiceLocator.GetService<IAudioService>().SetVolume(this.GameData.Volume);

            IVibrationService vibrationService = new NativeVibrationService();
            ServiceLocator.Register(vibrationService);
            ServiceLocator.GetService<IVibrationService>().SetEnable(this.GameData.Vibration);
        }

        private void Start()
        {
            presenter.Enter(this);
            presenter.InitialViewPresenters();
            OnEnter();
            Messenger.AddListener<int>(EventKey.PlayLevel, PlayLevel);
            Messenger.AddListener(EventKey.PlayAgain, PlayAgainHandler);
            Messenger.AddListener(EventKey.LevelWin, LevelWinHandler);
            Messenger.AddListener(EventKey.ClearLevel, ClearLevelHandler);
            Messenger.AddListener(EventKey.LevelLose, LevelLoseHandler);
            
        }

        private async void OnEnter()
        {
            presenter.GetViewPresenter<LoadingViewPresenter>().Show();
            ServiceLocator.GetService<IAudioService>().PlayMusic(AudioName.Bgm);
            await presenter.GetViewPresenter<LoadingViewPresenter>().Load();
            presenter.GetViewPresenter<LoadingViewPresenter>().Hide();
            presenter.GetViewPresenter<HomeViewPresenter>().Show();
        }

        private async void PlayLevel(int levelId)
        {
            string address = $"L_{levelId:D3}";
            var loader = 
                Addressables.LoadAssetAsync<GameObject>(address);
            GameObject levelPrefab = await loader.Task;
            this.currentLevel = Instantiate(levelPrefab);
        }

        private void LevelWinHandler()
        {
            this.GameData.CurrentLevelOrder++;
            if (this.GameData.CurrentLevelOrder > 34)
            {
                this.GameData.CurrentLevelOrder = 1;
            }
            SaveManager.Save();
            Destroy(this.currentLevel);
            ServiceLocator.GetService<IAudioService>().PlaySfx(AudioName.Win);
            presenter.GetViewPresenter<WinViewPresenter>().Show();
        }

        private void ClearLevelHandler()
        {
            Destroy(this.currentLevel);
        }

        private void PlayAgainHandler()
        {
            ClearLevelHandler();
            PlayLevel(this.GameData.CurrentLevelOrder);
        }

        private void LevelLoseHandler()
        {
            ServiceLocator.GetService<IAudioService>().PlaySfx(AudioName.Lose);
            presenter.GetViewPresenter<LoseViewPresenter>().Show();
        }
    }
}