using UnityEngine;

namespace CyberNet.Tools
{
    public static class ConvertRectTransformPosition
    {
        /*
        public static Vector2 Convert(RectTransform from, RectTransform to)
        {
            Vector2 localPoint;
            Vector2 fromPivotDerivedOffset = new Vector2(from.rect.width * from.pivot.x + from.rect.xMin, from.rect.height * from.pivot.y + from.rect.yMin);
            Vector2 screenP = RectTransformUtility.WorldToScreenPoint(null, from.position);
            screenP += fromPivotDerivedOffset;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(to, screenP, null, out localPoint);
            Vector2 pivotDerivedOffset = new Vector2(to.rect.width * to.pivot.x + to.rect.xMin, to.rect.height * to.pivot.y + to.rect.yMin);
            return to.anchoredPosition + localPoint - pivotDerivedOffset;
        }
        */
        public static Vector2 Convert( RectTransform fromPivotSpace, RectTransform toPivotSpace)
        {
            Vector2 convertedPos;
 
            Vector2 fromPivotDerivedOffset = new Vector2(fromPivotSpace.rect.width * fromPivotSpace.pivot.x + fromPivotSpace.rect.xMin, fromPivotSpace.rect.height * fromPivotSpace.pivot.y + fromPivotSpace.rect.yMin);
 
            Vector3 fromWorldSpace = fromPivotSpace.TransformPoint(fromPivotSpace.anchoredPosition);
            Vector2 screenP = RectTransformUtility.WorldToScreenPoint(null, fromWorldSpace);
            screenP += fromPivotDerivedOffset;
 
            RectTransformUtility.ScreenPointToLocalPointInRectangle(toPivotSpace, screenP, null, out convertedPos);
            Vector2 pivotDerivedOffset = new Vector2(toPivotSpace.rect.width * toPivotSpace.pivot.x + toPivotSpace.rect.xMin, toPivotSpace.rect.height * toPivotSpace.pivot.y + toPivotSpace.rect.yMin);
 
            return toPivotSpace.anchoredPosition + convertedPos - pivotDerivedOffset;
        }
    }
}