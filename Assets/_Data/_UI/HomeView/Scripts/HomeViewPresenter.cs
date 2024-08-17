using Core;
using Gameplay;
using MessageEvent;
using UnityEngine;

public class HomeViewPresenter : BaseViewPresenter
{
    private HomeView homeView;
    private readonly GameData gameData;
    
    public HomeViewPresenter(
        GamePresenter gamePresenter, 
        Transform transform,
        GameData gameData
    ) : base(gamePresenter, transform)
    {
        this.gameData = gameData;
    }

    protected override void AddViews()
    {
        homeView = AddView<HomeView>();
    }

    protected override void OnShow()
    {
        base.OnShow();
        this.homeView.OnClickSettingButton += ClickSettingHandler;
        this.homeView.OnClickPlayButton += ClickPlayHandler;
        this.homeView.SetLevelCountText(this.gameData.CurrentLevelOrder);
    }

    protected override void OnHide()
    {
        base.OnHide();
        this.homeView.OnClickSettingButton -= ClickSettingHandler;
        this.homeView.OnClickPlayButton -= ClickPlayHandler;
    }

    private void ClickPlayHandler()
    {
        Hide();
        GamePresenter.GetViewPresenter<GameplayViewPresenter>().Show();
        Messenger.Broadcast(EventKey.PlayLevel, this.gameData.CurrentLevelOrder);
    }

    private void ClickSettingHandler()
    {
        GamePresenter.GetViewPresenter<SettingViewPresenter>().Show();
    }
}
