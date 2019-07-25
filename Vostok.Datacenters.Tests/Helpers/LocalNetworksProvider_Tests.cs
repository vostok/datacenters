using System;
using System.Net.NetworkInformation;
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
            var x = NetworkInterface.GetAllNetworkInterfaces();
            foreach (var networkInterface in x)
            {
                Console.WriteLine($"INTERFACE {networkInterface.Id} {networkInterface.OperationalStatus} {networkInterface.NetworkInterfaceType}");
            }

            LocalNetworksProvider.Get().Should().NotBeEmpty();
        }
    }
}