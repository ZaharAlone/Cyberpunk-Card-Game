using UnityEngine;
namespace CyberNet.Core.BezierCurveNavigation
{
    [CreateAssetMenu(fileName = "BezierCurveConfig", menuName = "Scriptable Object/Board Game/Bezier Curve Config")]
    public class BezierCurveConfigSO : ScriptableObject
    {
        public BezierArrowMono BezierArrowPrefab;
    }
}