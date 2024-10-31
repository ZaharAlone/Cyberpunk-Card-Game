using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CyberNet.Core.Battle.CutsceneArena
{
    [EcsSystem(typeof(CoreModule))]
    public class ArenaInitSystem : IInitSystem
    {
        private DataWorld _dataWorld;
        private readonly Vector3 _arenaPosition = new Vector3(-150f, 0f, 200f);

        public void Init()
        {
            CreateArenaStartCoreGame();
        }

        private void CreateArenaStartCoreGame()
        {
            var arenaMonoPrefab = _dataWorld.OneData<BoardGameData>().BoardGameConfig.ArenaMono;
            var arenaMonoInit = Object.Instantiate(arenaMonoPrefab);
            arenaMonoInit.transform.position = _arenaPosition;
            
            var arenaData = new ArenaData {
                ArenaMono = arenaMonoInit,
            };
            
            _dataWorld.CreateOneData(arenaData);            
        }
    }
}