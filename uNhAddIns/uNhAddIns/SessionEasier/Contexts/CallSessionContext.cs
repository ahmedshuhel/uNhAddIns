using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using NHibernate;
using NHibernate.Engine;

namespace uNhAddIns.SessionEasier.Contexts
{
	[Serializable]
	public class CallSessionContext : CurrentSessionContext
	{
		public CallSessionContext(ISessionFactoryImplementor factory) : base(factory) {}

		#region Overrides of AbstractCurrentSessionContext

		protected override IDictionary<ISessionFactory, ISession> GetContextDictionary()
		{
			return CallContext.GetData(Commons.SessionFactoryKey) as IDictionary<ISessionFactory, ISession>;
		}

		protected override void SetContextDictionary(IDictionary<ISessionFactory, ISession> value)
		{
			CallContext.LogicalSetData(Commons.SessionFactoryKey, value);
		}

		#endregion
	}
}