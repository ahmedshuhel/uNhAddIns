using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.Practices.ServiceLocation;
using NHibernate;
using NHibernate.Linq;

namespace uNhAddIns.Adapters.CommonTests.Integration
{
	public class DaoFactory : IDaoFactory
	{
		private readonly IServiceLocator sc;

		public DaoFactory(IServiceLocator serviceLocator)
		{
			sc = serviceLocator;
		}

		public TDao GetDao<TDao>()
		{
			return sc.GetInstance<TDao>();
		}
	}

	public class SillyDao : ISillyDao
	{
		private readonly ISessionFactory factory;

		public SillyDao(ISessionFactory factory)
		{
			this.factory = factory;
		}

        /// <summary>
        /// Required for testing - don't do this in production code
        /// </summary>
	    public ISessionFactory Factory
	    {
	        get { return factory; }
	    }

	    public Silly Get(Guid id)
		{
			return factory.GetCurrentSession().Get<Silly>(id);
		}

		public IList<Silly> GetAll()
		{
			return factory.GetCurrentSession().CreateQuery("from Silly").List<Silly>();
		}

		public IQueryable<Silly> Retrieve(Expression<Func<Silly, bool>> predicate)
		{
			return factory.GetCurrentSession().Query<Silly>();
		}

		public Silly MakePersistent(Silly entity)
		{
			factory.GetCurrentSession().SaveOrUpdate(entity);
			return entity;
		}

		public void MakeTransient(Silly entity)
		{
			factory.GetCurrentSession().Delete(entity);
		}


	}

	[PersistenceConversational]
	public class SillyCrudModel : ISillyCrudModel
	{
		private readonly IDaoFactory factory;

		public SillyCrudModel(IDaoFactory factory)
		{
			if (factory == null)
			{
				throw new ArgumentNullException("factory");
			}
			this.factory = factory;
		}

		protected ISillyDao EntityDao
		{
			get { return factory.GetDao<ISillyDao>(); }
		}

		#region Implementation of ISillyCrudModel
		[PersistenceConversation]
		public virtual IList<Silly> GetEntirelyList()
		{
			return EntityDao.GetAll();
		}

		[PersistenceConversation]
		public virtual Silly GetIfAvailable(Guid id)
		{
			return EntityDao.Get(id);
		}

		[PersistenceConversation]
		public virtual Silly Save(Silly entity)
		{
			return EntityDao.MakePersistent(entity);
		}

		[PersistenceConversation]
		public virtual void Delete(Silly entity)
		{
			EntityDao.MakeTransient(entity);
		}

		[PersistenceConversation(ConversationEndMode = EndMode.CommitAndContinue)]
		public virtual void ImmediateDelete(Silly entity)
		{
			EntityDao.MakeTransient(entity);
		}

		[PersistenceConversation(ConversationEndMode = EndMode.End)]
		public virtual void AcceptAll()
		{
			// method for use-case End
		}

		[PersistenceConversation(ConversationEndMode = EndMode.Abort)]
		public virtual void Abort()
		{
			// method for use-case Abort
		}

		#endregion
	}

	[PersistenceConversational(AllowOutsidePersistentCall = true)]
	public class SillyReportModel : ISillyReportModel
	{
		private readonly IDaoFactory factory;

		public SillyReportModel(IDaoFactory factory)
		{
			if (factory == null)
			{
				throw new ArgumentNullException("factory");
			}
			this.factory = factory;
		}

		protected ISillyDao EntityDao
		{
			get { return factory.GetDao<ISillyDao>(); }
		}

		public IQueryable<Silly> GetSillies()
		{
			return EntityDao.Retrieve(s => true);
		}

		[PersistenceConversation(ConversationEndMode = EndMode.End)]
		public void End()
		{
		}

	}
}