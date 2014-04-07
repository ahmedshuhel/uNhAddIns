using System;
using NHibernate.Util;
using uNhAddIns.SessionEasier;

namespace uNhAddIns.Test
{
	public static class TestHelpers
	{
		public static ISessionWrapper GetSessionWrapper()
		{
			return (ISessionWrapper)Activator.CreateInstance(ReflectHelper.ClassForName(GetSessionWrapperQualifiedName()));
		}

		private static string GetSessionWrapperQualifiedName()
		{
            return "uNhAddIns.CastleAdapters.SessionWrapper, uNhAddIns.CastleAdapters";
		}
	}
}