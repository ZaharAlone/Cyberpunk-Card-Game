using System;
using UnityEngine.Serialization;

namespace CyberNet.Core
{
    [Serializable]
    public struct PlayerViewComponent
    {
        [FormerlySerializedAs("Key")]
        public string LeaderKey;
        public string Name;
        public string AvatarKey;
    }
}