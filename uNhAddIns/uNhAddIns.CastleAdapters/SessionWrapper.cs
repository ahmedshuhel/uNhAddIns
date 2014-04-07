using Castle.DynamicProxy;
using NHibernate;
using uNhAddIns.SessionEasier;

namespace uNhAddIns.CastleAdapters
{
	public class SessionWrapper : ISessionWrapper
	{
		private readonly ProxyGenerator _proxyGenerator = new ProxyGenerator();

		public ISession Wrap(ISession realSession, SessionCloseDelegate closeDelegate, SessionDisposeDelegate disposeDelegate)
		{
			if (IsWrapped(realSession))
			{
				return realSession;
			}

			var wrapper = new TransactionProtectionWrapper(realSession, closeDelegate, disposeDelegate);

			return GenerateProxy(realSession, wrapper);
		}

		public ISession WrapWithAutoTransaction(ISession realSession, SessionCloseDelegate closeDelegate,
		                                        SessionDisposeDelegate disposeDelegate)
		{
			if (IsWrapped(realSession))
			{
				return realSession;
			}

			var wrapper = new AutoTransactionProtectionWrapper(realSession, closeDelegate, disposeDelegate);

			return GenerateProxy(realSession, wrapper);
		}

		public bool IsWrapped(ISession session)
		{
			if (session == null)
			{
				return false;
			}
			var sessionProxy = session as ISessionProxy;
			// try to make sure we don't wrap and already wrapped session
			return sessionProxy != null && sessionProxy.InvocationHandler != null
			       && sessionProxy.InvocationHandler is TransactionProtectionWrapper;
		}

		private ISession GenerateProxy(ISession realSession, TransactionProtectionWrapper wrapper)
		{
			var wrapped = (ISession) _proxyGenerator.CreateInterfaceProxyWithTarget(typeof (ISession),
			                                                                       Commons.SessionProxyInterfaces,
			                                                                       realSession,
			                                                                       wrapper);
			return wrapped;
		}

	}
}