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

            this.settings.LocalDatacenter = this.settings.LocalDatacenter
                                            ?? Environment.GetEnvironmentVariable(Constants.LocalDatacenterVariable);

            this.settings.LocalHostname = this.settings.LocalHostname
                                          ?? Environment.GetEnvironmentVariable(Constants.LocalHostnameVariable);

            dnsResolver = new DnsResolver(settings.DnsCacheTtl, settings.DnsResolveTimeout);
        }

        public string GetLocalDatacenter()
        {
            if (settings.LocalDatacenter != null)
                return settings.LocalDatacenter;

            if (settings.LocalHostname != null)
            {
                var localDatacenter = GetDatacenter(settings.LocalHostname);
                if (localDatacenter != null)
                    return localDatacenter;
            }

            return LocalNetworksProvider
                .Get()
                .Select(GetDatacenter)
                .FirstOrDefault(datacenter => datacenter != null);
        }

        public string GetDatacenter(IPAddress address)
            => settings.DatacenterMapping(address);

        public string GetDatacenter(string hostname)
            => GetDatacenterInternal(hostname, true);

        public string GetDatacenterWeak(string hostname)
            => GetDatacenterInternal(hostname, false);

        public IReadOnlyCollection<string> GetActiveDatacenters() =>
            settings.ActiveDatacentersProvider() ?? Array.Empty<string>();

        private string GetDatacenterInternal(string hostname, bool canWaitForDnsResolution) =>
            dnsResolver
                .Resolve(hostname, canWaitForDnsResolution)
                .Select(GetDatacenter)
                .FirstOrDefault(x => x != null);
    }
}