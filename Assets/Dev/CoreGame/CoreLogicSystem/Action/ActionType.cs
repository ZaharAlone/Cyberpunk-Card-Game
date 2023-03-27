using System;

namespace BoardGame.Core
{
    [Serializable]
    public enum ActionType
    {
        PlayAll,
        Attack,
        EndTurn
    }
}