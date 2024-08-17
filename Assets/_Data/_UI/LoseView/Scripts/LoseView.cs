using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class LoseView : BaseView
{
    [SerializeField] private Image fadeBg;
    
    [Header("Button")]
    [SerializeField] private Button restartButton;

    public event Action OnClickRestartButton;

    protected override void Initialize()
    {
        base.Initialize();
        this.restartButton.onClick
            .AddListener(() => OnClickRestartButton?.Invoke());
    }

    public override async void Show()
    {
        base.Show();
        await FadeOut();
    }

    public override async void Hide()
    {
        await FadeIn();
        base.Hide();
    }

    private async UniTask FadeOut()
    {
        await this.fadeBg.DOFade(0, 0.2f)
            .AsyncWaitForCompletion();
        this.fadeBg.gameObject.SetActive(false);
    }

    private async UniTask FadeIn()
    {
        this.fadeBg.gameObject.SetActive(true);
        await this.fadeBg.DOFade(1, 0.2f)
            .AsyncWaitForCompletion();
    }
}
