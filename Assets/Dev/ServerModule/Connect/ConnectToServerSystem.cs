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
using CyberNet.Meta;

namespace CyberNet.Server
{
    [EcsSystem(typeof(ServerModule))]
    public class ConnectToServerSystem : IInitSystem, IDestroySystem, IRunSystem
    {
        private DataWorld _dataWorld;
        private bool _isOnServer;

        public Client client = new Client(16384);

        public void Init()
        {
            ConnectServerAction.ConnectServer += InitServer;
            ConnectServerAction.ConnectError += ConnectError;

            Log.Info = Debug.Log;
            Log.Warning = Debug.LogWarning;
            Log.Error = Debug.LogError;

            client.OnConnected += OnConnect;
            client.OnData += (message) => readDataMessage(message);
            client.OnDisconnected += ConnectError;
        }

        private void readDataMessage(ArraySegment<byte> message)
        {
            Debug.Log($"Read message {message}");

            NetworkReader.OnMsg(message);
        }

        private void InitServer()
        {
            PopupAction.WaitPopup("MainMenu/$connectServer_header");
            Application.runInBackground = true;

            client.Connect("localhost", 1337);
            _isOnServer = true;

            Debug.Log("Init Server Connect");
        }

        private void OnConnect()
        {
            Debug.Log("Client Connect");
            WaitConnectGameAction.Init?.Invoke();
        }

        private void ConnectError()
        {
            PopupAction.CloseWaitPopup?.Invoke();
            PopupAction.WarningPopup?.Invoke("MainMenu/$notConnectServer_header", "Global/$clear_key", "Global/$ok_name", PopupAction.CloseWarningPopup);
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



/*
Debug.Log("Client Connected");
var test = new TestStruct { NameCard = "Viking", Server = "Valhala", value = 12500 };
var date = JsonConvert.SerializeObject(test);
var sendData = new SendData { EntityId = "0", Type = test.GetType().ToString(), Data = date };
var data = NetworkWriter.Write(sendData);
client.Send(data);*/