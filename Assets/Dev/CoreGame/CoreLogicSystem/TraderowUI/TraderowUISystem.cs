using CyberNet.Core.UI;
using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;

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
            TraderowUIAction.HideTraderow += HideTraderow;
            TraderowUIAction.EndShowAnimations += EndShowAnimations;
        }

        private void CheckTraderow()
        {
            var tradePoint = CheckTradePoint();

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
            
            uiTraderow.ShowTraderow();
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
            
            uiTraderow.HideTraderow();
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
            TraderowUIAction.HideTraderow -= HideTraderow;
            TraderowUIAction.EndShowAnimations -= EndShowAnimations;
        }
    }
}