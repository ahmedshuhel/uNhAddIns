using NHibernate;

namespace uNhAddIns.SessionEasier
{
	public interface ISessionWrapper
	{
		ISession Wrap(ISession realSession, SessionCloseDelegate closeDelegate, SessionDisposeDelegate disposeDelegate);
		ISession WrapWithAutoTransaction(ISession realSession, SessionCloseDelegate closeDelegate, SessionDisposeDelegate disposeDelegate);
		bool IsWrapped(ISession session);
	}
}