using System;
using System.Reflection;
using Castle.DynamicProxy;
using NHibernate;
using NHibernate.Util;
using uNhAddIns.SessionEasier;
using IInterceptor = Castle.DynamicProxy.IInterceptor;

namespace uNhAddIns.CastleAdapters
{
	[Serializable]
	public class TransactionProtectionWrapper : BasicTransactionProtectionWrapper, IInterceptor
	{
		public TransactionProtectionWrapper(ISession realSession, SessionCloseDelegate closeDelegate)
			: base(realSession, closeDelegate)
		{
		}

		public TransactionProtectionWrapper(ISession realSession, SessionCloseDelegate closeDelegate,
		                                    SessionDisposeDelegate disposeDelegate)
			: base(realSession, closeDelegate, disposeDelegate)
		{
		}

		#region IInterceptor Members

		public virtual void Intercept(IInvocation invocation)
		{
			invocation.ReturnValue = null;
			try
			{
				object returnValue = base.Invoke(invocation.Method, invocation.Arguments);

				// Avoid invoking the actual implementation
				if (returnValue != InvokeImplementation)
				{
					invocation.ReturnValue = returnValue;
				}
				else
				{
					invocation.Proceed();
				}
			}
			catch (TargetInvocationException ex)
			{
				throw ReflectHelper.UnwrapTargetInvocationException(ex);
			}
		}

		#endregion
	}
}