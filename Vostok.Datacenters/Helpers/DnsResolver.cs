using System;
using System.Collections.Concurrent;
using System.Net;
using System.Threading.Tasks;
using Vostok.Commons.Threading;

namespace Vostok.Datacenters.Helpers
{
    internal class DnsResolver
    {
        private static readonly IPAddress[] EmptyAddresses = {};

        private readonly TimeSpan cacheTtl;
        private readonly TimeSpan resolveTimeout;
        private readonly ConcurrentDictionary<string, (IPAddress[] addresses, DateTime validTo)> cache;
        private volatile AtomicBoolean isUpdatingNow = false;

        public DnsResolver(TimeSpan cacheTtl, TimeSpan resolveTimeout)
        {
            this.cacheTtl = cacheTtl;
            this.resolveTimeout = resolveTimeout;

            cache = new ConcurrentDictionary<string, (IPAddress[] addresses, DateTime validTo)>(StringComparer.OrdinalIgnoreCase);
        }

        public IPAddress[] Resolve(string hostname)
        {
            var currentTime = DateTime.UtcNow;

            if (cache.TryGetValue(hostname, out var cacheEntry))
            {
                if (cacheEntry.validTo < currentTime && isUpdatingNow.TrySetTrue())
                {
                    Task.Run(
                        async () =>
                        {
                            try
                            {
                                await ResolveAndUpdateCacheAsync(hostname, currentTime).ConfigureAwait(false);
                            }
                            finally
                            {
                                isUpdatingNow.TrySetFalse();
                            }
                        });
                }

                return cacheEntry.addresses;
            }

            var resolveTask = ResolveAndUpdateCacheAsync(hostname, currentTime);
            return resolveTask.Wait(resolveTimeout)
                ? resolveTask.GetAwaiter().GetResult()
                : EmptyAddresses;
        }

        private async Task<IPAddress[]> ResolveAndUpdateCacheAsync(string hostname, DateTime currentTime)
        {
            var addresses = await ResolveInternal(hostname).ConfigureAwait(false);
            cache[hostname] = (addresses, currentTime + cacheTtl);
            return addresses;
        }

        private static async Task<IPAddress[]> ResolveInternal(string hostname)
        {
            try
            {
                return await Dns.GetHostAddressesAsync(hostname).ConfigureAwait(false);
            }
            catch
            {
                return EmptyAddresses;
            }
        }
    }
}