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
using BoardGame.Meta;
using BoardGame.Core;

namespace BoardGame.Server
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
            Debug.LogError("Wait Init");
            PopupAction.CloseWaitPopup?.Invoke();
            PopupAction.WaitPopup?.Invoke("MainMenu/$bt_searchOnlineGame_name");

            NetworkReader.RegisterHandle<RoundData>(InitRoundData);

            _dataWorld.CreateOneData(new DeckCardsData());
            NetworkReader.RegisterHandle<ShopCardComponent>(InitShopCard);
            NetworkReader.RegisterHandle<PlayersComponent>(InitPlayerCard);
            NetworkReader.RegisterHandle<ActionData>(InitActionData);
            NetworkReader.RegisterHandle<StartGameComponent>(StartGame);
        }

        private void StartGame(StartGameComponent _)
        {
            PopupAction.CloseWaitPopup?.Invoke();
            var menu = _dataWorld.OneData<MainMenuData>();
            menu.UI.SetActive(false);
            ModulesUnityAdapter.world.InitModule<BoardGameModule>(true);
        }

        private void InitRoundData(RoundData roundData)
        {
            Debug.LogError("Get Round Data");
            _dataWorld.CreateOneData(roundData);

            var rules = _dataWorld.OneData<BoardGameData>().BoardGameRule;
            _dataWorld.RiseEvent(new EventDistributionCard { Target = roundData.CurrentPlayer, Count = rules.CountDropCard });
        }

        private void InitShopCard(ShopCardComponent shopCard)
        {
            Debug.LogError("Get ShopCard");
            ref var deckCard = ref _dataWorld.OneData<DeckCardsData>();

            deckCard.NeutralShopCards = shopCard.NeutralCardTradeRow;
            deckCard.ShopCards = shopCard.CardTradeRow;
        }

        private void InitPlayerCard(PlayersComponent playerCard)
        {
            Debug.LogError("Get Player Card");
            ref var deckCard = ref _dataWorld.OneData<DeckCardsData>();

            deckCard.PlayerCards_1 = playerCard.Player1.DeckCard;
            deckCard.PlayerCards_2 = playerCard.Player2.DeckCard;
        }

        private void InitActionData(ActionData actionData)
        {
            Debug.LogError("Get Action Data");
            _dataWorld.CreateOneData(actionData);
        }
    }
}