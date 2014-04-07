using NUnit.Framework;
using uNhAddIns.Extensions;

namespace uNhAddIns.Test.Extensions
{
	[TestFixture]
	public class StringFixture
	{
		[Test]
		public void ShouldSplitValues()
		{
			"~es~~Hola~~it~~Salve~".SplitByEncloser('~')
				.Should().Have.SameSequenceAs(new[] {"es","Hola","it","Salve"});

			"~es~~Hola~~it~~Salve~~en~~Hello~".SplitByEncloser('~')
				.Should().Have.SameSequenceAs(new[] {"es", "Hola", "it", "Salve","en", "Hello"});

			"#es##Hola#".SplitByEncloser('#')
				.Should().Have.SameSequenceAs(new[] {"es", "Hola"});

			"".SplitByEncloser('#').Should().Be.Empty();
		}
	}
}