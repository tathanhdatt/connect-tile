using System.Collections;
using System.Collections.Generic;
using MessageEvent;
using UnityEngine;

public class LoseViewPresenter : BaseViewPresenter
{
    private LoseView loseView;

    public LoseViewPresenter(
        GamePresenter gamePresenter, 
        Transform transform) : base(gamePresenter, transform)
    {
    }

    protected override void AddViews()
    {
        this.loseView = AddView<LoseView>();
    }

    protected override void OnShow()
    {
        base.OnShow();
        this.loseView.OnClickRestartButton += ClickRestartHandler;
    }


    protected override void OnHide()
    {
        base.OnHide();
        this.loseView.OnClickRestartButton -= ClickRestartHandler;
    }
    
    private void ClickRestartHandler()
    {
        Hide();
        Messenger.Broadcast(EventKey.PlayAgain);
    }
}
