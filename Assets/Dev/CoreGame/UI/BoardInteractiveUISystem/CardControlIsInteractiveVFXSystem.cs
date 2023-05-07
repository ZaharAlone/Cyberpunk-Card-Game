using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems.Events;

namespace CyberNet.Core.UI
{
    [EcsSystem(typeof(CoreModule))]
    public class CardControlIsInteractiveVFXSystem : IPostRunEventSystem<EventUpdateBoardCard>
    {
        private DataWorld _dataWorld;

        public void PostRunEvent(EventUpdateBoardCard _)
        {
            var viewPlayer = _dataWorld.OneData<ViewPlayerData>();
            var roundData = _dataWorld.OneData<RoundData>();
            if (roundData.CurrentPlayer == viewPlayer.PlayerView)
                UpdateVFX(viewPlayer.PlayerView);
        }

        private void UpdateVFX(PlayerEnum player)
        {
            var entitiesCardInHand = _dataWorld.Select<CardComponent>()
                                               .Where<CardComponent>(card => card.Player == player)
                                               .With<CardHandComponent>()
                                               .GetEntities();
            var entitiesCardInDeck = _dataWorld.Select<CardComponent>()
                                               .Where<CardComponent>(card => card.Player == player)
                                               .With<CardTableComponent>()
                                               .GetEntities();
            var entitiesCardInDrop = _dataWorld.Select<CardComponent>()
                                               .Where<CardComponent>(card => card.Player == player)
                                               .With<CardDiscardComponent>()
                                               .GetEntities();
            var entitiesCardInShop = _dataWorld.Select<CardComponent>().With<CardTradeRowComponent>().GetEntities();
            var actionValue = _dataWorld.OneData<ActionData>();
            var valueTrade = actionValue.TotalTrade - actionValue.SpendTrade;

            foreach (var entity in entitiesCardInHand)
            {
                ref var component = ref entity.GetComponent<CardComponent>().CardMono;
                component.SetStatusInteractiveVFX(true);
            }

            foreach (var entity in entitiesCardInDeck)
            {
                ref var component = ref entity.GetComponent<CardComponent>().CardMono;
                component.SetStatusInteractiveVFX(false);
            }

            foreach (var entity in entitiesCardInDrop)
            {
                ref var component = ref entity.GetComponent<CardComponent>().CardMono;
                component.SetStatusInteractiveVFX(false);
            }

            foreach (var entity in entitiesCardInShop)
            {
                ref var component = ref entity.GetComponent<CardComponent>();
                if (component.Price <= valueTrade)
                    component.CardMono.SetStatusInteractiveVFX(true);
                else
                    component.CardMono.SetStatusInteractiveVFX(false);
            }
        }
    }
}