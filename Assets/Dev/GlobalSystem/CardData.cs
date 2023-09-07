using System;
using System.Collections.Generic;

namespace CyberNet.Core
{
	[Serializable]
	public struct CardData
	{
		public string CardName;
		public int IDPositions;
		public CardStage PlayerStageCard;
	}

	[Serializable]
	public struct PlayerCardData
	{
		public int IndexPlayer;
		public List<CardData> Cards;
	}

	[Serializable]
	public enum CardStage
	{
		Hand,
		Deck,
		Table,
		Discard,
		Destroy
	}
}