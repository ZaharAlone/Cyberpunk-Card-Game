using System;
using System.Collections.Generic;

namespace CyberNet.Core
{
	[Serializable]
	public struct PlayersComponent
	{
		public PlayerData Player1;
		public PlayerData Player2;
	}

	[Serializable]
	public struct PlayerData
    {
		public int HP;
		public int Cyberpsychosis;
		public List<CardData> DeckCard;
	}
}