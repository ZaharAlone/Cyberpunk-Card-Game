using System;
using UnityEngine.Serialization;

namespace CyberNet.Core
{
    /// <summary>
    /// Текущий отображаемый UI игрока
    /// </summary>
    [Serializable]
    public struct CurrentPlayerViewScreenData
    {
        [FormerlySerializedAs("CurrentPlayerViewID")]
        public int CurrentPlayerID;
    }
}