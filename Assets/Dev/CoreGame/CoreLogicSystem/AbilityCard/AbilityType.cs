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
        PostAgent,
        ReturnAgent,
        DrawCardEnemyDiscardCard,
        DestroyNeutralSquad,
        DestroyEnemySquad,
        DestroyEnemyAgentPresence,
        PostSquad,
        AddNoiseCard,
        EnemyDiscardCard,
    }
}