using System;
namespace CyberNet.Core.UI.PopupDistrictInfo
{
    public static class PopupDistrictInfoAction
    {
        public static Action<string> OpenPopup;
        public static Action ClosePopup;
        public static Action ForceUpdateViewCurrentPopup;
    }
}