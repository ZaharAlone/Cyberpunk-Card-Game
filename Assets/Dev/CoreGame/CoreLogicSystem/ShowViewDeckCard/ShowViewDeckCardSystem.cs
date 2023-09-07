using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System;

namespace CyberNet.Core.UI
{
    [EcsSystem(typeof(CoreModule))]
    public class System : IInitSystem
    {
        private DataWorld _dataWorld;

        public void Init()
        {
            ShowViewDeckCardAction.OpenDiscard += OpenDiscard;
            ShowViewDeckCardAction.OpenDraw += OpenDraw;
            ShowViewDeckCardAction.CloseView += CloseView;
        }

        private void OpenDiscard()
        {
            var showViewUI = _dataWorld.OneData<CoreGameUIData>().ViewDeckCard;
            var viewData = _dataWorld.OneData<CurrentPlayerViewScreenData>();
            var entitiesCardDiscard = _dataWorld.Select<CardComponent>()
                .Where<CardComponent>(card => card.PlayerID == viewData.CurrentPlayerID)
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
            var viewData = _dataWorld.OneData<CurrentPlayerViewScreenData>();
            var entitiesCardDiscard = _dataWorld.Select<CardComponent>()
                .Where<CardComponent>(card => card.PlayerID == viewData.CurrentPlayerID)
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
    }
}