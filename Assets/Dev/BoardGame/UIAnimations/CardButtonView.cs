using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BoardGame.Core.UI
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