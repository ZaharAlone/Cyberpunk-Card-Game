using UnityEngine;
using UnityEngine.EventSystems;

namespace CyberNet.Core.UI
{
    public class CardButtonView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public Animator CardButtonAnimator;

        public void OnPointerEnter(PointerEventData eventData)
        {
            CardButtonAnimator.SetTrigger("Select");
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            CardButtonAnimator.SetTrigger("Unselect");
        }
    }
}