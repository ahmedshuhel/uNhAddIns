using System;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using NUnit.Framework;
using uNhAddIns.Extensions;

namespace uNhAddIns.Test.Extensions
{
	[TestFixture]
	public class EnumerablesFixture
	{
		[Test]
		public void ConcatWithSeparator()
		{
			(new[] {"1", "2"}).ConcatWithSeparator(';').Should().Be.EqualTo("1;2");
			(new[] {"1", "2", "3"}).ConcatWithSeparator(';').Should().Be.EqualTo("1;2;3");
			(new[] { "1" }).ConcatWithSeparator(';').Should().Be.EqualTo("1");
			(new[] { "" }).ConcatWithSeparator(';').Should().Be.EqualTo("");
		}

		[Test]
		public void ConcatWithSeparatorKeyValuePair()
		{
			(new[] { GetPair("es", "Hola"), GetPair("it", "Salve") }).ToString('~')
				.Should().Be.EqualTo("~es~~Hola~~it~~Salve~");

			(new[] { GetPair("es", "Hola"), GetPair("it", "Salve"), GetPair("en", "Hello") }).ToString('~')
				.Should().Be.EqualTo("~es~~Hola~~it~~Salve~~en~~Hello~");

			(new[] { GetPair("es", "Hola") }).ToString('#')
				.Should().Be.EqualTo("#es##Hola#");

			(new KeyValuePair<CultureInfo, string>[0]).ToString('~')
				.Should().Be.EqualTo("");
		}

		private KeyValuePair<CultureInfo,string> GetPair(string name, string descr)
		{
			return new KeyValuePair<CultureInfo, string>(new CultureInfo(name),descr);
		}

		[Test]
		public void ToPairs()
		{
			Assert.Throws<ArgumentException>(() => (new[] {"a", "aa", "b"}).ToPairs().ToList())
				.Message.Should().Contain("The number of elements should be pair");

			var l = (new[] {"a", "aa", "b", "bb"}).ToPairs().ToArray();
			l[0].Key.Should().Be.EqualTo("a");
			l[0].Value.Should().Be.EqualTo("aa");

			l[1].Key.Should().Be.EqualTo("b");
			l[1].Value.Should().Be.EqualTo("bb");
		}
	}
}