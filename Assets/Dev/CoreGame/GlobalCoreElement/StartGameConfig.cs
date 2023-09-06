using UnityEngine.Serialization;
namespace CyberNet.Global
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
        None = 0,
        Player = 1,
        AIEasy = 2,
        AIMedium = 3,
        AIHard = 4
    }
}