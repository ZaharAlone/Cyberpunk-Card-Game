using System.Collections.Generic;

namespace CyberNet.Core.AI.Ability
{
    /// <summary>
    /// Компонент содержит выбранные карты для сброса AI, чтобы сбросить их в ближайшее время.
    /// </summary>
    public struct DiscardCardAIComponent
    {
        public List<string> DiscardCard;
    }
}