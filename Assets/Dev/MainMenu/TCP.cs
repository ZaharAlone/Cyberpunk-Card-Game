using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace BoardGame.Meta
{
    public class TCP
    {
        public TcpClient Socket;
        public static int DataBufferSize = 4096;
        public string IP = "127.0.0.1";
        public int Port = 8701;

        private NetworkStream _stream;
        private byte[] _receiveBuffer;

        public void Connect()
        {
            Socket = new TcpClient
            {
                ReceiveBufferSize = DataBufferSize,
                SendBufferSize = DataBufferSize
            };

            _receiveBuffer = new byte[DataBufferSize];
            Socket.BeginConnect(IP, Port, ConnectCallback, Socket);
        }

        private void ConnectCallback(IAsyncResult result)
        {
            Socket.EndConnect(result);

            if (!Socket.Connected)
                return;

            _stream = Socket.GetStream();
            _stream.BeginRead(_receiveBuffer, 0, DataBufferSize, ReceiveCallback, null);
        }

        private void ReceiveCallback(IAsyncResult result)
        {
            try
            {
                var byteLenght = _stream.EndRead(result);
                if (byteLenght <= 0)
                    return;
                var data = new byte[byteLenght];
                Array.Copy(_receiveBuffer, data, byteLenght);
                _stream.BeginRead(_receiveBuffer, 0, DataBufferSize, ReceiveCallback, null);
            }
            catch
            {

            }
        }
    }
}
