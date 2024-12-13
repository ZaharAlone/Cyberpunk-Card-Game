using CyberNet.Core;
using CyberNet.Global;
using UnityEngine.Serialization;

namespace CyberNet.Meta
{
    public struct SelectLeaderData
    {
        public int PlayerID;
        public string NamePlayer;
        public PlayerOrAI PlayerOrAI;
        public string SelectLeader;
        public string KeyVisualCity;
    }
}