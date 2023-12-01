using UnityEngine;
namespace CyberNet.Global.Config
{
    [CreateAssetMenu(fileName = "ColorsGameConfigSO", menuName = "Scriptable Object/Board Game/Colors Game Config SO")]
    public class ColorsGameConfigSO : ScriptableObject
    {
        public Color32 BaseColor;
        public Color32 SelectCurrentTargetBlueColor;
        public Color32 SelectWrongTargetRedColor;
    }
}