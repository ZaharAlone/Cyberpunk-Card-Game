using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using Input;
using UnityEngine;
using System.Collections.Generic;
using CyberNet.Core.City;
using CyberNet.Core.UI;
using CyberNet.Global.GameCamera;
using Object = UnityEngine.Object;

namespace CyberNet.Core.BezierCurveNavigation
{
    [EcsSystem(typeof(CoreModule))]
    public class BezierCurveNavigationSystem : IPreInitSystem, IRunSystem
    {
        private DataWorld _dataWorld;
        
        private List<BezierArrowMono> graphPoints;
        private Vector2 _oldMousePos;
        
        public void PreInit()
        {
            BezierCurveNavigationAction.StartBezierCurve += StartBezier;
            BezierCurveNavigationAction.OffBezierCurve += OffBezierCurve;
        }
        
        private void StartBezier(Vector3 startPosition, BezierTargetEnum target)
        {
            var bezierEntity = _dataWorld.NewEntity();
            var bezierComponent = new BezierCurveNavigationComponent {Target = target};
            
            bezierEntity.AddComponent(bezierComponent);
            var uiBezier = _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.BezierCurveUIMono;
            uiBezier.ControlPoints[0].position = startPosition;
            graphPoints = new List<BezierArrowMono>();
        }

        public void Run()
        {
            if (_dataWorld.Select<BezierCurveNavigationComponent>().Count() == 0)
                return;
            
            UpdateMousePosition();
        }

        private void UpdateMousePosition()
        {
            var input = _dataWorld.OneData<InputData>();
            
            if (input.MousePosition == _oldMousePos)
                return;
            _oldMousePos = input.MousePosition;

            UpdateBezierPoint();
            UpdateBezierVector();
            CheckTarget();
        }

        private void UpdateBezierPoint()
        {
            var input = _dataWorld.OneData<InputData>();
            foreach (var point in graphPoints)
            {
                Object.Destroy(point.gameObject);
            }
            
            graphPoints.Clear();
            
            var uiBezier = _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.BezierCurveUIMono;
            uiBezier.ControlPoints[2].position = input.MousePosition;
            var distancePoint = Vector3.Distance(uiBezier.ControlPoints[0].position, uiBezier.ControlPoints[1].position) + Vector3.Distance(uiBezier.ControlPoints[1].position, uiBezier.ControlPoints[2].position);

            var distanceValue = Mathf.InverseLerp(100, 1200, distancePoint);
            var countPoint = (int)(Mathf.Lerp(0, 10, distanceValue));
            var bezierConfig = _dataWorld.OneData<BezierData>().BezierCurveConfigSO;
            
            //Loop through values of t to create the graph, spawning points at each step
            for (float i = 0.05f; i < 1; i += 1f / countPoint)
            {
                var valuePosRot = BezierCalculateStatic.NOrderBezierInterp(uiBezier.ControlPoints, i);

                graphPoints.Add(Object.Instantiate(bezierConfig.BezierArrowPrefab, valuePosRot.Item1, valuePosRot.Item2, uiBezier.Canvas));
            }
            
            if (countPoint == 0)
                return;
            
            var valuePosRotEndPoint = BezierCalculateStatic.NOrderBezierInterp(uiBezier.ControlPoints, 1);
            graphPoints.Add(Object.Instantiate(bezierConfig.BezierArrowPrefab, valuePosRotEndPoint.Item1, valuePosRotEndPoint.Item2, uiBezier.Canvas));

        }

        private void UpdateBezierVector()
        {
            var uiBezier = _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.BezierCurveUIMono;

            var distancePointX = uiBezier.ControlPoints[2].position.x - uiBezier.ControlPoints[0].position.x;
            var distanceNormalizeX = Mathf.InverseLerp(-150, 150, distancePointX);
            var pozitionX = (int)(Mathf.Lerp(300, -300, distanceNormalizeX));
            
            var distancePointY = uiBezier.ControlPoints[2].position.y - uiBezier.ControlPoints[0].position.y;
            var distanceNormalizeY = Mathf.InverseLerp(0, 400, distancePointY);
            var pozitionY = (int)(Mathf.Lerp(100, 0, distanceNormalizeY));
            
            var pos = uiBezier.ControlPoints[1].anchoredPosition;
            pos.x = pozitionX;
            pos.y = pozitionY;
            uiBezier.ControlPoints[1].anchoredPosition = pos;
        }

        private void CheckTarget()
        {
            var bezierEntity = _dataWorld.Select<BezierCurveNavigationComponent>().SelectFirstEntity();
            var bezierComponent = bezierEntity.GetComponent<BezierCurveNavigationComponent>();

            switch (bezierComponent.Target)
            {
                case BezierTargetEnum.Tower:
                    CheckTowerTarget();
                    break;
                case BezierTargetEnum.Unit:
                    break;
                case BezierTargetEnum.Card:
                    break;
                case BezierTargetEnum.Player:
                    break;
            }
        }

        private void CheckTowerTarget()
        {
            var inputData = _dataWorld.OneData<InputData>();
            var camera = _dataWorld.OneData<GameCameraData>();
            var ray = camera.MainCamera.ScreenPointToRay(inputData.MousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, 1500f))
            {
                var towerMono = hit.collider.gameObject.GetComponent<TowerMono>();
                if (towerMono)
                {
                    UpdateVisualBezierColor(BezierCurveStatusEnum.SelectCurrentTarget);
                }
                else
                {
                    UpdateVisualBezierColor(BezierCurveStatusEnum.Base);
                }
            }
        }

        public void UpdateVisualBezierColor(BezierCurveStatusEnum status)
        {
            var bezierConfig = _dataWorld.OneData<BezierData>().BezierCurveConfigSO;
            var color = new Color32();
            
            switch (status)
            {
                case BezierCurveStatusEnum.Base:
                    color = bezierConfig.BaseArrowColor;
                    break;
                case BezierCurveStatusEnum.SelectCurrentTarget:
                    color = bezierConfig.SelectCurrentTargetArrowColor;
                    break;
                case BezierCurveStatusEnum.SelectWrongTarget:
                    color = bezierConfig.SelectWrongTargetArrowColor;
                    break;
            }

            foreach (var arrow in graphPoints)
            {
                arrow.SetColorArrow(color);
            }
        }
        
        private void OffBezierCurve()
        {
            var entityBezier = _dataWorld.Select<BezierCurveNavigationComponent>().SelectFirstEntity();
            entityBezier.Destroy();
            
            foreach (var point in graphPoints)
            {
                Object.Destroy(point.gameObject);
            }
        }
    }
}