using System;
using CyberNet.Core.Map.InteractiveElement.Support;

namespace CyberNet.Core.Map.InteractiveElement
{
    public static class FollowSelectInteractiveMapAction
    {
        public static Action<TargetDistrictAndPlayerIDDTO> StartFollowSelectUnit;
        public static Action EndFollowSelectUnit;

        public static Action UpdateSelectUnit;
    }
}