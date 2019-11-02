using System.Linq;
using JetBrains.Annotations;

namespace Vostok.Datacenters
{
    [PublicAPI]
    public static class IDatacentersExtensions
    {
        /// <summary>
        /// <para>Returns <c>true</c> if current local datacenter is among ones listed in result of <see cref="IDatacenters.GetActiveDatacenters"/>.</para>
        /// <para>Also returns <c>true</c> if local datacenter cannot be determined.</para>
        /// </summary>
        public static bool LocalDatacenterIsActive([NotNull] this IDatacenters datacenters)
        {
            var localDatacenter = datacenters.GetLocalDatacenter();

            return localDatacenter == null || datacenters.GetActiveDatacenters().Contains(localDatacenter);
        }
    }
}