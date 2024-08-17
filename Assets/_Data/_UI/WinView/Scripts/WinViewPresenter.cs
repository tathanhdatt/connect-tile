using System.Collections;
using System.Collections.Generic;
using Core;
using MessageEvent;
using UnityEngine;

public class WinViewPresenter : BaseViewPresenter
{
    private WinView winView;
    private readonly GameData gameData;

    public WinViewPresenter(
        GamePresenter gamePresenter, Transform transform,
        GameData gameData) : base(gamePresenter, transform)
    {
        this.gameData = gameData;
    }

    protected override void AddViews()
    {
        this.winView = AddView<WinView>();
    }

    protected override void OnShow()
    {
        base.OnShow();
        this.winView.OnClickHomeButton += ClickHomeHandler;
        this.winView.OnClickNextButton += ClickNextHandler;
    }

    protected override void OnHide()
    {
        base.OnHide();
        this.winView.OnClickHomeButton -= ClickHomeHandler;
        this.winView.OnClickNextButton -= ClickNextHandler;
    }

    private void ClickNextHandler()
    {
        Hide();
        Messenger.Broadcast(EventKey.PlayLevel, this.gameData.CurrentLevelOrder);
    }

    private void ClickHomeHandler()
    {
        Hide();
        
        GamePresenter.GetViewPresenter<HomeViewPresenter>().Show();
    }
}
