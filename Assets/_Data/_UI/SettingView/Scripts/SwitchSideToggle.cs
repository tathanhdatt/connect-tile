using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class SwitchSideToggle : MonoBehaviour
{
    [SerializeField] private Toggle toggle;

    [Header("Indicator")]
    [SerializeField, Tooltip("Second")] private float changingDuration;
    [SerializeField] private Image indicator;
    [SerializeField] private Transform onPosition;
    [SerializeField] private Transform offPosition;
    [SerializeField] private Ease ease;

    public event Action<bool> OnChange;

    private void Awake()
    {
        this.toggle = GetComponent<Toggle>();
        this.toggle.onValueChanged.AddListener(isOn =>
        {
            OnChange?.Invoke(isOn);
            ChangePositionIndicatorEffect(isOn);
        });
    }

    public void SetCurrentStatus(bool isOn)
    {
        this.toggle.isOn = isOn;
        if (isOn)
        {
            this.indicator.transform.localPosition = this.onPosition.localPosition;
        }
        else
        {
            this.indicator.transform.localPosition = this.offPosition.localPosition;
        }
    }

    private async void ChangePositionIndicatorEffect(bool isOn)
    {
        if (isOn)
        {
            await this.indicator.transform
                .DOLocalMove(onPosition.localPosition, changingDuration)
                .SetEase(ease).AsyncWaitForCompletion().AsUniTask();
        }
        else
        {
            await this.indicator.transform
                .DOLocalMove(offPosition.localPosition, changingDuration)
                .SetEase(ease).AsyncWaitForCompletion().AsUniTask();
        }
    }
}
