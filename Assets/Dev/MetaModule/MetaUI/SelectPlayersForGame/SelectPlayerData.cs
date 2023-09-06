using System.Collections.Generic;
using CyberNet.Meta;
using UnityEngine.Serialization;

namespace CyberNet.Global
{
    public struct SelectPlayerData
    {
        public List<SelectLeaderData> SelectLeaders;
        [FormerlySerializedAs("PrevSelectLeader")]
        public SelectLeaderData prevSelectLeader;
    }
}