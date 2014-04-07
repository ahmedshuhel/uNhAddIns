using System;
using NHibernate;

namespace uNhAddIns.TestUtils.NhIntegration
{
	public static class Extensions
	{
		public static void EncloseInTransaction(this ISessionFactory sessionFactory, Action<ISession> work)
		{
			using (ISession s = sessionFactory.OpenSession())
			{
				using (ITransaction tx = s.BeginTransaction())
				{
					work(s);
					tx.Commit();
				}
			}
		}
	}
}