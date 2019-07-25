using System.Collections.Generic;
using System.Linq;
using System.Net;
using Vostok.Commons.Collections;
using Vostok.Commons.Helpers.Network;

namespace Vostok.Datacenters.Helpers
{
    internal static class LocalNetworksProvider
    {
        private static readonly CachingTransform<List<IPv4Network>, IPAddress[]> LocalNetworksTransform
            = new CachingTransform<List<IPv4Network>, IPAddress[]>(list => list.Select(x => x.NetworkAddress).ToArray());

        public static IPAddress[] Get() => LocalNetworksTransform.Get(NetworkHelper.LocalNetworks);
    }
}