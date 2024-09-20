using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using CyberNet.Core.Player;
using CyberNet.Core.UI;

namespace CyberNet.Core.AI.Ability
{
    [EcsSystem(typeof(CoreModule))]
    public class AbilityDiscardCardAISystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;
        
        public void PreInit()
        {
            AbilityAIAction.DiscardCardSelectPlayer += DiscardCardSelectPlayer;
        }
        
        //AI разыгрывает карту с абилкой сбросы карты, и выбирает игрока который будет сбрасывать карту
        private void DiscardCardSelectPlayer()
        {
            var playerEntities = _dataWorld.Select<PlayerComponent>()
                .Without<CurrentPlayerComponent>()
                .GetEntities();

            var targetPlayerID = 0;
            var maxVP = -1;
            
            foreach (var playerEntity in playerEntities)
            {
                var playerComponent = playerEntity.GetComponent<PlayerComponent>();
                if (playerComponent.VictoryPoint < maxVP)
                    continue;
                
                targetPlayerID = playerComponent.PlayerID;
                maxVP = playerComponent.VictoryPoint;
            }
            
            var targetPlayerEntity = _dataWorld.Select<PlayerComponent>()
                .Where<PlayerComponent>(player => player.PlayerID == targetPlayerID)
                .SelectFirstEntity();
            
            if (targetPlayerEntity.HasComponent<PlayerDiscardCardComponent>())
            {
                ref var playerDiscardCardComponent = ref targetPlayerEntity.GetComponent<PlayerDiscardCardComponent>();
                playerDiscardCardComponent.Count++;
            }
            else
            {
                targetPlayerEntity.AddComponent(new PlayerDiscardCardComponent {Count = 1});
            }
            
            BoardGameUIAction.UpdateStatsAllPlayersPassportUI?.Invoke();
        }

        public void Destroy()
        {
            AbilityAIAction.DiscardCardSelectPlayer -= DiscardCardSelectPlayer;
        }
    }
}