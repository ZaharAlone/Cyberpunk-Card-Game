using Newtonsoft.Json;
using System;
using System.Text;

namespace Telepathy
{
    public class NetworkWriter
    {
        public static ArraySegment<byte> Write<T>(T value)
        {
            var date = JsonConvert.SerializeObject(value);
            var sendData = new SendData { Type = value.GetType().ToString(), Data = date };
            return Convert(sendData);
        }

        private static ArraySegment<byte> Convert(SendData data)
        {
            var json = JsonConvert.SerializeObject(data);
            var packData = Encoding.ASCII.GetBytes(json);
            return new ArraySegment<byte>(packData);
        }
    }
}

[Serializable]
public struct SendData
{
    public string Type;
    public string Data;
}