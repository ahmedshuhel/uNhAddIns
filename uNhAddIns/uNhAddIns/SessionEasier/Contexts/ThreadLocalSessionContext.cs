using System;
using System.Collections.Generic;
using NHibernate;
using NHibernate.Engine;

namespace uNhAddIns.SessionEasier.Contexts
{
	public class ThreadLocalSessionContext: CurrentSessionContext
	{
		[ThreadStatic]
		protected static IDictionary<ISessionFactory, ISession> Context;

		public ThreadLocalSessionContext(ISessionFactoryImplementor factory) : base(factory) {}


		protected override IDictionary<ISessionFactory, ISession> GetContextDictionary()
		{
			return Context;
		}

		protected override void SetContextDictionary(IDictionary<ISessionFactory, ISession> value)
		{
			Context = value;
		}

	}
}