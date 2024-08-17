using System.Collections.Generic;
using UnityEngine;

public abstract class BaseViewPresenter
{
    protected GamePresenter GamePresenter { get; private set; }
    private readonly List<BaseView> views = new List<BaseView>();
    public Transform Transform { get; private set; }
    public bool IsShowing { get; private set; }

    protected BaseViewPresenter(GamePresenter gamePresenter, Transform transform)
    {
        this.GamePresenter = gamePresenter;
        Transform = transform;
    }

    public void Initialize()
    {
        AddViews();
    }

    protected abstract void AddViews();

    protected T AddView<T>() where T : BaseView
    {
        var view = Object.FindObjectOfType<T>();
        views.Add(view);
        return view;
    }

    public void Show()
    {
        IsShowing = true;
        foreach (var view in views)
        {
            view.Show();
        }
        
        OnShow();
    }

    protected virtual void OnShow()
    {
        
    }

    public void Hide()
    {
        IsShowing = false;
        foreach (var view in views)
        {
            view.Hide();
        }

        OnHide();
    }

    protected virtual void OnHide()
    {
        
    }
}
