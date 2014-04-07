using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace uNhAddIns.Extensions
{
	public static class String
	{
		public static IEnumerable<string> SplitByEncloser(this string value, char valueEncloser)
		{
			var rx = new Regex(string.Format(@"{0}[^{0}\r\n]*{0}", valueEncloser));
			foreach (Match match in rx.Matches(value))
			{
				yield return match.Value.Trim(valueEncloser);
			}
		}
	}
}