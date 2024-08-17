using System;
using System.Collections.Generic;
using Core.Game;
using Gameplay;
using UnityEngine;

public class GamePresenter : MonoBehaviour
{
    private readonly Dictionary<Type, BaseViewPresenter> presenters = new Dictionary<Type, BaseViewPresenter>();
    private GameContext context;
    
    public void Enter(GameContext gameContext)
    {
        this.context = gameContext;
    }

    public void InitialViewPresenters()
    {
        var gameplayViewPresenter = new GameplayViewPresenter(
            this, this.transform, context.GameData);
        AddPresenter(gameplayViewPresenter);
        
        var loadingViewPresenter = new LoadingViewPresenter(
            this, this.transform);
        AddPresenter(loadingViewPresenter);

        var homeViewPresenter = new HomeViewPresenter(
            this, this.transform, context.GameData);
        AddPresenter(homeViewPresenter);

        var settingViewPresenter = new SettingViewPresenter(
            this, this.transform, context.GameData, context.SaveManager);
        AddPresenter(settingViewPresenter);

        var winViewPresenter = new WinViewPresenter(
            this, this.transform, context.GameData);
        AddPresenter(winViewPresenter);
        
        var loseViewPresenter = new LoseViewPresenter(
            this, this.transform);
        AddPresenter(loseViewPresenter);
    }

    private void AddPresenter(BaseViewPresenter presenter)
    {
        presenter.Initialize();
        presenters.Add(presenter.GetType(), presenter);
    }

    public T GetViewPresenter<T>() where T : BaseViewPresenter
    {
        var type = typeof(T);
        return (T)this.presenters[type];
    }
}