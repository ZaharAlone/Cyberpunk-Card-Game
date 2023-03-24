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
        private bool _isOnServer;

        public Client client = new Client(10000);

        public void Init()
        {
            WaitConnectGameAction.Init += InitWait;
        }

        private void InitWait()
        {
            PopupAction.CloseWaitPopup?.Invoke();
            PopupAction.WaitPopup?.Invoke("MainMenu/$bt_searchOnlineGame_name");
            NetworkReader.RegisterHandle<RoundData>(InitRoundData);
        }

        private void InitRoundData(RoundData roundData)
        {

        }
    }
}