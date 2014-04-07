using System;
using System.Collections;
using NHibernate;
using NHibernate.Util;
using System.Collections.Generic;
using uNhAddIns.SessionEasier;

namespace uNhAddIns.Test.Conversations
{
	public class SessionFactoryProviderStub : ISessionFactoryProvider
	{
		private readonly ISessionFactory factory;
		private readonly IEnumerable<ISessionFactory> esf;

		public SessionFactoryProviderStub(ISessionFactory factory)
		{
			this.factory = factory;
			esf = new SingletonEnumerable<ISessionFactory>(factory);
		}

		#region Implementation of IEnumerable

		public IEnumerator<ISessionFactory> GetEnumerator()
		{
			return esf.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion

		#region Implementation of IDisposable

		public void Dispose()
		{

		}

		public ISessionFactory GetFactory(string factoryId)
		{
			return factory;
		}

#pragma warning disable 67
		public event EventHandler<EventArgs> BeforeCloseSessionFactory;
#pragma warning restore 67

		#endregion

	}
}