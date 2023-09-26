using System;

namespace CyberNet.Meta
{
    public struct PopupConfimStruct
    {
        public string HeaderLoc;
        public string DescrLoc;
        public string ButtonConfimLoc;
        public string ButtonCancelLoc;
        public Action ButtonConfimAction;
        public Action ButtonCancelAction;
    }
}