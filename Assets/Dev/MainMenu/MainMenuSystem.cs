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
    public class MainMenuSystem : IInitSystem
    {
        private DataWorld _dataWorld;

        public void Init()
        {
            MainMenuMono.ButtonStartGame += StartGame;
            MainMenuMono.ButtonConnectServer += ConnectServer;
        }

        private void StartGame()
        {
            var menu = _dataWorld.OneData<MainMenuData>();
            menu.UI.SetActive(false);
            EcsWorldContainer.World.InitModule<BoardGameModule>(true);
        }

        private async void ConnectServer()
        {
            foreach (var a in Dns.GetHostAddresses("localhost"))
                Debug.Log(a.ToString());

            var addresses = Dns.GetHostAddresses("localhost");
            var ipAddress = addresses[0];
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
        }
    }
}