using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using ModulesFramework.Systems.Events;
using UnityEngine;

namespace BoardGame.Core
{
    [EcsSystem(typeof(BoardGameModule))]
    public class RoundSystem : IActivateSystem, IPostRunEventSystem<EventEndCurrentTurn>
    {
        private DataWorld _dataWorld;

        public void Activate()
        {
            var rules = _dataWorld.OneData<BoardGameData>().BoardGameRule;
            _dataWorld.CreateOneData(new RoundData { CurrentRound = 0, CurrentTurn = 1, CurrentPlayer = PlayerEnum.Player });
            _dataWorld.RiseEvent(new EventDistributionCard { Target = PlayerEnum.Player, Count = rules.CardInHandFirstPlayerOneRound });
        }
        //¬ключать когда подготовлю сервер, выбор первого игрока
        private PlayerEnum SelectFirstTurn()
        {
            var select = Random.Range(0, 2);
            if (select == 0)
                return PlayerEnum.Player;
            else
                return PlayerEnum.Enemy;
        }

        public void PostRunEvent(EventEndCurrentTurn _) => SwitchRound();

        private void SwitchRound()
        {
            ref var roundData = ref _dataWorld.OneData<RoundData>();
            if (roundData.CurrentTurn == 1)
            {
                roundData.CurrentRound++;
                roundData.CurrentTurn = 0;

            }
            else
                roundData.CurrentTurn++;

            if (roundData.CurrentPlayer == PlayerEnum.Player)
                roundData.CurrentPlayer = PlayerEnum.Enemy;
            else
                roundData.CurrentPlayer = PlayerEnum.Player;

            var rules = _dataWorld.OneData<BoardGameData>().BoardGameRule;
            _dataWorld.RiseEvent(new EventDistributionCard { Target = roundData.CurrentPlayer, Count = rules.BaseCountDropCard });
        }
    }
}