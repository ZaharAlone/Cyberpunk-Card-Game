namespace CyberNet.Core.AbilityCard
{
    /// <summary>
    /// Компонент вещается на карту во время применения абилки,
    /// Служит для следующих целей:
    /// 1) Хранить данные абилки
    /// 2) Отменять выбранную абилку если понадобиться
    /// </summary>
    public struct AbilitySelectElementComponent
    {
        public AbilityCardContainer AbilityCard;
    }
}