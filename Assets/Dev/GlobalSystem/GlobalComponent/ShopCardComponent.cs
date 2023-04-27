using System;
using System.Collections.Generic;

namespace BoardGame.Core
{
    [Serializable]
    public struct ShopCardComponent
    {
        public List<CardData> NeutralCardTradeRow;
        public List<CardData> CardTradeRow;
    }
}