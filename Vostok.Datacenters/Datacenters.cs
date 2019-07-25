using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using JetBrains.Annotations;
using Vostok.Datacenters.Helpers;

namespace Vostok.Datacenters
{
    /// <inheritdoc/>
    [PublicAPI]
    public class Datacenters : IDatacenters
    {
        private readonly DatacentersSettings settings;
        private readonly DnsResolver dnsResolver;

        public Datacenters([NotNull] DatacentersSettings settings)
        {
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
            dnsResolver = new DnsResolver(settings.DnsCacheTtl, settings.DnsResolveTimeout);
        }

        public string GetLocalDatacenter() =>
            LocalNetworksProvider
                .Get()
                .Select(GetDatacenter)
                .FirstOrDefault(datacenter => datacenter != null);

        public string GetDatacenter(IPAddress address) =>
            settings.DatacenterMapping(address);

        public string GetDatacenter(string hostname) =>
            dnsResolver
                .Resolve(hostname)
                .Select(GetDatacenter)
                .FirstOrDefault(x => x != null);

        public IReadOnlyCollection<string> GetActiveDatacenters() =>
            settings.ActiveDatacentersProvider() ?? new string[] {};
    }
}