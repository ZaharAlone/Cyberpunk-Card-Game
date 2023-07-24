using System;
using System.Collections.Generic;

namespace CyberNet.Core
{
    [Serializable]
    public struct ShopCardComponent
    {
        public List<CardData> NeutralCardTradeRow;
        public List<CardData> CardTradeRow;
    }
}