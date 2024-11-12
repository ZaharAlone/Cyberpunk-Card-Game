using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System;
using CyberNet.Core.AbilityCard;
using CyberNet.Core.Map;
using CyberNet.Core.Player;
using CyberNet.Core.UI;

namespace CyberNet.Core
{
    /// <summary>
    /// Система считает бонусы Trade Point за контроль территории игроком
    /// </summary>
    [EcsSystem(typeof(CoreModule))]
    public class CityDistrictTradeBonusSystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        private const string trade_point_key = "Trade";
        
        public void PreInit()
        {
            CityAction.UpdateDistrictTradeBonus += UpdateDistrictTradeBonus;
        }

        public void UpdateDistrictTradeBonus()
        {
            ref var actionCardData = ref _dataWorld.OneData<ActionCardData>();
            var idCurrentPlayer = _dataWorld.Select<PlayerComponent>()
                .With<CurrentPlayerComponent>()
                .SelectFirstEntity()
                .GetComponent<PlayerComponent>()
                .PlayerID;

            var oldBonusValue = actionCardData.BonusDistrictTrade;
            var countTradePointBonusDistrict = 0;

            var districtEntities = _dataWorld.Select<DistrictComponent>()
                .Where<DistrictComponent>(district => district.DistrictBelongPlayerID == idCurrentPlayer
                && district.PlayerControlEntity == PlayerControlEntity.PlayerControl)
                .GetEntities();

            foreach (var districtEntity in districtEntities)
            {
                var districtComponent = districtEntity.GetComponent<DistrictComponent>();

                if (districtComponent.BonusDistrict.Item == trade_point_key)
                    countTradePointBonusDistrict += districtComponent.BonusDistrict.Value;
            }
            
            actionCardData.BonusDistrictTrade = countTradePointBonusDistrict;
            
            BoardGameUIAction.UpdateStatsPlayersCurrency?.Invoke();

            if (countTradePointBonusDistrict > oldBonusValue)
            {
                var gameUI = _dataWorld.OneData<CoreGameUIData>();
                gameUI.BoardGameUIMono.TraderowMono.PlayEffectAddTradePoint();   
            }
        }

        public void Destroy()
        {
            CityAction.UpdateDistrictTradeBonus -= UpdateDistrictTradeBonus;
        }
    }
}