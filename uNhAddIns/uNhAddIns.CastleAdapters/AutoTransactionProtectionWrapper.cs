using NHibernate;
using uNhAddIns.SessionEasier;

namespace uNhAddIns.CastleAdapters
{
	public class AutoTransactionProtectionWrapper : TransactionProtectionWrapper
	{
		private ITransaction autoTransaction;

		public AutoTransactionProtectionWrapper(ISession realSession, SessionCloseDelegate closeDelegate) : base(realSession, closeDelegate)
		{}

		public AutoTransactionProtectionWrapper(ISession realSession, SessionCloseDelegate closeDelegate, SessionDisposeDelegate disposeDelegate) : base(realSession, closeDelegate, disposeDelegate)
		{}

		public override void Intercept(Castle.DynamicProxy.IInvocation invocation)
		{
			base.Intercept(invocation);

			if (autoTransaction == null) return;
			autoTransaction.Commit();
			autoTransaction.Dispose();
			autoTransaction = null;
		}

		protected override bool HandleMissingTransaction(string methodName)
		{
			autoTransaction = realSession.BeginTransaction();
			return true;
		}
	}
}