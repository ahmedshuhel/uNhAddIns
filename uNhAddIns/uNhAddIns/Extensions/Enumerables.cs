using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace uNhAddIns.Extensions
{
	public static class Enumerables
	{
		public static string ConcatWithSeparator(this IEnumerable<string> source, char separator)
		{
			var result = new StringBuilder(128);
			bool separatorFirst = false;
			foreach (var part in source)
			{
				if (separatorFirst)
				{
					result.Append(separator).Append(part);
				}
				else
				{
					result.Append(part);
					separatorFirst = true;
				}
			}
			return result.ToString();
		}

		public static string ToString(this IEnumerable<KeyValuePair<CultureInfo, string>> source, char keyValueEncloser)
		{
			var result = new StringBuilder(1024);
			foreach (var part in source)
			{
				result
					.Append(keyValueEncloser).Append(part.Key).Append(keyValueEncloser)
					.Append(keyValueEncloser).Append(part.Value).Append(keyValueEncloser);
			}

			return result.ToString();
		}

		public static IEnumerable<KeyValuePair<T, T>> ToPairs<T>(this IEnumerable<T> source)
		{
			var list = new List<T>(source);
			if (list.Count % 2 != 0)
			{
				throw new ArgumentException(string.Format("The number of elements should be pair; found={0}", list.Count), "source");
			}
			var enumerator = source.GetEnumerator();
			while (enumerator.MoveNext())
			{
				var key = enumerator.Current;
				enumerator.MoveNext();
				var value = enumerator.Current;
				yield return new KeyValuePair<T, T>(key, value);
			}
		}
	}
}