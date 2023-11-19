using UnityEngine;
using UnityEngine.UI;

namespace CyberNet.Core.BezierCurveNavigation
{
    public class BezierArrowMono : MonoBehaviour
    {
        [SerializeField]
        private Image _imageArrow;

        public void SetColorArrow(Color32 color)
        {
            _imageArrow.color = color;
        }
    }
}