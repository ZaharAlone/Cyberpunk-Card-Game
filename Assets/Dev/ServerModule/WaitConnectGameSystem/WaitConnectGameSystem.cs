using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using ModulesFrameworkUnity;
using UnityEngine;
using Telepathy;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using CyberNet.Core;
using CyberNet.Core.AbilityCard;
using CyberNet.Core.Player;
using CyberNet.Core.UI;
using CyberNet.Meta;

namespace CyberNet.Server
{
    [EcsSystem(typeof(ServerModule))]
    public class WaitConnectGameSystem : IInitSystem
    {
        private DataWorld _dataWorld;

        public void Init()
        {
            WaitConnectGameAction.Init += InitWait;
        }

        private void InitWait()
        {
            Debug.Log("Wait Init");
            PopupAction.CloseWaitPopup?.Invoke();
            PopupAction.WaitPopup?.Invoke("MainMenu/$bt_searchOnlineGame_name");

            NetworkReader.RegisterHandle<RoundData>(InitRoundData);

            _dataWorld.CreateOneData(new DeckCardsData());
            NetworkReader.RegisterHandle<ShopCardComponent>(InitShopCard);
            NetworkReader.RegisterHandle<PlayerComponent>(InitPlayers);
            NetworkReader.RegisterHandle<ActionCardData>(InitActionData);

            NetworkReader.RegisterHandle<StartGameComponent>(StartGame);
        }

        private void StartGame(StartGameComponent _)
        {
            PopupAction.CloseWaitPopup?.Invoke();
            var menu = _dataWorld.OneData<MetaUIData>();
            menu.UIGO.SetActive(false);
            ModulesUnityAdapter.world.InitModule<CoreModule>(true);
            BoardGameUIAction.UpdateCountCardInHand?.Invoke();
        }

        private void InitRoundData(RoundData roundData)
        {
            Debug.Log("Get Round Data");
            _dataWorld.CreateOneData(roundData);
        }

        private void InitShopCard(ShopCardComponent shopCard)
        {
            Debug.Log("Get ShopCard");
            ref var deckCard = ref _dataWorld.OneData<DeckCardsData>();

            deckCard.NeutralShopCards = shopCard.NeutralCardTradeRow;
            deckCard.ShopCards = shopCard.CardTradeRow;
        }

        //TODO: вернуть как до сервера дойду
        private void InitPlayers(PlayerComponent playerComponent)
        {
            /*
            Debug.Log("Get Players info");
            ref var deckCard = ref _dataWorld.OneData<DeckCardsData>();

            deckCard.PlayerCards_1 = playerCard.Player1.DeckCard;
            deckCard.PlayerCards_2 = playerCard.Player2.DeckCard;

            ref var player1stats = ref _dataWorld.OneData<PlayerStatsComponent>();
            ref var player2stats = ref _dataWorld.OneData<Player2StatsData>();
            */
        }

        private void InitActionData(ActionCardData actionCardData)
        {
            Debug.Log("Get Action Data");
            _dataWorld.CreateOneData(actionCardData);
        }
    }
}