using System;

namespace AutoSplat
{
    internal struct ServiceInfo
    {
        public Type Type { get; }
        public string Contract { get; }

        public ServiceInfo(Type type, string contract)
        {
            Type = type;
            Contract = contract;
        }
    }
}
