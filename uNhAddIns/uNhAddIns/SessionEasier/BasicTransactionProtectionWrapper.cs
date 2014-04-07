using System;
using System.Reflection;
using log4net;
using NHibernate;

namespace uNhAddIns.SessionEasier
{
	public class BasicTransactionProtectionWrapper
	{
		protected static readonly object InvokeImplementation = new object();
		protected static readonly ILog log = LogManager.GetLogger(typeof (BasicTransactionProtectionWrapper));

		protected readonly SessionCloseDelegate closeDelegate;
		protected readonly SessionDisposeDelegate disposeDelegate;
		protected readonly ISession realSession;

		public BasicTransactionProtectionWrapper(ISession realSession, SessionCloseDelegate closeDelegate)
		{
			if (realSession == null)
			{
				throw new ArgumentNullException("realSession");
			}
			this.realSession = realSession;
			this.closeDelegate = closeDelegate;
		}

		public BasicTransactionProtectionWrapper(ISession realSession, SessionCloseDelegate closeDelegate,
		                                         SessionDisposeDelegate disposeDelegate) : this(realSession, closeDelegate)
		{
			this.disposeDelegate = disposeDelegate;
		}

		public virtual object Invoke(MethodBase method, object[] args)
		{
			string methodName = method.Name;
			if ("get_InvocationHandler".Equals(methodName))
			{
				return this;
			}
			// If Close() is called, guarantee Unbind()
			if ("Close".Equals(methodName) || "Dispose".Equals(methodName))
			{
				if (closeDelegate != null)
				{
					closeDelegate(realSession);
				}
				if ("Dispose".Equals(methodName))
				{
					if (disposeDelegate != null)
					{
						disposeDelegate(realSession);
					}
				}
			}
			else if (IsPassThroughMethod(methodName))
			{
				// allow these to go through the the real session no matter what
			}
			else if (!realSession.IsOpen)
			{
				// essentially, if the real session is closed allow any
				// method call to pass through since the real session
				// will complain by throwing an appropriate exception;
			}
			else if (!realSession.Transaction.IsActive)
			{
				// limit the methods available if no transaction is active
				if (IsPassThroughMethodWithoutTransaction(methodName))
				{
					log.Debug("allowing method [" + methodName + "] in non-transacted context");
				}
				else if ("Reconnect".Equals(methodName) || "Disconnect".Equals(methodName))
				{
					// allow these (deprecated) methods to pass through
				}
				else
				{
					if(HandleMissingTransaction(methodName))
					{
						return InvokeImplementation;
					}
				}
			}
			log.Debug("allowing proxied method [" + methodName + "] to proceed to real session");
			return InvokeImplementation;
		}

		/// <summary>
		/// Method to handle the absence of transaction.
		/// </summary>
		/// <param name="methodName"></param>
		/// <returns>Return true if the interceptor must proceed with the transaction.</returns>
		protected virtual bool HandleMissingTransaction(string methodName)
		{
			throw new HibernateException(methodName + " is not valid without active transaction");
		}

		protected virtual bool IsPassThroughMethodWithoutTransaction(string methodName)
		{
			return "BeginTransaction".Equals(methodName) || "get_Transaction".Equals(methodName)
						 || "get_FlushMode".Equals(methodName) || "set_FlushMode".Equals(methodName) 
						 || "get_SessionFactory".Equals(methodName) || "get_IsConnected".Equals(methodName);
		}

		protected virtual bool IsPassThroughMethod(string methodName)
		{
			return "ToString".Equals(methodName) || "Equals".Equals(methodName) || "GetHashCode".Equals(methodName)
			       || "get_Statistics".Equals(methodName) || "get_IsOpen".Equals(methodName)
			       || "get_Timestamp".Equals(methodName);
		}
	}
}