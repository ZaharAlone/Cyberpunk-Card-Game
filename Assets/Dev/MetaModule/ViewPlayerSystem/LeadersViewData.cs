using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Serialization;

namespace CyberNet
{
    /// <summary>
    /// Словарь с артом для лидеров
    /// </summary>
    public struct LeadersViewData
    {
        public Dictionary<string, Sprite> LeadersView;
        public Sprite NeutralLeaderAvatar;
    }
}