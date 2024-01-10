using UnityEngine;
namespace CyberNet.Global.Cursor
{
    [CreateAssetMenu(fileName = "CursorConfigSO", menuName = "Scriptable Object/Board Game/CursorConfigSO")]
    public class CursorConfigSO : ScriptableObject
    {
        public Texture2D BaseCursorTexture;
        public Texture2D AimCursorTexture;
    }
}