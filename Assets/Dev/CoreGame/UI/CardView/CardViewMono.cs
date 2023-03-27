using UnityEngine;

namespace BoardGame.Core.UI
{
    public class CardViewMono : MonoBehaviour
    {
        public void OnClick()
        {
            InteractiveActionCard.HideViewCard?.Invoke();
        }
    }
}