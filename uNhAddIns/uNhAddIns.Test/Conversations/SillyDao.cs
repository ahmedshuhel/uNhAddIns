using System.Collections.Generic;
using NHibernate;

namespace uNhAddIns.Test.Conversations
{
	public class SillyDao
	{
		private readonly ISessionFactory factory;

		public SillyDao(ISessionFactory factory)
		{
			this.factory = factory;
		}

		public Silly3 Get(int id)
		{
			return factory.GetCurrentSession().Get<Silly3>(id);
		}

		public IList<Silly3> GetAll()
		{
			return factory.GetCurrentSession().CreateQuery("from Silly3").List<Silly3>();
		}

		public Silly3 MakePersistent(Silly3 entity)
		{
			factory.GetCurrentSession().SaveOrUpdate(entity);
			return entity;
		}

		public void MakeTransient(Silly3 entity)
		{
			factory.GetCurrentSession().Delete(entity);
		}
	}
}