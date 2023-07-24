using System;

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
	public enum CardStage
	{
		Hand,
		Deck,
		Table,
		Discard,
		Destroy
	}
}