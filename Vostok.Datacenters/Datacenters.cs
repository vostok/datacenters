using System;
using System.Collections.Generic;
using System.Net;
using JetBrains.Annotations;
using Vostok.Commons.Environment;
using Vostok.Commons.Helpers.Network;
using Vostok.Datacenters.Helpers;

namespace Vostok.Datacenters
{
    /// <inheritdoc/>
    [PublicAPI]
    public class Datacenters : IDatacenters
    {
        public const string LocalDatacenterVariable = "VOSTOK_LOCAL_DATACENTER";

        private readonly DatacentersSettings settings;
        private readonly ResolveHostname resolveHostname;

        private readonly string localDatacenter;
        private readonly string localHostname;

        public Datacenters([NotNull] DatacentersSettings settings)
        {
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));

            localDatacenter = this.settings.LocalDatacenter
                              ?? Environment.GetEnvironmentVariable(LocalDatacenterVariable);

            localHostname = this.settings.LocalHostname
                            ?? EnvironmentInfo.FQDN;

            resolveHostname = settings.ResolveHostname ?? new DnsResolver(settings.DnsCacheTtl, settings.DnsResolveTimeout).Resolve;
        }

        public string GetLocalDatacenter()
        {
            if (localDatacenter != null)
                return localDatacenter;

            if (localHostname != null)
            {
                var d = GetDatacenter(localHostname);
                if (d != null)
                    return d;
            }

            return GetDatacenterInternal(LocalNetworksProvider.Get());
        }

        public string GetDatacenter(IPAddress address)
            => settings.DatacenterMapping(address);

        public string GetDatacenter(string hostname)
            => IPAddress.TryParse(hostname, out var address)
                ? GetDatacenter(address)
                : GetDatacenterInternal(hostname, true);

        public string GetDatacenterWeak(string hostname)
            => IPAddress.TryParse(hostname, out var address)
                ? GetDatacenter(address)
                : GetDatacenterInternal(hostname, false);

        public IReadOnlyCollection<string> GetActiveDatacenters() =>
            settings.ActiveDatacentersProvider() ?? Array.Empty<string>();

        private string GetDatacenterInternal(string hostname, bool canWaitForDnsResolution) =>
            GetDatacenterInternal(resolveHostname(hostname, canWaitForDnsResolution));

        //Don't even dare to press your Alt + Enter on this code, it's ugly for perf reasons.
        private string GetDatacenterInternal(IPAddress[] ipAddresses)
        {
            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < ipAddresses.Length; i++)
            {
                var datacenter = GetDatacenter(ipAddresses[i]);
                if (datacenter != null)
                    return datacenter;
            }

            return default;
        }
    }
}