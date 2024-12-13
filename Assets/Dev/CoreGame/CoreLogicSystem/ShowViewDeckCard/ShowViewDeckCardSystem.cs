using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;

namespace CyberNet.Core.UI
{
    [EcsSystem(typeof(CoreModule))]
    public class ShowViewDeckCardSystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            ShowViewDeckCardAction.OpenDiscard += OpenDiscard;
            ShowViewDeckCardAction.OpenDraw += OpenDraw;
            ShowViewDeckCardAction.CloseView += CloseView;
        }

        private void OpenDiscard()
        {
            var showViewUI = _dataWorld.OneData<CoreGameUIData>().ViewDeckCard;
            var currentPlayerID = _dataWorld.OneData<RoundData>().CurrentPlayerID;
            var entitiesCardDiscard = _dataWorld.Select<CardComponent>()
                .Where<CardComponent>(card => card.PlayerID == currentPlayerID)
                .With<CardDiscardComponent>()
                .GetEntities();

            foreach (var entity in entitiesCardDiscard)
            {
                ref var cardComponent = ref entity.GetComponent<CardComponent>();
                showViewUI.SetCardInContainer(cardComponent.CardMono.CardFace.gameObject);
            }
            
            showViewUI.SetOpenWindowDiscard();
        }

        private void OpenDraw()
        {
            var showViewUI = _dataWorld.OneData<CoreGameUIData>().ViewDeckCard;
            var currentPlayerID = _dataWorld.OneData<RoundData>().CurrentPlayerID;
            var entitiesCardDiscard = _dataWorld.Select<CardComponent>()
                .Where<CardComponent>(card => card.PlayerID == currentPlayerID)
                .With<CardDrawComponent>()
                .GetEntities();

            foreach (var entity in entitiesCardDiscard)
            {
                ref var cardComponent = ref entity.GetComponent<CardComponent>();
                showViewUI.SetCardInContainer(cardComponent.CardMono.CardFace.gameObject);
            }
            
            showViewUI.SetOpenWindowDraw();
        }

        private void CloseView()
        {

        }

        public void Destroy()
        {
            ShowViewDeckCardAction.OpenDiscard -= OpenDiscard;
            ShowViewDeckCardAction.OpenDraw -= OpenDraw;
            ShowViewDeckCardAction.CloseView -= CloseView;
        }
    }
}