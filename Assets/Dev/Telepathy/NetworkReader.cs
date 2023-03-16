using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Telepathy
{
    public class NetworkReader
    {
        private Dictionary<Type, Action<object>> _handlers = new Dictionary<Type, Action<object>>();

        public void OnMsg(SendData sendData)
        {
            var type = Type.GetType(sendData.Type);
            if (_handlers.TryGetValue(type, out var handler))
            {
                var deserialized = JsonConvert.DeserializeObject(sendData.Data, type);
                handler.Invoke(deserialized);
            }
        }

        public void RegisterHandle<T>(Action<T> onMsg)
        {
            var handler = new Action<object>(deserialized =>
            {
                onMsg.Invoke((T)deserialized);
            });
            _handlers.Add(typeof(T), handler);
        }
    }
}
