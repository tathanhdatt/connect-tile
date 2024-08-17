using System.Threading.Tasks;
using Core.AudioService;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Gameplay;
using MessageEvent;
using UnityEngine;

public class LoadingViewPresenter : BaseViewPresenter
{
    private LoadingView loadingView;
    public LoadingViewPresenter(
        GamePresenter gamePresenter,
        Transform transform) : base(gamePresenter, transform)
    {
    }

    protected override void AddViews()
    {
        loadingView = AddView<LoadingView>();
    }

    public async UniTask Load()
    {
        await DOTween.To(fillValue =>
            {
                this.loadingView.SetFillAmount(fillValue);
            }, 0, 0.4f, 2f)
            .SetEase(Ease.OutCubic)
            .AsyncWaitForCompletion().AsUniTask();
        
        await DOTween.To(fillValue =>
            {
                this.loadingView.SetFillAmount(fillValue);
            }, 0.4f, 0.8f, 3f)
            .SetDelay(0.3f).SetEase(Ease.OutCubic)
            .AsyncWaitForCompletion().AsUniTask();
        
        await DOTween.To(fillValue =>
            {
                this.loadingView.SetFillAmount(fillValue);
            }, 0.8f, 1f, 0.5f)
            .SetDelay(0.1f).SetEase(Ease.OutCubic)
            .AsyncWaitForCompletion().AsUniTask();
        await UniTask.Delay(400);
    }
}
