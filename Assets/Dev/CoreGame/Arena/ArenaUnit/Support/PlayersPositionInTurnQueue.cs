using System;

namespace CyberNet.Core.Arena.Support
{
    [Serializable]
    public struct PlayersPositionInTurnQueue
    {
        public int PlayerID;
        public int PositionInTurnQueue;
    }
}