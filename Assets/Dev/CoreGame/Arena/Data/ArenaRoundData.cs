using CyberNet.Core.City;
using UnityEngine.Serialization;

namespace CyberNet.Core.Arena
{
    public struct ArenaRoundData
    {
        public int CurrentPlayerID;
        [FormerlySerializedAs("playerControlTower")]
        [FormerlySerializedAs("PlayerControlEnum")]
        public PlayerControlEntity PlayerControlEntity;
        public ArenaCurrentStageEnum ArenaCurrentStage;
    }

    public enum ArenaCurrentStageEnum
    {
        Action,
        Reaction,
        CheckEnd
    }
}