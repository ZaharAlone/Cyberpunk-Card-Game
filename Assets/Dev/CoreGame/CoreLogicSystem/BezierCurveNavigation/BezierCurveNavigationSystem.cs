using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using Input;
using UnityEngine;
using System;
using System.Collections.Generic;

namespace CyberNet.Core.BezierCurveNavigation
{
    [EcsSystem(typeof(CoreModule))]
    public class BezierCurveNavigationSystem : IPreInitSystem, IRunSystem
    {
        private DataWorld _dataWorld;

        [Header("Config")]
        private GameObject plotPointPrefab; //The prefab for the point that will represent the curve
        private List<Transform> controlPoints = new List<Transform>(); //A list of control points. These can be any Transform

        [Header("No. points used to plot the curve")]
        private int pointCount = 20; //The number of points that should be plotted to graph the curve

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
            CheckExitCurve();
        }

        private void FollowMousePosition()
        {
            //Destroy previous points and clear the list
            /*
            foreach (GameObject g in graphPoints)
            {
                Destroy(g);
            }
            graphPoints.Clear();
*/
            if (pointCount < 2)
            {
                pointCount = 10;
                Debug.LogWarning("Point count cannot be less than 2");
            }
        
            //Loop through values of t to create the graph, spawning points at each step
            for (float i = 0; i < 1; i += 1f / pointCount)
            {
                var position = Bezier.NOrderBezierInterp(controlPoints, i);

                //graphPoints.Add(Instantiate(plotPointPrefab, position, Quaternion.identity));
            }
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
        }
    }
}