using System;
using NHibernate;
using NHibernate.Engine;
using NHibernate.Event;

namespace uNhAddIns.SessionEasier
{
	public delegate ISession SessionCloseDelegate(ISession session);
	public delegate void SessionDisposeDelegate(ISession session);

	public static class Commons
	{
		public const string SessionFactoryProviderKey = "NHSession.SessionFactoryProvider";
		public const string SessionFactoryKey = "NHSession.Context.TransactedSessionContext";
		public const string NHibernateSessionKey    = "NHibernateSession";

		public static readonly Type[] SessionProxyInterfaces = new[]
		                                                       	{
		                                                       		typeof (ISessionProxy), typeof (ISession),
		                                                       		typeof (ISessionImplementor), typeof (IEventSource)
		                                                       	};
	}
}