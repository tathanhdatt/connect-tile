using System.Threading;
using System.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingView : BaseView
{
    [SerializeField] private Image fill;
    [SerializeField] private TMP_Text loadingText;

    private bool isLoaded = false;

    public override void Show()
    {
        base.Show();
        LoadingTextEffect();
    }

    public void SetFillAmount(float fillValue)
    {
        this.fill.fillAmount = fillValue;
    }

    public override void Hide()
    {
        base.Hide();
        isLoaded = true;
    }

    private async void LoadingTextEffect()
    {
        const string loadingText1 = "Loading.";
        const string loadingText2 = "Loading..";
        const string loadingText3 = "Loading...";
        while (true)
        {
            loadingText.SetText(loadingText1);
            await Task.Delay(400);
            loadingText.SetText(loadingText2);
            await Task.Delay(400);
            loadingText.SetText(loadingText3);
            await Task.Delay(400);
            if (isLoaded)
            {
                break;
            }
        }
    }
}
