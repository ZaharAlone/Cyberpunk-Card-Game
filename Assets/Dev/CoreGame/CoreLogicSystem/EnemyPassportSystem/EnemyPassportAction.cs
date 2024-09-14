using System;

namespace CyberNet.Core.EnemyPassport
{
    public static class EnemyPassportAction
    {
        public static Action<int> SelectPlayerPassport;
        public static Action<int> UnselectPlayerPassport;
        public static Action<int> OnClickPlayerPassport;
    }
}