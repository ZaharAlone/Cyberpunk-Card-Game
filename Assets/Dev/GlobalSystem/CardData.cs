using System;
using System.Collections.Generic;

namespace CyberNet.Core
{
	[Serializable]
	public struct CardData
	{
		public string CardName;
		public int IDPositions;
	}

	[Serializable]
	public struct PlayerCardData
	{
		public int IndexPlayer;
		public List<CardData> Cards;
	}
}