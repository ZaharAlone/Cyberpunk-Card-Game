using System;
using System.Collections.Generic;
using CyberNet.Core;
using ModulesFramework.Modules;

namespace EcsCore
{
    public class LocalGameModule : EcsModule
    {
        protected override Dictionary<Type, int> GetSystemsOrder()
        {
            return new Dictionary<Type, int>
            {
                { typeof(InitPlayersSystem), -100},
            };
        }
    }
}