using CyberNet.Global;
using UnityEngine.Serialization;
namespace CyberNet.Core
{
    public struct RoundData
    {
        public int CurrentRound;
        public int CurrentTurn;
        public int CurrentPlayerID;
        [FormerlySerializedAs("PlayerType")]
        public PlayerTypeEnum playerTypeEnum;
        public bool EndPreparationRound;
    }
}