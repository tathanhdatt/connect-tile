using _Module.Core.SaveGame;
using Core;
using Core.AudioService;
using Core.Service;
using Core.VibrationService;
using Cysharp.Threading.Tasks;
using Gameplay;
using MessageEvent;
using UnityEngine;

public class SettingViewPresenter : BaseViewPresenter
{
    private SettingView settingView;
    private readonly GameData gameData;
    private readonly ISaveManager saveManager;
    
    public SettingViewPresenter(
        GamePresenter gamePresenter, 
        Transform transform,
        GameData gameData,
        ISaveManager saveManager) : base(gamePresenter, transform)
    {
        this.gameData = gameData;
        this.saveManager = saveManager;
    }

    protected override void AddViews()
    {
        this.settingView = AddView<SettingView>();
    }

    protected override async void OnShow()
    {
        base.OnShow();
        this.settingView.OnClickHomeButton += ClickHomeHandler;
        this.settingView.OnClickRestartButton += ClickRestartHandler;
        this.settingView.OnClickCloseButton += ClickCloseHandler;
        this.settingView.OnSoundToggleChange += SoundToggleChangeHandler;
        this.settingView.OnVibrantToggleChange += VibrantToggleChangeHandler;
        // await UniTask.Delay(200);
        this.settingView.SetSoundToggleStatus(Mathf.Abs(this.gameData.Volume - 1) <= Mathf.Epsilon);
        this.settingView.SetVibrationToggleStatus(this.gameData.Vibration);
    }

    protected override void OnHide()
    {
        base.OnHide();
        this.settingView.OnClickHomeButton -= ClickHomeHandler;
        this.settingView.OnClickRestartButton -= ClickRestartHandler;
        this.settingView.OnClickCloseButton -= ClickCloseHandler;
        this.settingView.OnSoundToggleChange -= SoundToggleChangeHandler;
        this.settingView.OnVibrantToggleChange -= VibrantToggleChangeHandler;
    }

    private void ClickCloseHandler()
    {
        Hide();
    }

    private void VibrantToggleChangeHandler(bool isOn)
    {
        ServiceLocator.GetService<IVibrationService>().SetEnable(isOn);
        this.gameData.Vibration = isOn;
        this.saveManager.Save();
    }

    private void SoundToggleChangeHandler(bool isOn)
    {
        if (isOn)
        {
            ServiceLocator.GetService<IAudioService>().SetVolume(1);
            this.gameData.Volume = 1;
        }
        else
        {
            ServiceLocator.GetService<IAudioService>().SetVolume(0);
            this.gameData.Volume = 0;
        }
        this.saveManager.Save();
    }

    private void ClickRestartHandler()
    {
        Hide();
        if (GamePresenter.GetViewPresenter<GameplayViewPresenter>().IsShowing)
        {
            Messenger.Broadcast(EventKey.PlayAgain);
        }
    }

    private void ClickHomeHandler()
    {
        Hide();
        Messenger.Broadcast(EventKey.ClearLevel);
        GamePresenter.GetViewPresenter<GameplayViewPresenter>().Hide();
        GamePresenter.GetViewPresenter<HomeViewPresenter>().Show();
    }
}
