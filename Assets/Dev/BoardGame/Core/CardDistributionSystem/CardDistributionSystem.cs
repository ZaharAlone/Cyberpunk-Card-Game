using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;

namespace BoardGame.Core
{
    [EcsSystem(typeof(BoardGameModule))]
    public class CardDistributionSystem : IPostRunSystem
    {
        private DataWorld _dataWorld;

        public void PostRun()
        {
            var entities = _dataWorld.Select<EventEndCurrentTurn>().GetEntities();

            foreach (var entity in entities)
            {
                var round = _dataWorld.OneData<RoundData>();
            }
        }
    }
}