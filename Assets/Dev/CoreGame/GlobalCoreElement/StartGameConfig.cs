using UnityEngine.Serialization;
namespace CyberNet.Core
{
    public struct StartGameConfig
    {
        public PlayerConfigStartGame Player_1;
        public PlayerConfigStartGame Player_2;
        public PlayerConfigStartGame Player_3;
        public PlayerConfigStartGame Player_4;
    }

    public struct PlayerConfigStartGame
    {
        public PlayerType PlayerType;
        public string Key;
        public string AvatarKey;
        public string Name;
    }

    public enum PlayerType
    {
        None,
        Player,
        AI
    }
}