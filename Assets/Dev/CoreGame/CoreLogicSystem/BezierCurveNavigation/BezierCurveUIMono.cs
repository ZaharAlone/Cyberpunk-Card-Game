using System.Collections.Generic;
using UnityEngine;

namespace CyberNet.Core.BezierCurveNavigation
{
    public class BezierCurveUIMono : MonoBehaviour
    {
        public List<RectTransform> ControlPoints = new List<RectTransform>();
        public Transform Canvas;
    }
}