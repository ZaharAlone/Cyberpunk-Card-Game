using I2.Loc;
using Sirenix.OdinInspector;
using UnityEngine;

namespace CyberNet.Core.EndTurnWarningPopup
{
    public class EndTurnWarningPopupUIMono : MonoBehaviour
    {
        [Header("Links")]
        [Required]
        [SerializeField]
        private GameObject _background;
        [Required]
        [SerializeField]
        private GameObject _panelEndTurnWarning;
        [Required]
        [SerializeField]
        private GameObject _panelSavedDontShow;
        
        [Required]
        [SerializeField]
        private Localize _descriptionPopup;

        [Header("Localize settings")]
        [SerializeField]
        private LocalizedString _unuseCardDescrLoc;
        [SerializeField]
        private LocalizedString _unuseMoneyDescrLoc;

        private void OnEnable()
        {
            ClosePopup();
        }

        public void OpenPopupUnuseCard()
        {
            _descriptionPopup.Term = _unuseCardDescrLoc.mTerm;
            OpenPopup();
        }

        public void OpenPopupUnuseMoney()
        {
            _descriptionPopup.Term = _unuseMoneyDescrLoc.mTerm;
            OpenPopup();
        }

        private void OpenPopup()
        {
            _background.SetActive(true);
            _panelEndTurnWarning.SetActive(true);
        }

        public void ClosePopup()
        {
            _background.SetActive(false);
            _panelEndTurnWarning.SetActive(false);
            _panelSavedDontShow.SetActive(false);
        }

        private void OpenSavedPopup()
        {
            _panelEndTurnWarning.SetActive(false);
            _panelSavedDontShow.SetActive(true);
        }

        public void OnClickYes()
        {
            ClosePopup();
            EndTurnWarningPopupAction.OnClickYes?.Invoke();
        }

        public void OnClickCancel()
        {
            ClosePopup();
        }

        public void OnClickSavedDontShow()
        {
            ClosePopup();
            EndTurnWarningPopupAction.OnClickDontShow?.Invoke();
        }

        public void OnClickDontShow()
        {
            OpenSavedPopup();
        }
    }
}