using CyberNet.Core.UI;
using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;

namespace CyberNet.Core.Traderow
{
    [EcsSystem(typeof(CoreModule))]
    public class TraderowUISystem : IPreInitSystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            TraderowUIAction.ShowTraderow += ShowTraderow;
            TraderowUIAction.HideTraderow += HideTraderow;
        }

        private void ShowTraderow()
        {
            ref var uiTraderow = ref _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.TraderowMono;
            ref var roundData = ref _dataWorld.OneData<RoundData>();
            
            if (roundData.EndPreparationRound)
                uiTraderow.ShowTraderow();
        }
        
        private void HideTraderow()
        {
            ref var uiTraderow = ref _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.TraderowMono;
            ref var roundData = ref _dataWorld.OneData<RoundData>();
            
            if (roundData.EndPreparationRound)
                uiTraderow.HideTraderow();
        }
    }
}