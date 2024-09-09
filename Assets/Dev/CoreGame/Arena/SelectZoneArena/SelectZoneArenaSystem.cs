using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System.Collections.Generic;
using CyberNet.Core.BezierCurveNavigation;
using CyberNet.Global.Sound;
using Input;
using Object = UnityEngine.Object;

namespace CyberNet.Core.Arena.SelectZone
{
    [EcsSystem(typeof(CoreModule))]
    public class SelectZoneArenaSystem : IPreInitSystem, IInitSystem, IRunSystem, IDestroySystem
    {
        private DataWorld _dataWorld;
        
        private GameObject _selectZoneGrenade;
        private bool _enable;

        private int _layerMask;
        
        private List<BezierArrowMono> graphPoints;

        public void PreInit()
        {
            SelectZoneArenaAction.EnableSelectZone += EnableSelectZone;
            SelectZoneArenaAction.DisableSelectZone += DisableSelectZone;
        }

        public void Init()
        {
            _layerMask = LayerMask.GetMask("Ground");
        }
        
        private void EnableSelectZone()
        {
            var arenaConfig = _dataWorld.OneData<BoardGameData>().BoardGameConfig.ArenaConfigSO;
            _selectZoneGrenade = Object.Instantiate(arenaConfig.SelectZoneGrenade);
            _enable = true;

            var currentUnit = _dataWorld.Select<ArenaUnitComponent>()
                .With<ArenaUnitCurrentComponent>()
                .SelectFirstEntity()
                .GetComponent<ArenaUnitComponent>();
            
            StartBezier(currentUnit.UnitArenaMono.transform.position);
        }
        
        private void StartBezier(Vector3 startPosition)
        {
            Debug.LogError("start bezier");
            //bezierEntity.AddComponent(bezierComponent);
            var selectZoneBezierCurveMono = _dataWorld.OneData<ArenaData>().ArenaMono.SelectZoneBezierCurveMono;
            selectZoneBezierCurveMono.ControlPoints[0].position = startPosition;
            graphPoints = new List<BezierArrowMono>();
            
            SoundAction.PlaySound?.Invoke(_dataWorld.OneData<SoundData>().Sound.StartInteractiveCard);
        }


        public void Run()
        {
            if (!_enable)
                return;
            
            FollowMouseZone();
        }

        private void FollowMouseZone()
        {
            var inputData = _dataWorld.OneData<InputData>();
            var camera = _dataWorld.OneData<ArenaData>().ArenaMono.ArenaCameraMono.ArenaCamera;
            var ray = camera.ScreenPointToRay(inputData.MousePosition);

            if (Physics.Raycast(ray, out var hit, 1000, _layerMask))
            {
                _selectZoneGrenade.transform.position = hit.point;
                UpdateBezierPoint(hit.point);
            }
        }
        
        private void UpdateBezierPoint(Vector3 targetPosition)
        {
            foreach (var point in graphPoints)
            {
                Object.Destroy(point.gameObject);
            }
            Debug.LogError("update bezier");
            graphPoints.Clear();
            
            var selectZoneBezierCurveMono = _dataWorld.OneData<ArenaData>().ArenaMono.SelectZoneBezierCurveMono;
            selectZoneBezierCurveMono.ControlPoints[2].position = targetPosition;
            var distancePoint = Vector3.Distance(selectZoneBezierCurveMono.ControlPoints[0].position, selectZoneBezierCurveMono.ControlPoints[1].position)
                + Vector3.Distance(selectZoneBezierCurveMono.ControlPoints[1].position, selectZoneBezierCurveMono.ControlPoints[2].position);

            var distanceValue = Mathf.InverseLerp(100, 1200, distancePoint);
            var countPoint = (int)(Mathf.Lerp(0, 16, distanceValue));
            var bezierConfig = _dataWorld.OneData<BezierData>().BezierCurveConfigSO;
            
            //SoundMoveArrow(countPoint);
            
            //Loop through values of t to create the graph, spawning points at each step
            for (float i = 0.05f; i < 1; i += 1f / countPoint)
            {
                var valuePosRot = BezierCalculateStatic.NOrderBezierInterpTransform(selectZoneBezierCurveMono.ControlPoints, i);

                graphPoints.Add(Object.Instantiate(bezierConfig.BezierArrowPrefab, valuePosRot.Item1, valuePosRot.Item2));
            }

            if (countPoint == 0)
            {
                Debug.LogError("count point zero");
                return;
            }
            
            var valuePosRotEndPoint = BezierCalculateStatic.NOrderBezierInterpTransform(selectZoneBezierCurveMono.ControlPoints, 1);
            graphPoints.Add(Object.Instantiate(bezierConfig.BezierArrowPrefab, valuePosRotEndPoint.Item1, valuePosRotEndPoint.Item2));
        }
        
        private void DisableSelectZone()
        {
            Debug.LogError("Disable");
            _enable = false;
            Object.Destroy(_selectZoneGrenade);
        }

        public void Destroy()
        {
            SelectZoneArenaAction.EnableSelectZone -= EnableSelectZone;
            SelectZoneArenaAction.DisableSelectZone -= DisableSelectZone;
        }
    }
}