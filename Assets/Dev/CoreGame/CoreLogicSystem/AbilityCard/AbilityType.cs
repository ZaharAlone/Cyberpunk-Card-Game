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
        SwitchNeutralArmy,
        SwitchEnemyArmy,
        PostAgent,
        ReturnAgent,
        ReturnAgentGetTrade,
        ReturnAgentGetAttack,
        DrawCardEnemyDiscardCard,
        DestroyNeutralUnit,
        BuyCardPriceThreeGetHand,
        DestroyEnemyUnit,
        DestroyEnemyAgentPresence,
        PostUnit,
        DestroyTwoCardGetAttack,
        TradeAndGetNextBuyCard,
        DestroyEnemyAgentGetAttack,
        ReturnAgentIfEnemyGetAttack,
        AddNoiseCard,
        EnemyDiscardCard,
    }
}