using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;

namespace CyberNet.Core.UI
{
    [EcsSystem(typeof(CoreModule))]
    public class WaitEndRoundSystem : IRunSystem
    {
        private DataWorld _dataWorld;

        public void Run()
        {
            var isEntity = _dataWorld.Select<WaitEndRoundComponent>().TrySelectFirstEntity(out var entity);

            if (isEntity)
            {
                var countMoveCard = _dataWorld.Select<CardMoveToDiscardComponent>().Count();
                if (countMoveCard == 0)
                {
                    entity.Destroy();
                    RoundAction.EndCurrentTurn?.Invoke();
                }
            }
        }
    }
}