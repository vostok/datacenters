using System.Collections.Generic;
using System.Net;
using JetBrains.Annotations;

namespace Vostok.Datacenters
{
    /// <summary>
    /// An utility class containing helper methods for working with datacenters.
    /// </summary>
    [PublicAPI]
    public interface IDatacenters
    {
        /// <summary>
        /// Returns name of local datacenter.
        /// </summary>
        [CanBeNull]
        string GetLocalDatacenter();

        /// <summary>
        /// Returns name of datacenter which contains provided <see cref="IPAddress"/>.
        /// </summary>
        [CanBeNull]
        string GetDatacenter(IPAddress address);

        /// <summary>
        /// Returns name of datacenter which contains provided hostname.
        /// </summary>
        [CanBeNull]
        string GetDatacenter(string hostname);

        /// <summary>
        /// Returns list of active datacenters.
        /// </summary>
        [NotNull]
        IReadOnlyCollection<string> GetActiveDatacenters();
    }
}