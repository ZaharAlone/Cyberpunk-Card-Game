using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using Input;
using UnityEngine;
using System.Collections.Generic;
using CyberNet.Core.UI;
using Object = UnityEngine.Object;

namespace CyberNet.Core.BezierCurveNavigation
{
    [EcsSystem(typeof(CoreModule))]
    public class BezierCurveNavigationSystem : IPreInitSystem, IRunSystem
    {
        private DataWorld _dataWorld;
        
        private List<GameObject> graphPoints = new List<GameObject>(); //A list of all the points that are spawned to graph the curve
        
        public void PreInit()
        {
            BezierCurveNavigationAction.StartBezierCurve += StartBezier;
        }
        
        private void StartBezier()
        {
            var bezierEntity = _dataWorld.NewEntity();
            var bezierComponent = new BezierCurveNavigationComponent();
            
            bezierEntity.AddComponent(bezierComponent);
        }

        public void Run()
        {
            if (_dataWorld.Select<BezierCurveNavigationComponent>().Count() == 0)
                return;
            
            FollowMousePosition();
            //CheckExitCurve();
        }

        private void FollowMousePosition()
        {
            foreach (GameObject g in graphPoints)
            {
                Object.Destroy(g);
            }
            
            graphPoints.Clear();
            var input = _dataWorld.OneData<InputData>();
            var uiBezier = _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.TestBezierUIMono;
            
            uiBezier.ControlPoints[2].position = input.MousePosition;
            var distancePoint = Vector3.Distance(uiBezier.ControlPoints[0].position, uiBezier.ControlPoints[2].position);

            var distanceValue = Mathf.InverseLerp(100, 1200, distancePoint);
            var countPoint = (int)(Mathf.Lerp(0, 15, distanceValue));
            var bezierConfig = _dataWorld.OneData<BezierData>().BezierCurveConfigSO;
            //Loop through values of t to create the graph, spawning points at each step
            
            for (float i = 0; i < 1; i += 1f / countPoint)
            {
                var valuePosRot = BezierCalculateStatic.NOrderBezierInterp(uiBezier.ControlPoints, i);

                graphPoints.Add(Object.Instantiate(bezierConfig.BezierArrowPrefab.gameObject, valuePosRot.Item1, valuePosRot.Item2, uiBezier.Canvas));
            }
            
            if (countPoint == 0)
                return;
            var valuePosRotEndPoint = BezierCalculateStatic.NOrderBezierInterp(uiBezier.ControlPoints, 1);
            graphPoints.Add(Object.Instantiate(bezierConfig.BezierArrowPrefab.gameObject, valuePosRotEndPoint.Item1, valuePosRotEndPoint.Item2, uiBezier.Canvas));
        }
        
        private void CheckExitCurve()
        {
            var inputData = _dataWorld.OneData<InputData>();
            
            if (!inputData.RightClick)
                return;
            
            //Exit Curve
            //Return Card

            var entityBezier = _dataWorld.Select<BezierCurveNavigationComponent>().SelectFirstEntity();
            entityBezier.Destroy();
            
            foreach (GameObject g in graphPoints)
            {
                Object.Destroy(g);
            }
        }
    }
}