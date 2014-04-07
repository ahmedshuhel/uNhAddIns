using System;
using NHibernate;
using System.Collections.Generic;

namespace uNhAddIns.SessionEasier
{
	public interface ISessionFactoryProvider : IEnumerable<ISessionFactory>, IDisposable
	{
		ISessionFactory GetFactory(string factoryId);
		event EventHandler<EventArgs> BeforeCloseSessionFactory;
	}
}