using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace  CyberNet.Meta
{
    public class LeaderAvatarButtonUIMono : MonoBehaviour
    {
        [Required]
        public Button Button;
        public Image ImageButton;
        public GameObject SelectLeaderImage;

        [SerializeField]
        private Color32 _baseColor;
        [SerializeField]
        private Color32 _deactiveColor;
        
        public string KeyLeaders;
        public bool IsFirstButton;
        
        public void Start()
        {
            ImageButton.sprite = SelectLeaderAction.InitButtonLeader?.Invoke(KeyLeaders, IsFirstButton);
        }
        
        public void OnClicked()
        {
            SelectLeaderAction.SelectLeader?.Invoke(KeyLeaders);
        }

        public void SelectButton()
        {
            SelectLeaderImage.SetActive(true);
            ImageButton.color = _baseColor;
            Button.Select();
        }

        public void DeselectButton()
        {
            SelectLeaderImage.SetActive(false);
            ImageButton.color = _deactiveColor;
        }
        
        #if UNITY_EDITOR
        [Button("Get Element")]
        public void GetElement()
        {
            Button = gameObject.GetComponent<Button>();
            ImageButton = gameObject.GetComponent<Image>();
        }
        #endif
    }   
}
