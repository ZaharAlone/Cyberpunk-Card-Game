using System;

namespace CyberNet.Core.AbilityCard
{
    [Serializable]
    public enum AbilityType
    {
        None,
        AddUnit,
        Trade,
        DrawCard,
        DestroyCard,
        CloneCard,
        DestroyTradeCard,
        SwitchNeutralUnit,
        SwitchEnemyUnit,
        DestroyNeutralUnit,
        DestroyUnit,
        EnemyDiscardCard,
        UnitMove,
        SetIce,
        DestroyIce,
        Grenade,
        HeadShot,
        LuckyShot,
        Cyberpsychosis,
        Shield,
        AddNoiseCard,
    }
}