using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using ModulesFrameworkUnity;
using UnityEngine;

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace BoardGame.Meta
{
    [EcsSystem(typeof(GlobalModule))]
    public class MainMenuSystem : IInitSystem, IDestroySystem, IRunSystem
    {
        private DataWorld _dataWorld;
        public TCP Tcp;

        private bool _isOnServer;

        public Telepathy.Client client = new Telepathy.Client(10000);

        public void Init()
        {
            Tcp = new TCP();
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
            client.OnConnected = () => Debug.Log("Client Connected");
            client.OnData = (message) => Debug.Log("Client Data: " + BitConverter.ToString(message.Array, message.Offset, message.Count));
            client.OnDisconnected = () => Debug.Log("Client Disconnected");

            ConnectToServer();
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

            // client
            if (client.Connected)
            {
                client.Send(new ArraySegment<byte>(new byte[] { 0x1 }));
            }

            // tick to process messages
            // (even if not connected so we still process disconnect messages)
            client.Tick(100);
        }

        public void Destroy()
        {
            client.Disconnect();
        }

        /*
        private void ConnectToServer()
        {
            Tcp.Connect();
            Debug.Log("Is Connect");
        }

        private async void ConnectServer()
        {
            foreach (var a in Dns.GetHostAddresses("localhost"))
                Debug.Log(a.ToString());

            var addresses = Dns.GetHostAddresses("localhost");
            var ipAddress = addresses[1];
            var endPoint = new IPEndPoint(ipAddress, 8701);
            using Socket sock = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            Debug.Log($"Try to connect to {endPoint}");

            await sock.ConnectAsync(endPoint);
            while (true)
            {
                var message = "Hi friends !<|EOM|>";
                var messageBytes = Encoding.UTF8.GetBytes(message);
                _ = await sock.SendAsync(messageBytes, SocketFlags.None);
                Debug.Log($"Socket client sent message: \"{message}\"");

                // Receive ack.
                var buffer = new byte[1_024];
                var received = await sock.ReceiveAsync(buffer, SocketFlags.None);
                var response = Encoding.UTF8.GetString(buffer, 0, received);
                if (response == "<|ACK|>")
                {
                    Debug.Log(
                        $"Socket client received acknowledgment: \"{response}\"");
                    break;
                }
            }
        }*/
    }
}