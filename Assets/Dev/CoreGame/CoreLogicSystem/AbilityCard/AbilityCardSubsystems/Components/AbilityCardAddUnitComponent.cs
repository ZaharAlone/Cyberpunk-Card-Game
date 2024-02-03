using System.Collections.Generic;

namespace CyberNet.Core.AbilityCard
{
    public struct AbilityCardAddUnitComponent
    {
        // Параметр счетчик для многочисленных действий, к при для 2х размещений или нескольких перемещений
        public int CountUseElement;
        // Список территорий куда были добавленны юниты. Если потребуется отменить абилку.
        public List<string> ListTowerAddUnit;
    }
}