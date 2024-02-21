using CyberNet.Core.City;

namespace CyberNet.Core.Arena
{
    public struct ArenaRoundData
    {
        public int CurrentPlayerID;
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