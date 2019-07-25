using FluentAssertions;
using NUnit.Framework;
using Vostok.Datacenters.Helpers;

namespace Vostok.Datacenters.Tests.Helpers
{
    [TestFixture]
    internal class LocalNetworksProvider_Tests
    {
        [Test]
        public void Should_locate_something()
        {
            LocalNetworksProvider.Get().Should().NotBeEmpty();
        }
    }
}