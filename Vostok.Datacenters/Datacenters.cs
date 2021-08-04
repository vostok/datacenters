﻿using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly DnsResolver dnsResolver;

        private readonly string localDatacenter;
        private readonly string localHostname;

        /// We need to cache this Func because of the Select enumerators in <see cref="GetDatacenterInternal"/> and <see cref="GetLocalDatacenter"/> functions.
        /// To prevent our <code>GetDatacenter(IPAddress address)</code> function from wrapping itself into a new function every time it is called.
        private readonly Func<IPAddress, string> cachedGetDatacenterFunc;

        public Datacenters([NotNull] DatacentersSettings settings)
        {
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));

            localDatacenter = this.settings.LocalDatacenter
                              ?? Environment.GetEnvironmentVariable(LocalDatacenterVariable);

            localHostname = this.settings.LocalHostname
                            ?? EnvironmentInfo.FQDN;

            dnsResolver = new DnsResolver(settings.DnsCacheTtl, settings.DnsResolveTimeout);

            cachedGetDatacenterFunc = GetDatacenter;
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

            return LocalNetworksProvider
                .Get()
                .Select(cachedGetDatacenterFunc)
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
                .Select(cachedGetDatacenterFunc)
                .FirstOrDefault(x => x != null);
    }
}