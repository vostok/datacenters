using System;
using System.Collections.Generic;
using System.Net;
using JetBrains.Annotations;

namespace Vostok.Datacenters
{
    [PublicAPI]
    public class EmptyDatacenters : IDatacenters
    {
        public string GetLocalDatacenter() => null;

        public string GetDatacenter(IPAddress address) => null;

        public string GetDatacenter(string hostname) => null;

        public string GetDatacenterWeak(string hostname) => null;

        public IReadOnlyCollection<string> GetActiveDatacenters() => Array.Empty<string>();
    }
}