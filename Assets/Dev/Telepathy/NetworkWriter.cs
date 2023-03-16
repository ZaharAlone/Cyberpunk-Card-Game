using Newtonsoft.Json;
using System;
using System.Text;

namespace Telepathy
{
    public class NetworkWriter
    {
        public static ArraySegment<byte> Write(SendData data)
        {
            var json = JsonConvert.SerializeObject(data);
            var packData = Encoding.ASCII.GetBytes(json);
            return new ArraySegment<byte>(packData);
        }
    }

    [Serializable]
    public struct SendData
    {
        public string EntityId;
        public string Type;
        public string Data;
    }
}
