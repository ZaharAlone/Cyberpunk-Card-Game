using System.Collections.Generic;
using CyberNet.Meta;

namespace CyberNet.Global
{
    public struct SelectPlayerData
    {
        public List<SelectLeaderData> SelectLeaders;
        public SelectLeaderData PrevSelectLeader;
    }
}