using CyberNet.Core.City;

namespace CyberNet.Core.Arena
{
    public struct ArenaRoundData
    {
        public int CurrentPlayerID;
        public PlayerControlEnum PlayerControlEnum;
        public ArenaCurrentStageEnum ArenaCurrentStage;
    }

    public enum ArenaCurrentStageEnum
    {
        Action,
        Reaction,
        CheckEnd
    }
}