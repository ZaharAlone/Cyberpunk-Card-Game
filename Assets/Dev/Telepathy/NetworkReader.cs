using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Telepathy
{
    public static class NetworkReader
    {
        private static Dictionary<Type, Action<object>> _handlers = new Dictionary<Type, Action<object>>();

        public static void OnMsg(ArraySegment<byte> msg)
        {
            var message = Encoding.ASCII.GetString(msg);
            var sendData = JsonConvert.DeserializeObject<SendData>(message);

            var type = Type.GetType(sendData.Type);
            if (_handlers.TryGetValue(type, out var handler))
            {
                var deserialized = JsonConvert.DeserializeObject(sendData.Data, type);
                handler.Invoke(deserialized);
            }
        }

        public static void RegisterHandle<T>(Action<T> onMsg)
        {
            var handler = new Action<object>(deserialized =>
            {
                onMsg.Invoke((T)deserialized);
            });
            _handlers.Add(typeof(T), handler);
        }
    }
}