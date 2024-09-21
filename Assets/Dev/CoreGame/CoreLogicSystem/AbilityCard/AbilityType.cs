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
        SwitchUnit,
        EnemyDiscardCard,
        UnitMove,
        DestroyUnit,
        AddNoiseCard,
        SetIce,
        PowerPoint,
        KillPoint,
        DefencePoint,
        Disorientation,
        MoveEnemyUnit,
        TradeHack,
    }
}