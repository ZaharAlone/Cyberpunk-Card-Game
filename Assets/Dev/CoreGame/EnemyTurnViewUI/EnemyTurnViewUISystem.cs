using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using System.Threading.Tasks;
using CyberNet.Core.Player;
using CyberNet.Core.UI;

namespace CyberNet.Core.EnemyTurnView
{
    [EcsSystem(typeof(CoreModule))]
    public class EnemyTurnViewUISystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        private bool _isShowPanelUI;

        public void PreInit()
        {
            EnemyTurnViewUIAction.PlayingCardShowView += ShowPlayingCard;
            EnemyTurnViewUIAction.PurchaseCardShowView += ShowPurchaseCard;
            EnemyTurnViewUIAction.HideView += HideView;
        }

        private async void ShowPlayingCard(string keyCard)
        {
            if (!_isShowPanelUI)
            {
                _isShowPanelUI = true;
                ShowUI(true);
                await Task.Delay(300);
            }

            var viewEnemyConfig = _dataWorld.OneData<BoardGameData>().BoardGameConfig.ViewEnemySO;
            var playerEnemyTurnActionUIMono = _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.CoreHudUIMono.PlayerEnemyTurnActionUIMono;
            var cardMono = playerEnemyTurnActionUIMono.CreateNewCard(viewEnemyConfig.CardForEnemyTurnView);
            
            SetupCardAction.SetViewCardNotInit?.Invoke(cardMono, keyCard);
        }

        private void ShowPurchaseCard(string keyCard)
        {
            
        }

        private void ShowUI(bool isPlayingCard)
        {
            var turnUI = _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.CoreHudUIMono.PlayerEnemyTurnActionUIMono;
            var viewEnemyConfig = _dataWorld.OneData<BoardGameData>().BoardGameConfig.ViewEnemySO;
            var currentPlayerEntity = _dataWorld.Select<PlayerComponent>()
                .With<CurrentPlayerComponent>()
                .SelectFirstEntity();
            var currentPlayerComponent = currentPlayerEntity.GetComponent<PlayerComponent>();
            var playerViewComponent = currentPlayerEntity.GetComponent<PlayerViewComponent>();
            ref var cityVisual = ref _dataWorld.OneData<BoardGameData>().CitySO;
            cityVisual.UnitDictionary.TryGetValue(playerViewComponent.KeyCityVisual, out var playerUnitVisual);
            
            var header = "";
            if (isPlayingCard)
                header = viewEnemyConfig.PlayingCardHeader;
            else
                header = viewEnemyConfig.BuyCardHeader;
            
            turnUI.SetViewPlayer(playerViewComponent.Avatar, playerViewComponent.Name, header, 
                playerUnitVisual.IconsUnit, playerUnitVisual.ColorUnit);
            turnUI.EnableFrame();
        }
        
        private async void HideView()
        {
            var turnUI = _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.CoreHudUIMono.PlayerEnemyTurnActionUIMono;
            turnUI.DisableFrame();
            await Task.Delay(300);
            turnUI.ClearContainerCard();
            _isShowPanelUI = false;
        }

        public void Destroy()
        {
            EnemyTurnViewUIAction.PlayingCardShowView -= ShowPlayingCard;
            EnemyTurnViewUIAction.PurchaseCardShowView -= ShowPurchaseCard;
            EnemyTurnViewUIAction.HideView -= HideView;
        }
    }
}