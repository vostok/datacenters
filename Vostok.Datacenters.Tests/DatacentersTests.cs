using System.Collections.Generic;
using System.Linq;
using System.Net;
using FluentAssertions;
using NUnit.Framework;
using Vostok.Commons.Environment;
using Vostok.Datacenters.Helpers;

namespace Vostok.Datacenters.Tests
{
    [TestFixture]
    internal class DatacentersTests
    {
        private HashSet<string> activeDatacenters;
        private Dictionary<IPAddress, string> datacentersMapping;
        private IDatacenters datacenters;

        [SetUp]
        public void SetUp()
        {
            activeDatacenters = new HashSet<string> {"dc1"};

            datacentersMapping = new Dictionary<IPAddress, string>
            {
                [IPAddress.Parse("10.1.1.1")] = "dc1",
                [IPAddress.Parse("20.1.1.1")] = "dc2"
            };

            datacenters = new Datacenters(
                new DatacentersSettings(
                    ip => datacentersMapping.ContainsKey(ip) ? datacentersMapping[ip] : null,
                    () => activeDatacenters));
        }

        [Test]
        [Explicit("Not works on appveyor.")]
        public void GetLocalDatacenter_should_works_correctly()
        {
            var ips = LocalNetworksProvider.Get();
            ips.Should().NotBeEmpty();

            datacentersMapping[ips.Last()] = "my";

            datacenters.GetLocalDatacenter().Should().Be("my");
        }

        [Test]
        [Explicit("Not works on appveyor.")]
        public void GetLocalDatacenter_should_works_correctly_with_hostname_overwriting()
        {
            var hostName = EnvironmentInfo.Host;

            var ips = LocalNetworksProvider.Get();
            ips.Should().NotBeEmpty();

            datacentersMapping[ips.Last()] = "my";

            datacenters = new Datacenters(
                new DatacentersSettings(
                    ip => datacentersMapping.ContainsKey(ip) ? datacentersMapping[ip] : null,
                    () => activeDatacenters)
                {
                    LocalHostnameOverwriting = hostName
                });

            datacenters.GetLocalDatacenter().Should().Be("my");
        }

        [Test]
        public void GetLocalDatacenter_should_works_correctly_with_overwriting()
        {
            datacenters = new Datacenters(
                new DatacentersSettings(
                    ip => null,
                    () => activeDatacenters)
                {
                    LocalDatacenterOverwriting = "my"
                });

            datacenters.GetLocalDatacenter().Should().Be("my");
        }

        [Test]
        public void GetDatacenter_by_ip_should_works_correctly()
        {
            datacenters.GetDatacenter(IPAddress.Parse("10.1.1.1")).Should().Be("dc1");
            datacenters.GetDatacenter(IPAddress.Parse("20.1.1.1")).Should().Be("dc2");
        }

        [Test]
        public void GetDatacenter_by_hostname_should_works_correctly()
        {
            var hostName = Dns.GetHostName();
            var ips = Dns.GetHostAddresses(hostName);

            ips.Should().NotBeEmpty();

            datacentersMapping[ips.Last()] = "my";

            datacenters.GetDatacenter(hostName).Should().Be("my");
        }

        [Test]
        public void GetActiveDatacenters_should_works_correctly()
        {
            datacenters.GetActiveDatacenters().Should().BeEquivalentTo("dc1");
            activeDatacenters.Add("dc2");
            datacenters.GetActiveDatacenters().Should().BeEquivalentTo("dc1", "dc2");
        }
    }
}