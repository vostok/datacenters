using System.Collections.Generic;
using System.Net;
using JetBrains.Annotations;

namespace Vostok.Datacenters
{
    /// <summary>
    /// <see cref="IDatacenters"/> is a helper that assists in locating machines in datacenters.
    /// </summary>
    [PublicAPI]
    public interface IDatacenters
    {
        /// <summary>
        /// <para>Returns the name of the datacenter that contains the local machine running this code.</para>
        /// <para>May return <c>null</c> if its datacenter is not known.</para>
        /// </summary>
        [CanBeNull]
        string GetLocalDatacenter();

        /// <summary>
        /// <para>Returns the name of the datacenter that contains the machine with given <paramref name="address"></paramref>.</para>
        /// <para>May return <c>null</c> if no such datacenter is known.</para>
        /// </summary>
        [CanBeNull]
        string GetDatacenter([NotNull] IPAddress address);

        /// <summary>
        /// <para>Returns the name of the datacenter that contains the machine with given <paramref name="hostname"></paramref>.</para>
        /// <para>May return <c>null</c> if no such datacenter is known.</para>
        /// </summary>
        [CanBeNull]
        string GetDatacenter([NotNull] string hostname);

        /// <summary>
        /// <para>Returns the name of the datacenter that contains the machine with given <paramref name="hostname"></paramref>.</para>
        /// <para>May return <c>null</c> if no such datacenter is known or the initial DNS resolution has not happened yet.</para>
        /// </summary>
        [CanBeNull]
        string GetDatacenterWeak([NotNull] string hostname);

        /// <summary>
        /// Returns a list of currently active datacenters.
        /// </summary>
        [NotNull]
        IReadOnlyCollection<string> GetActiveDatacenters();
    }
}