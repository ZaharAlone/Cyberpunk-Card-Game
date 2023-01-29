using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using ModulesFrameworkUnity;
using System.Collections.Generic;
using UnityEngine;

namespace BoardGame.Core
{
    [EcsSystem(typeof(BoardGameModule))]
    public class PlayersSystem : IActivateSystem
    {
        private DataWorld _dataWorld;

        public void Activate()
        {
            ref var config = ref _dataWorld.OneData<BoardGameData>().BoardGameRule;
            _dataWorld.CreateOneData(new PlayerStatsData { Influence = config.BaseInfluenceCount, Cyberpsychosis = config.BaseCyberpsychosisCount });
            _dataWorld.CreateOneData(new EnemyStatsData { Influence = config.BaseInfluenceCount, Cyberpsychosis = config.BaseCyberpsychosisCount });
        }
    }
}