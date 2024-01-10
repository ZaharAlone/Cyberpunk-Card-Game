using UnityEngine;

namespace CyberNet.Tools
{
    public static class ConvertRectTransformPosition
    {
        //TODO: Лютый хардкод чтобы карта уходила в сброс, по другому делать будет долго
        public static Vector2 ConvertDiscardPosition(RectTransform to)
        {
            var height = Screen.height;
            var width = Screen.width;
            var targetAnchoredPosition = to.anchoredPosition;
            return new Vector2((width + targetAnchoredPosition.x) /2 - to.rect.width / 2, (-height + targetAnchoredPosition.y)/2 + to.rect.height -20);
        }
    }
}