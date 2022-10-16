using System.Net;
using Xunit;

namespace Server.Tests
{
	public class IPAddressTests
	{
		[Theory]
		[InlineData("192.168.100.254/16", "192.168.100.1", true)]
		[InlineData("192.168.100.254/24", "192.168.50.1", false)]
		[InlineData("192.168.100.1/32", "192.168.50.1", false)]
		[InlineData("192.168.50.1/32", "192.168.50.1", true)]
		[InlineData("192.168.50.4/30", "192.168.50.7", true)]
		[InlineData("192.168.50.4/30", "192.168.50.9", false)]
		public void TestIPMatchCIDR(string cidr, string addr, bool shouldMatch)
		{
			var cidrParsed = Utility.TryParseCIDR(cidr, out var cidrAddress, out var cidrLength);
			Assert.True(cidrParsed);

			var address = IPAddress.Parse(addr);
			Assert.Equal(shouldMatch, Utility.IPMatchCIDR(cidr, address));
			Assert.Equal(shouldMatch, Utility.IPMatchCIDR(cidrAddress, address, cidrLength));
		}
	}
}
