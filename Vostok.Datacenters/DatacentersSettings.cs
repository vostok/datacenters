using System;
using System.Collections.Generic;
using System.Net;
using JetBrains.Annotations;
using Vostok.Commons.Time;

namespace Vostok.Datacenters
{
    /// <summary>
    /// Represents <see cref="Datacenters"/> settings.
    /// </summary>
    [PublicAPI]
    public class DatacentersSettings
    {
        /// <summary>
        /// Creates a new instance of <see cref="DatacentersSettings"/>.
        /// <param name="datacenterMapping">Mapping from <see cref="IPAddress"/> to the name of datacenter.</param>
        /// <param name="activeDatacentersProvider">A delegate that returns set of active datacenters.</param>
        /// </summary>
        public DatacentersSettings(
            [NotNull] Func<IPAddress, string> datacenterMapping,
            [NotNull] Func<IReadOnlyCollection<string>> activeDatacentersProvider)
        {
            DatacenterMapping = datacenterMapping ?? throw new ArgumentNullException(nameof(datacenterMapping));
            ActiveDatacentersProvider = activeDatacentersProvider ?? throw new ArgumentNullException(nameof(activeDatacentersProvider));
        }

        [NotNull]
        public Func<IPAddress, string> DatacenterMapping { get; }

        [NotNull]
        public Func<IReadOnlyCollection<string>> ActiveDatacentersProvider { get; }

        public TimeSpan DnsCacheTtl { get; set; } = 30.Minutes();

        public TimeSpan DnsResolveTimeout { get; set; } = 1.Seconds();

        /// <summary>
        /// Local datacenter can be specified here, or using <c>VOSTOK_LOCAL_DATACENTER</c> environment variable.
        /// </summary>
        [CanBeNull]
        public string LocalDatacenter { get; set; }

        /// <summary>
        /// Local hostname can be specified here, or using <c>VOSTOK_LOCAL_HOSTNAME</c> or <c>VOSTOK_LOCAL_FQDN</c> environment variable.
        /// </summary>
        [CanBeNull]
        public string LocalHostname { get; set; }
    }
}