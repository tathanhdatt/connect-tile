using System;
using Core.Extension;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HomeView : BaseView
{
    [Header("Button")] 
    [SerializeField] private Button settingButton;
    [SerializeField] private Button playButton;

    [Header("Text")] 
    [SerializeField] private TMP_Text levelCountText;

    public event Action OnClickSettingButton;
    public event Action OnClickPlayButton;

    protected override void Initialize()
    {
        base.Initialize();
        this.settingButton.onClick.AddListener(() => OnClickSettingButton?.Invoke());
        this.playButton.onClick.AddListener(() => OnClickPlayButton?.Invoke());
    }

    public void SetLevelCountText(int levelOrder)
    {
        this.levelCountText.SetText(levelOrder.IntToText());
    }
}