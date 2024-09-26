using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using Input;
using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
using CyberNet.Core.Arena;
using CyberNet.Core.Map;
using CyberNet.Core.EnemyPassport;
using CyberNet.Core.UI;
using CyberNet.Global.GameCamera;
using CyberNet.Global.Sound;
using Object = UnityEngine.Object;

namespace CyberNet.Core.BezierCurveNavigation
{
    [EcsSystem(typeof(CoreModule))]
    public class BezierCurveNavigationSystem : IPreInitSystem, IRunSystem, IDestroySystem
    {
        private DataWorld _dataWorld;
        
        private List<BezierArrowMono> graphPoints;
        private Vector2 _oldMousePos;

        private int _countPreviewArrow;
        private float _timePlaySoundArrow;
        
        public void PreInit()
        {
            BezierCurveNavigationAction.StartBezierCurve += StartBezier;
            BezierCurveNavigationAction.StartBezierCurveCard += StartBezierCurveCard;
            BezierCurveNavigationAction.OffBezierCurve += OffBezierCurve;
        }
        
        //Кривая базье что стартует от карты, используется для абилок
        private void StartBezierCurveCard(string guidCard, BezierTargetEnum target)
        {
            var positionCard = _dataWorld.Select<CardComponent>()
                .Where<CardComponent>(card => card.GUID == guidCard)
                .SelectFirstEntity()
                .GetComponent<CardComponent>()
                .RectTransform.position;
            
            StartBezier(positionCard, target);
        }

        private void StartBezier(Vector3 startPosition, BezierTargetEnum target)
        {
            var bezierEntity = _dataWorld.NewEntity();
            var bezierComponent = new BezierCurveNavigationComponent {Target = target};
            
            bezierEntity.AddComponent(bezierComponent);
            var uiBezier = _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.BezierCurveUIMono;
            uiBezier.ControlPoints[0].position = startPosition;
            graphPoints = new List<BezierArrowMono>();
            
            SoundAction.PlaySound?.Invoke(_dataWorld.OneData<SoundData>().Sound.StartInteractiveCard);
            
            if (target == BezierTargetEnum.Player)
                FollowPlayerTarget();
        }

        public void Run()
        {
            if (_dataWorld.Select<BezierCurveNavigationComponent>().Count() == 0)
                return;
            
            UpdateMousePosition();

            _timePlaySoundArrow -= Time.deltaTime;
        }

        private void UpdateMousePosition()
        {
            var input = _dataWorld.OneData<InputData>();
            
            if (input.MousePosition == _oldMousePos)
                return;
            _oldMousePos = input.MousePosition;

            UpdateBezierPoint(input.MousePosition);
            UpdateBezierVector();
            CheckIsSelectTarget();
        }

        private void UpdateBezierPoint(Vector3 targetPosition)
        {
            foreach (var point in graphPoints)
            {
                Object.Destroy(point.gameObject);
            }
            
            graphPoints.Clear();
            
            var uiBezier = _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.BezierCurveUIMono;
            uiBezier.ControlPoints[2].position = targetPosition;
            var distancePoint = Vector3.Distance(uiBezier.ControlPoints[0].position, uiBezier.ControlPoints[1].position) + Vector3.Distance(uiBezier.ControlPoints[1].position, uiBezier.ControlPoints[2].position);

            var distanceValue = Mathf.InverseLerp(100, 1200, distancePoint);
            var countPoint = (int)(Mathf.Lerp(0, 16, distanceValue));
            var bezierConfig = _dataWorld.OneData<BezierData>().BezierCurveConfigSO;
            
            SoundMoveArrow(countPoint);
            
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

        private void SoundMoveArrow(int newCountPoint)
        {
            //Смотрим изменилось ли кол-во стрелок с прошлого раза, было ли совершенно движение мышки по сути
            if (newCountPoint == _countPreviewArrow)
                return;
            
            //Прошел ли кулдаун с предыдущего трека, чтобы его не спавнить регулярно
            if (_timePlaySoundArrow > 0)
                return;
            
            //Задаем новый кулдаун на воспроизведение звука
            _timePlaySoundArrow = 0.5f;

            //Находим и воспроизводим звук
            var soundMoveArrow = _dataWorld.OneData<SoundData>().Sound.MoveArrowMap;
            SoundAction.PlaySound?.Invoke(soundMoveArrow);
            
            //Записываем кол-во текущих стрелок, чтобы сравнить с ним в следующий раз
            _countPreviewArrow = newCountPoint;
        }

        private void UpdateBezierVector()
        {
            var uiBezier = _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.BezierCurveUIMono;

            var distancePointX = uiBezier.ControlPoints[2].position.x - uiBezier.ControlPoints[0].position.x;
            var distanceNormalizeX = Mathf.InverseLerp(-150, 150, distancePointX);
            var positionX = (int)(Mathf.Lerp(300, -300, distanceNormalizeX));
            
            var distancePointY = uiBezier.ControlPoints[2].position.y - uiBezier.ControlPoints[0].position.y;
            var distanceNormalizeY = Mathf.InverseLerp(30, 250, distancePointY);
            var positionY = (int)(Mathf.Lerp(220, -100, distanceNormalizeY));
            
            var pos = uiBezier.ControlPoints[1].anchoredPosition;
            pos.x = positionX;
            pos.y = positionY;
            uiBezier.ControlPoints[1].anchoredPosition = pos;
        }

        private void CheckIsSelectTarget()
        {
            var bezierEntity = _dataWorld.Select<BezierCurveNavigationComponent>().SelectFirstEntity();
            var bezierComponent = bezierEntity.GetComponent<BezierCurveNavigationComponent>();

            switch (bezierComponent.Target)
            {
                case BezierTargetEnum.Tower:
                    CheckTowerTarget();
                    break;
                case BezierTargetEnum.Player:
                    // У игрока состояние меняется по подписке, поэтому меняем цеет относительно последнего состояния
                    if (bezierComponent.SelectTarget)
                        UpdateVisualBezierColor(BezierCurveStatusEnum.SelectCurrentTarget);
                    else
                        UpdateVisualBezierColor(BezierCurveStatusEnum.Base);
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
                    if (towerMono.IsInteractiveTower)
                    {
                        UpdateVisualBezierColor(BezierCurveStatusEnum.SelectCurrentTarget);
                        return;
                    }
                }
                
                UpdateVisualBezierColor(BezierCurveStatusEnum.Base);
            }
        }

        private void FollowPlayerTarget()
        {
            EnemyPassportAction.SelectPlayerPassport += FollowSelectPlayerPassport;
            EnemyPassportAction.UnselectPlayerPassport += FollowUnselectPlayerPassport;
        }

        private void FollowSelectPlayerPassport(int _)
        {
            UpdateVisualBezierColor(BezierCurveStatusEnum.SelectCurrentTarget);
            //TODO Add play VFX on Select player
        }

        private void FollowUnselectPlayerPassport(int _)
        {
            UpdateVisualBezierColor(BezierCurveStatusEnum.Base);
            //TODO Add play not playe VFX select player
        }
        
        private void UpdateVisualBezierColor(BezierCurveStatusEnum status)
        {
            ref var bezierComponent = ref _dataWorld.Select<BezierCurveNavigationComponent>()
                .SelectFirstEntity()
                .GetComponent<BezierCurveNavigationComponent>();
            
            var colorsConfig = _dataWorld.OneData<BoardGameData>().BoardGameConfig.ColorsGameConfigSO;
            var color = new Color32();
            
            switch (status)
            {
                case BezierCurveStatusEnum.Base:
                    color = colorsConfig.BaseColor;
                    bezierComponent.SelectTarget = false;
                    break;
                case BezierCurveStatusEnum.SelectCurrentTarget:
                    color = colorsConfig.SelectCurrentTargetBlueColor;
                    
                    if (bezierComponent.BezierCurveStatusEnum != status)
                    {
                        var soundSelectCurrentTarget = _dataWorld.OneData<SoundData>().Sound.SelectCurrentTargetInMap;
                        SoundAction.PlaySound?.Invoke(soundSelectCurrentTarget);
                    }
                    
                    bezierComponent.SelectTarget = true;
                    break;
                case BezierCurveStatusEnum.SelectWrongTarget:
                    color = colorsConfig.SelectWrongTargetRedColor;
                    bezierComponent.SelectTarget = true;
                    break;
            }

            foreach (var arrow in graphPoints)
            {
                arrow.SetColorArrow(color);
            }

            bezierComponent.BezierCurveStatusEnum = status;
        }
        
        private void OffBezierCurve()
        {
            var bezierQuery = _dataWorld.Select<BezierCurveNavigationComponent>();
            if (bezierQuery.Count() == 0)
                return;

            var bezierComponent = bezierQuery.SelectFirstEntity().GetComponent<BezierCurveNavigationComponent>();

            if (bezierComponent.Target == BezierTargetEnum.Player)
            {
                EnemyPassportAction.SelectPlayerPassport -= FollowSelectPlayerPassport;
                EnemyPassportAction.UnselectPlayerPassport -= FollowUnselectPlayerPassport;
            }
            
            bezierQuery.SelectFirstEntity().Destroy();
            
            foreach (var point in graphPoints)
            {
                Object.Destroy(point.gameObject);
            }
        }

        public void Destroy()
        {
            BezierCurveNavigationAction.StartBezierCurve -= StartBezier;
            BezierCurveNavigationAction.StartBezierCurveCard -= StartBezierCurveCard;
            BezierCurveNavigationAction.OffBezierCurve -= OffBezierCurve;
        }
    }
}