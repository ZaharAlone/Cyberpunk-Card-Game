using System.Collections.Generic;
using CyberNet.Core.Arena.ArenaHUDUI;
using CyberNet.Core.City;
using UnityEngine;

namespace CyberNet.Core.Arena
{
    public struct PlayerArenaInBattleComponent
    {
        public PlayerControlEnum PlayerControlEnum;
        public int PlayerID;
        public ArenaContainerUICharacterMono ArenaContainerUICharacterMono;
        
        public bool Forwards;
        public Sprite Avatar;
        public string KeyCityVisual;
        public Color32 ColorVisual;
        
        public int PositionInTurnQueue;
    }
}