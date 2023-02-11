using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BoardGame.Core.UI
{
    public class ActionButtonView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public Animator ActionButtonAnimator;

        public void OnPointerEnter(PointerEventData eventData)
        {
            ActionButtonAnimator.SetTrigger("Select");
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            ActionButtonAnimator.SetTrigger("Unselect");
        }
    }
}