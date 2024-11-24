using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using CyberNet.Core.Battle.CutsceneArena;
using CyberNet.Core.Battle.TacticsMode;
using CyberNet.Core.Battle.TacticsMode.InteractiveCard;
using CyberNet.Core.UI;
using CyberNet.Global;
using UnityEngine;

namespace CyberNet.Core.Battle
{
    [EcsSystem(typeof(CoreModule))]
    public class StartBattleSystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            BattleTacticsUIAction.OnClickStartBattle += StartBattle;
            BattleAction.StartBattleInMap += StartBattleInMap;
        }

        private void StartBattle()
        {
            MoveAllSelectCardToDiscard();
            StartBattleCutscene();
        }

        private void MoveAllSelectCardToDiscard()
        {
            var playerInBattleEntities = _dataWorld.Select<PlayerInBattleComponent>()
                .With<SelectTacticsAndCardComponent>().GetEntities();

            foreach (var playerInBattleEntity in playerInBattleEntities)
            {
                var selectCardComponent = playerInBattleEntity.GetComponent<SelectTacticsAndCardComponent>();
                if (!string.IsNullOrEmpty(selectCardComponent.GUIDCard))
                {
                    var targetCardEntity = _dataWorld.Select<CardComponent>()
                        .Without<CardTacticsComponent>()
                        .Where<CardComponent>(card => card.GUID == selectCardComponent.GUIDCard)
                        .SelectFirstEntity();

                    targetCardEntity.RemoveComponent<CardHandComponent>();
                    //TODO дописать чтобы карты двигались в дискард без анимаций
                    targetCardEntity.AddComponent(new CardMoveToDiscardComponent());
                }
            }
            
            AnimationsMoveAtDiscardDeckAction.AnimationsMoveAtDiscardDeck?.Invoke();
            CardAnimationsHandAction.AnimationsFanCardInHand?.Invoke();
        }
        
        private void StartBattleCutscene()
        {
            BattleAction.CloseTacticsScreen?.Invoke();
            BattleCutsceneAction.StartCutscene?.Invoke();
        }
        
        private void StartBattleInMap()
        {
            Debug.LogError("Start battle in map");
            BattleAction.CalculateResultBattle?.Invoke();
            BattleAction.KillUnitInMapView?.Invoke();

        }
        
        public void Destroy()
        {
            BattleTacticsUIAction.OnClickStartBattle -= StartBattle;
        }
    }
}