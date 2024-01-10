using System;
using UnityEngine;

namespace CyberNet.Core.BezierCurveNavigation
{
    public static class BezierCurveNavigationAction
    {
        public static Action<Vector3, BezierTargetEnum> StartBezierCurve;
        public static Action OffBezierCurve;
    }
}