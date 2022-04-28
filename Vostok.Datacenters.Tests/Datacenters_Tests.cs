using System;
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
    internal class Datacenters_Tests
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
        public void GetLocalDatacenter_should_work_correctly()
        {
            var ips = LocalNetworksProvider.Get();
            ips.Should().NotBeEmpty();

            datacentersMapping[ips.Last()] = "my";

            datacenters.GetLocalDatacenter().Should().Be("my");
        }

        [Test]
        public void GetLocalDatacenter_should_work_correctly_with_hostname_overwriting()
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
                    LocalHostname = hostName
                });

            datacenters.GetLocalDatacenter().Should().Be("my");
        }

        [Test]
        public void GetLocalDatacenter_should_work_correctly_with_overwriting()
        {
            datacenters = new Datacenters(
                new DatacentersSettings(
                    ip => null,
                    () => activeDatacenters)
                {
                    LocalDatacenter = "dc_from_settings"
                });

            datacenters.GetLocalDatacenter().Should().Be("dc_from_settings");
        }

        [Test]
        public void GetLocalDatacenter_should_work_correctly_with_overwriting_using_env_variable()
        {
            Environment.SetEnvironmentVariable("VOSTOK_LOCAL_DATACENTER", "dc_from_env");

            datacenters = new Datacenters(
                new DatacentersSettings(
                    ip => null,
                    () => activeDatacenters));

            datacenters.GetLocalDatacenter().Should().Be("dc_from_env");

            Environment.SetEnvironmentVariable("VOSTOK_LOCAL_DATACENTER", null);
        }

        [Test]
        public void GetDatacenter_by_ip_should_work_correctly()
        {
            datacenters.GetDatacenter(IPAddress.Parse("10.1.1.1")).Should().Be("dc1");
            datacenters.GetDatacenter(IPAddress.Parse("20.1.1.1")).Should().Be("dc2");
        }
        
        [Test]
        public void GetDatacenter_by_hostname_ip_should_work_correctly()
        {
            datacenters.GetDatacenter("10.1.1.1").Should().Be("dc1");
            datacenters.GetDatacenter("20.1.1.1").Should().Be("dc2");
        }

        [Test]
        public void GetDatacenter_by_hostname_should_work_correctly()
        {
            var hostName = Dns.GetHostName();
            var ips = Dns.GetHostAddresses(hostName);

            ips.Should().NotBeEmpty();

            datacentersMapping[ips.Last()] = "my";

            datacenters.GetDatacenter(hostName).Should().Be("my");
        }

        [Test]
        public void GetActiveDatacenters_should_work_correctly()
        {
            datacenters.GetActiveDatacenters().Should().BeEquivalentTo("dc1");
            activeDatacenters.Add("dc2");
            datacenters.GetActiveDatacenters().Should().BeEquivalentTo("dc1", "dc2");
        }
    }
}