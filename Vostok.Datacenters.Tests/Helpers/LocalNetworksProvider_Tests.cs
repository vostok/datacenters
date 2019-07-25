using System;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using FluentAssertions;
using NUnit.Framework;
using Vostok.Commons.Helpers.Network;
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

                var ipProperties = networkInterface.GetIPProperties();

                var gatewayAddresses = ipProperties.GatewayAddresses
                    .Select(gw => gw.Address)
                    .ToArray();

                foreach (var address in ipProperties.UnicastAddresses)
                {
                    Console.WriteLine(address.Address.AddressFamily);
                }

                var unicastAddresses = ipProperties.UnicastAddresses
                    .Where(uni => uni.Address.AddressFamily == AddressFamily.InterNetwork)
                    .ToArray();

                if (gatewayAddresses.Length == 0 || unicastAddresses.Length == 0)
                    continue;

                //newLocalNetworks.AddRange(unicastAddresses.Select(uni => new IPv4Network(uni.Address, (byte)uni.PrefixLength)));
            }

            LocalNetworksProvider.Get().Should().NotBeEmpty();
        }
    }
}