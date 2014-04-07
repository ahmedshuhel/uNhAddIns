using System;
using NHibernate;

namespace uNhAddIns.SessionEasier
{
	/// <summary>
	/// A fake session wrapper
	/// </summary>
	/// <remarks>
	/// It can be used only in session-per-request or in others simples 
	/// session management pattern where you don't want a transaction-protection.
	/// <b>Not allowed in Coversation-per-BusinessTransaction.</b>
	/// </remarks>
	public class FakeSessionWrapper : ISessionWrapper
	{
		#region Implementation of ISessionWrapper

		public ISession Wrap(ISession realSession, SessionCloseDelegate closeDelegate, SessionDisposeDelegate disposeDelegate)
		{
			return realSession;
		}

		public ISession WrapWithAutoTransaction(ISession realSession, SessionCloseDelegate closeDelegate, SessionDisposeDelegate disposeDelegate)
		{
			return realSession;
		}

		public bool IsWrapped(ISession session)
		{
			return session != null;
		}

		#endregion
	}
}