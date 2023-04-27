using System;

namespace BoardGame.Core
{
    public static class SelectFirstPlayer
    {
        public static PlayerEnum Select()
        {
            var random = new Random();
            var select = random.Next(0, 1);

            if (select == 0)
                return PlayerEnum.Player1;
            else
                return PlayerEnum.Player2;
        }
    }
}