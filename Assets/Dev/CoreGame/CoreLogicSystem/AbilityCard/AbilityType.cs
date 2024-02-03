using System;

namespace CyberNet.Core.AbilityCard
{
    [Serializable]
    public enum AbilityType
    {
        None,
        Attack,
        Trade,
        DrawCard,
        DestroyCard,
        CloneCard,
        DestroyTradeCard,
        SwitchNeutralSquad,
        SwitchEnemySquad,
        DestroyNeutralSquad,
        DestroySquad,
        EnemyDiscardCard,
        SquadMove,
        SetIce,
        DestroyIce
    }
}