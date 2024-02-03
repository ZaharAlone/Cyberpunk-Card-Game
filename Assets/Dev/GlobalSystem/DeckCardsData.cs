using System.Collections;
using System.Collections.Generic;
using System;

namespace CyberNet.Core
{
    [Serializable]
    public struct DeckCardsData
    {
        public List<CardData> ShopCards;
        public List<CardData> NeutralShopCards;
        public List<PlayerCardData> PlayerDeckCard;
    }
}