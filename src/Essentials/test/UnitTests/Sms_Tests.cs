using System.Threading.Tasks;
using Xunit;

namespace Tests
{
	public class Sms_Tests
	{
		[Fact]
		public Task Sms_Fail_On_NetStandard() =>
			Assert.ThrowsAsync<NotImplementedInReferenceAssemblyException>(() => Sms.ComposeAsync());
	}
}
