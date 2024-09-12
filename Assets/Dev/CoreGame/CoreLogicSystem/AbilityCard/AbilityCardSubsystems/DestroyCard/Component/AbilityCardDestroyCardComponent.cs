using System.Collections.Generic;

namespace CyberNet.Core.AbilityCard
{
    public struct AbilityCardDestroyCardComponent
    {
        // Параметр счетчик для многочисленных действий, когда нужно уничтожить несколько карт
        public int CountUseElement;
        // Список карт которые должны быть уничтожены, если потребуется отменить абилку.
        public List<string> GUIDCardDestroyList;
    }
}