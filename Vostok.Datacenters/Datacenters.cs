using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using JetBrains.Annotations;
using Vostok.Commons.Environment;
using Vostok.Commons.Helpers.Network;
using Vostok.Commons.Threading;
using Vostok.Datacenters.Helpers;
using Vostok.Logging.Abstractions;

namespace Vostok.Datacenters
{
    /// <inheritdoc/>
    [PublicAPI]
    public class Datacenters : IDatacenters
    {
        public const string LocalDatacenterVariable = "VOSTOK_LOCAL_DATACENTER";

        private readonly DatacentersSettings settings;
        private readonly DnsResolver dnsResolver;
        private readonly ILog log;

        private readonly string localDatacenter;
        private readonly string localHostname;

        private bool isFirstLogging = new AtomicBoolean(true);

        public Datacenters([NotNull] DatacentersSettings settings)
        {
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
            
            log = settings.Log.ForContext<Datacenters>();
            
            localDatacenter = this.settings.LocalDatacenter
                              ?? Environment.GetEnvironmentVariable(LocalDatacenterVariable);

            localHostname = this.settings.LocalHostname
                            ?? EnvironmentInfo.FQDN;

            LogInitialHostname();

            dnsResolver = new DnsResolver(settings.DnsCacheTtl, settings.DnsResolveTimeout);
        }

        public string GetLocalDatacenter()
        {
            if (localDatacenter != null)
                return localDatacenter;

            var ips = LocalNetworksProvider.Get();

            LogIpList(ips);

            if (localHostname != null)
            {
                var d = GetDatacenter(localHostname);
                LogDcFromHostname(d);
                if (d != null)
                {
                    isFirstLogging = false;
                    return d;
                }
            }

            var dc = ips.Select(GetDatacenter)
                .FirstOrDefault(datacenter => datacenter != null);

            LogDcFromIp(dc);

            isFirstLogging = false;

            return dc;
        }

        public string GetDatacenter(IPAddress address)
            => settings.DatacenterMapping(address);

        public string GetDatacenter(string hostname)
            => GetDatacenterInternal(hostname, true);

        public string GetDatacenterWeak(string hostname)
            => GetDatacenterInternal(hostname, false);

        public IReadOnlyCollection<string> GetActiveDatacenters() =>
            settings.ActiveDatacentersProvider() ?? Array.Empty<string>();

        private string GetDatacenterInternal(string hostname, bool canWaitForDnsResolution)
        {
            var ip = dnsResolver
                .Resolve(hostname, canWaitForDnsResolution);

            LogIpListFromDnsResolver(hostname, ip);

                return ip.Select(GetDatacenter)
                .FirstOrDefault(x => x != null);
        }


        private void LogIpListFromDnsResolver(string hostname, IPAddress[] ips)
        {
            if(isFirstLogging)
                log.Info("{hostname} resolved in '{ips}'", hostname,string.Join(",", ips.Select(x => x.ToString())));
        }


        private void LogInitialHostname()
        {
            log.Info("LocalHostname = '{localHostname}'", localHostname);
        }

        private void LogDcFromIp(string dc)
        {
            if (isFirstLogging)
                log.Info("Dc from ip: '{dc}'", dc);
        }

        private void LogIpList(IPAddress[] ips)
        {
            if (isFirstLogging)
                log.Info("Ip from network provider: '{ips}'", string.Join(",", ips.Select(x => x.ToString())));
        }

        private void LogDcFromHostname(string d)
        {
            if (isFirstLogging)
                log.Info("Got dc from localHostname variable: '{d}'", d);
        }
    }
}