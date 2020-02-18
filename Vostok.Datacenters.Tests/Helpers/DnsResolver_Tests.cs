using System.Net;
using FluentAssertions;
using NUnit.Framework;
using Vostok.Commons.Time;
using Vostok.Datacenters.Helpers;

namespace Vostok.Datacenters.Tests.Helpers
{
    [TestFixture]
    internal class DnsResolver_Tests
    {
        [Test]
        public void Should_resolve_something()
        {
            var host = Dns.GetHostName();
            var resolver = new DnsResolver(1.Minutes(), 1.Seconds());
            resolver.Resolve(host, true).Should().NotBeEmpty();
        }
    }
}