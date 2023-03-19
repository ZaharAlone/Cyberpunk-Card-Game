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

namespace BoardGame.Meta
{
    [EcsSystem(typeof(MetaModule))]
    public class MainMenuSystem : IInitSystem, IDestroySystem, IRunSystem
    {
        private DataWorld _dataWorld;
        private bool _isOnServer;

        public Client client = new Client(10000);

        public void Init()
        {
            MainMenuMono.ButtonStartGame += StartGame;
            MainMenuMono.ButtonConnectServer += InitServer;
        }

        private void StartGame()
        {
            var menu = _dataWorld.OneData<MainMenuData>();
            menu.UI.SetActive(false);
            ModulesUnityAdapter.world.InitModule<BoardGameModule>(true);
        }

        private void InitServer()
        {
            // update even if window isn't focused, otherwise we don't receive.
            Application.runInBackground = true;

            // use Debug.Log functions for Telepathy so we can see it in the console
            Telepathy.Log.Info = Debug.Log;
            Telepathy.Log.Warning = Debug.LogWarning;
            Telepathy.Log.Error = Debug.LogError;

            // hook up events
            client.OnConnected += ActionConnetToServer;
            client.OnData = (message) => Debug.Log("Client Data: " + message);
            client.OnDisconnected = () => Debug.Log("Client Disconnected");

            ConnectToServer();
        }

        private void ActionConnetToServer()
        {
            Debug.Log("Client Connected");
            var test = new TestStruct { NameCard = "Viking", Server = "Valhala", value = 12500 };
            var date = JsonConvert.SerializeObject(test);
            var sendData = new SendData { EntityId = "0", Type = test.GetType().ToString(), Data = date };
            var data = NetworkWriter.Write(sendData);
            client.Send(data);
        }

        private void ConnectToServer()
        {
            client.Connect("localhost", 1337);
            _isOnServer = true;
        }

        public void Run()
        {
            if (!_isOnServer)
                return;

            client.Tick(100);
        }

        public void Destroy()
        {
            client.Disconnect();
        }
    }
}

[Serializable]
public struct TestStruct
{
    public string NameCard;
    public string Server;
    public int value;
}