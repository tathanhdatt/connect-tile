using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SettingView : BaseView
{
    [SerializeField] private Transform panel;
    [Header("Sound Toggle")] 
    [SerializeField] private SwitchSideToggle soundToggle;
    [SerializeField] private SwitchSideToggle vibrationToggle;
    
    [Header("Button")]
    [SerializeField] private Button homeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button closeButton;

    public event Action OnClickHomeButton;
    public event Action OnClickRestartButton;
    public event Action OnClickCloseButton; 

    public event Action<bool> OnSoundToggleChange;
    public event Action<bool> OnVibrantToggleChange;

    protected override void Initialize()
    {
        base.Initialize();
        this.homeButton.onClick.AddListener(() => OnClickHomeButton?.Invoke());
        this.restartButton.onClick.AddListener(() => OnClickRestartButton?.Invoke());
        this.closeButton.onClick.AddListener(() => OnClickCloseButton?.Invoke());
        this.soundToggle.OnChange += isOn => OnSoundToggleChange?.Invoke(isOn);
        this.vibrationToggle.OnChange += isOn => OnVibrantToggleChange?.Invoke(isOn);
    }

    public override async void Show()
    {
        base.Show();
        await this.panel.DOScale(1, 0.2f)
            .SetEase(Ease.OutBack).AsyncWaitForCompletion().AsUniTask();
    }

    public override async void Hide()
    {
        await this.panel.DOScale(0, 0.2f)
            .SetEase(Ease.Linear).AsyncWaitForCompletion().AsUniTask();
        base.Hide();
    }

    public void SetSoundToggleStatus(bool isOn)
    {
        this.soundToggle.SetCurrentStatus(isOn);
    }

    public void SetVibrationToggleStatus(bool isOn)
    {
        this.vibrationToggle.SetCurrentStatus(isOn);
    }
}

