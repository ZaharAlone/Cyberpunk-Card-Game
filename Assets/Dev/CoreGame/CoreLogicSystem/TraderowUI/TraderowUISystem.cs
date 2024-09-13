using CyberNet.Core.Player;
using CyberNet.Core.UI;
using CyberNet.Global;
using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;

namespace CyberNet.Core.Traderow
{
    [EcsSystem(typeof(CoreModule))]
    public class TraderowUISystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;
        private bool _statusTraderow;
        
        public void PreInit()
        {
            BoardGameUIAction.UpdateStatsPlayersCurrency += CheckTraderow;
            TraderowUIAction.ShowTraderow += ShowTraderow;
            TraderowUIAction.ForceShowTraderow += ForceShowTraderow;
            TraderowUIAction.HideTraderow += HideTraderow;
            TraderowUIAction.ForceHideTraderow += ForceHideTraderow;
            TraderowUIAction.EndShowAnimations += EndShowAnimations;
        }
        
        private void ForceHideTraderow()
        {
            ref var uiTraderow = ref _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.TraderowMono;
            uiTraderow.DisableTradeRow();
        }

        private void ForceShowTraderow()
        {
            ref var uiTraderow = ref _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.TraderowMono;
            uiTraderow.EnableTradeRow();
        }

        private void CheckTraderow()
        {
            var tradePoint = CheckTradePoint();

            var playerComponent = _dataWorld.Select<PlayerComponent>()
                .With<CurrentPlayerComponent>()
                .SelectFirstEntity()
                .GetComponent<PlayerComponent>();

            if (playerComponent.playerOrAI != PlayerOrAI.Player)
                return;
            
            if (tradePoint && !_statusTraderow)
                ShowTraderowAnimation();
            else if (!tradePoint && _statusTraderow)
                HideTradeRowAnimation();
        }

        private void ShowTraderow()
        {
            if (CheckTradePoint())
                return;
            
            ShowTraderowAnimation();
        }

        private void ShowTraderowAnimation()
        {
            ref var uiTraderow = ref _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.TraderowMono;
            ref var roundData = ref _dataWorld.OneData<RoundData>();
            
            if (roundData.PauseInteractive)
                return;
            
            uiTraderow.ShowFullTradeRowPanelAnimations();
            _statusTraderow = true;
        }
        
        private void EndShowAnimations()
        {
            _dataWorld.NewEntity().AddComponent(new TraderowIsShowComponent());
        }

        private void HideTraderow()
        {
            if (CheckTradePoint())
                return;
            
            HideTradeRowAnimation();
        }

        private void HideTradeRowAnimation()
        {
            ref var uiTraderow = ref _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.TraderowMono;
            
            uiTraderow.TradeRowToMiniPanelAnimations();
            _statusTraderow = false;
            
            var traderowIsShowEntities = _dataWorld.Select<TraderowIsShowComponent>().GetEntities();
            foreach (var entity in traderowIsShowEntities)
            {
                entity.Destroy();
            }
        }

        private bool CheckTradePoint()
        {
            var countCardFreeToBuy = _dataWorld.Select<CardTradeRowComponent>()
                .With<CardFreeToBuyComponent>()
                .Count();
            
            return countCardFreeToBuy > 0;
        }

        public void Destroy()
        {
            BoardGameUIAction.UpdateStatsPlayersCurrency -= CheckTraderow;
            TraderowUIAction.ShowTraderow -= ShowTraderow;
            TraderowUIAction.ForceShowTraderow -= ForceShowTraderow;
            TraderowUIAction.HideTraderow -= HideTraderow;
            TraderowUIAction.ForceHideTraderow -= ForceHideTraderow;
            TraderowUIAction.EndShowAnimations -= EndShowAnimations;
        }
    }
}