using System;
using System.Collections.Generic;

namespace uNhAddIns.TestUtils.Logging
{
	public interface ILogSpy : IDisposable
	{
		string GetWholeLog();
		IEnumerable<string> Messages();
	}
}