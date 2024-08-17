using Core;
using Core.Extension;
using MessageEvent;
using UnityEngine;

namespace Gameplay
{
    public class GameplayViewPresenter : BaseViewPresenter
    {
        private GameplayView gameplayView;
        private readonly GameData gameData;

        public GameplayViewPresenter(
            GamePresenter presenter,
            Transform transform,
            GameData gameData)
            : base(presenter, transform)
        {
            this.gameData = gameData;
        }

        protected override void AddViews()
        {
            this.gameplayView = AddView<GameplayView>();
            Messenger.AddListener<int>(EventKey.PlayLevel, PlayLevelHandler);
        }

        protected override void OnShow()
        {
            base.OnShow();
            this.gameplayView.OnClickSettingButton += ClickSettingButtonHandler;
            this.gameplayView.OnClickCheatButton += ClickCheatHandler;
            this.gameplayView.OnClickCheatSubmitButton += ClickCheatSubmitHandler;
        }

        protected override void OnHide()
        {
            base.OnHide();
            this.gameplayView.OnClickSettingButton -= ClickSettingButtonHandler;
            this.gameplayView.OnClickCheatButton -= ClickCheatHandler;
            this.gameplayView.OnClickCheatSubmitButton -= ClickCheatSubmitHandler;
        }

        private void PlayLevelHandler(int levelOrder)
        {
            this.gameplayView.SetLevelText($"Level {levelOrder.IntToText()}");
        }

        private void ClickSettingButtonHandler()
        {
            GamePresenter.GetViewPresenter<SettingViewPresenter>().Show();
        }

        private void ClickCheatSubmitHandler()
        {
#if UNITY_EDITOR
            Messenger.Broadcast(EventKey.ClearLevel);
            Messenger.Broadcast(EventKey.PlayLevel, this.gameplayView.GetCheatLevel());
#endif
        }

        private void ClickCheatHandler()
        {
#if UNITY_EDITOR
            var isCheatGroupActive = this.gameplayView.GetVisibilityCheatGroup();
            this.gameplayView.SetVisibilityCheatGroup(!isCheatGroupActive);
#endif
        }
    }
}