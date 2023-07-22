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
        
        public string KeyLeaders;
        public bool IsFirstButton;
        
        public void Start()
        {
            //TO-DO убрать когда будет полный конфиг
            if (string.IsNullOrEmpty(KeyLeaders))
            {
                gameObject.SetActive(false);
                return;
            }

            Button.onClick.AddListener(OnClicked);
            ImageButton.sprite = SelectLeaderAction.InitButtonLeader?.Invoke(KeyLeaders, IsFirstButton);
        }
        
        private void OnClicked()
        {
            SelectLeaderAction.SelectLeader?.Invoke(KeyLeaders);
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
