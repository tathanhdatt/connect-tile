using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay
{
    public class GameplayView : BaseView
    {
        [SerializeField] private Image fadeBg;
        [Header("Text")] 
        [SerializeField] private TMP_Text levelText;

        [Header("Button")] 
        [SerializeField] private Button settingButton;

        [Header("Cheat")] 
        [SerializeField] private Button cheatSubmitButton;
        [SerializeField] private TMP_InputField cheatLevel;
        [SerializeField] private Button cheatButton;

        public event Action OnClickCheatButton;
        public event Action OnClickCheatSubmitButton;
        public event Action OnClickSettingButton;

        protected override void Initialize()
        {
            base.Initialize();
            this.settingButton.onClick.AddListener(() => OnClickSettingButton?.Invoke());
            this.cheatButton.onClick.AddListener(() => OnClickCheatButton?.Invoke());
            this.cheatSubmitButton.onClick.AddListener(() => OnClickCheatSubmitButton?.Invoke());

#if UNITY_EDITOR
            this.cheatButton.gameObject.SetActive(true);
#endif
        }

        public override async void Show()
        {
            base.Show();
            await FadeOut();
        }

        public override async void Hide()
        {
            base.Hide();
            await FadeIn();
        }

        public void SetLevelText(string text)
        {
            this.levelText.SetText(text);
        }

        public void SetVisibilityCheatGroup(bool isActive)
        {
            this.cheatLevel.gameObject.SetActive(isActive);
        }

        public bool GetVisibilityCheatGroup()
        {
            return this.cheatLevel.gameObject.activeSelf;
        }

        public int GetCheatLevel()
        {
            if (cheatLevel.text.Length == 0)
            {
                return 1;
            }

            return int.Parse(cheatLevel.text);
        }

        private async UniTask FadeOut()
        {
            await this.fadeBg.DOFade(0, 0.2f)
                .AsyncWaitForCompletion().AsUniTask();
            this.fadeBg.gameObject.SetActive(false);
        }

        private async UniTask FadeIn()
        {
            this.fadeBg.gameObject.SetActive(true);
            await this.fadeBg.DOFade(1, 0.2f)
                .AsyncWaitForCompletion().AsUniTask();
        }
    }
}