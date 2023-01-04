using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using ModulesFrameworkUnity;
using System.Collections.Generic;
using UnityEngine;

namespace BoardGame.Core.UI
{
    [EcsSystem(typeof(BoardGameModule))]
    public class UIActionButtonSystem : IRunSystem
    {
        private DataWorld _dataWorld;

        public void Run()
        {
            var round = _dataWorld.OneData<RoundData>();
            if (round.CurrentPlayer != PlayerEnum.Player)
                return;

            var cardInHand = _dataWorld.Select<CardPlayerComponent>().With<CardInHandComponent>().Count();

            if (cardInHand > 0)
            {
                //Status button action - выложить карты
            }
            else
            {
                //Если есть атака, и атаковать возможно - атаковать
            }

            //закончить ход
        }
    }
}