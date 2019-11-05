using System;
using JetBrains.Annotations;

namespace Vostok.Datacenters
{
    /// <summary>
    /// <para><see cref="DatacentersProvider"/> is a static shared configuration point that allows to decouple configuration of datacenters in libraries from calling code.</para>
    /// <para>It is intended to be used primarily by library developers who must not force their users to explicitly provide <see cref="IDatacenters"/> instances.</para>
    /// <para>It is expected to be configured by a hosting system or just directly in the application entry point.</para>
    /// </summary>
    [PublicAPI]
    public static class DatacentersProvider
    {
        private static readonly IDatacenters DefaultInstance = new EmptyDatacenters();

        private static volatile IDatacenters instance;

        /// <summary>
        /// Returns <c>true</c> if a global <see cref="IDatacenters"/> instance has already been configured with <see cref="Configure"/> method. Returns <c>false</c> otherwise.
        /// </summary>
        public static bool IsConfigured => instance != null;

        /// <summary>
        /// <para>Returns the global default instance of <see cref="IDatacenters"/> if it's been configured.</para>
        /// <para>If nothing has been configured yet, falls back to an instance of <see cref="EmptyDatacenters"/>.</para>
        /// </summary>
        [NotNull]
        public static IDatacenters Get() => instance ?? DefaultInstance;

        /// <summary>
        /// <para>Configures the global default <see cref="IDatacenters"/> with given instance, which will be returned by all subsequent <see cref="Get"/> calls.</para>
        /// <para>By default, this method fails when trying to overwrite a previously configured instance. This behaviour can be changed with <paramref name="canOverwrite"/> parameter.</para>
        /// </summary>
        /// <exception cref="ArgumentNullException">Provided instance was <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Attempted to overwrite previously configured instance.</exception>
        public static void Configure([NotNull] IDatacenters datacenters, bool canOverwrite = false)
        {
            if (!canOverwrite && instance != null)
                throw new InvalidOperationException($"Can't overwrite existing configured implementation of type '{instance.GetType().Name}'.");

            instance = datacenters ?? throw new ArgumentNullException(nameof(datacenters));
        }
    }
}