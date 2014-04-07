using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using log4net;
using Microsoft.Practices.ServiceLocation;
using uNhAddIns.Adapters.CommonTests.Integration;
using uNhAddIns.SessionEasier.Conversations;

namespace uNhAddIns.Adapters.CommonTests.ConversationManagement
{
	public class ConversationFactoryStub : IConversationFactory
	{
		private readonly Func<string, IConversation> creator;

		public ConversationFactoryStub(Func<string, IConversation> creator)
		{
			this.creator = creator;
		}

		#region Implementation of IConversationFactory

		public IConversation CreateConversation()
		{
			throw new NotImplementedException();
		}

		public IConversation CreateConversation(string conversationId)
		{
			return creator(conversationId);
		}

		#endregion
	}

	public class ConversationsContainerAccessorStub : IConversationsContainerAccessor
	{
		private readonly IServiceLocator serviceLocator;

		public ConversationsContainerAccessorStub(IServiceLocator serviceLocator)
		{
			this.serviceLocator = serviceLocator;
		}

		#region Implementation of IConversationsContainerAccessor

		public IConversationContainer Container
		{
			get { return serviceLocator.GetInstance<IConversationContainer>(); }
		}

		#endregion
	}

	public class DaoFactoryStub : IDaoFactory
	{
		private readonly IServiceLocator serviceLocator;

		public DaoFactoryStub(IServiceLocator serviceLocator)
		{
			this.serviceLocator = serviceLocator;
		}

		#region Implementation of IDaoFactory

		public TDao GetDao<TDao>()
		{
			return serviceLocator.GetInstance<TDao>();
		}

		#endregion
	}

	public class SillyDaoStub : ISillyDao
	{
		#region Implementation of ISillyDao

		public Silly Get(Guid id)
		{
            return new Silly(id);
		}

		public IList<Silly> GetAll()
		{
			return new List<Silly>(new[] {new Silly(Guid.NewGuid())});
		}

		public IQueryable<Silly> Retrieve(Expression<Func<Silly, bool>> predicate)
		{
			return GetAll().Where(predicate.Compile()).AsQueryable();
		}

		public Silly MakePersistent(Silly entity)
		{
			return entity;
		}

		public void MakeTransient(Silly entity) {}

		#endregion
	}

	public class ExceptionOnFlushConversationStub : AbstractConversation
	{
		public ExceptionOnFlushConversationStub(string id) : base(id) {}

		#region Overrides of AbstractConversation

		protected override void Dispose(bool disposing) {}

		protected override void DoStart() {}

		protected override void DoFlushAndPause()
		{
			throw new NotImplementedException();
		}

		protected override void DoPause() {}

		protected override void DoResume() {}

		protected override void DoEnd()
		{
			throw new NotImplementedException();
		}

		protected override void DoAbort() {}

		#endregion
	}

	public class NoOpConversationStub : AbstractConversation
	{
		public NoOpConversationStub(string id) : base(id) {}

		#region Overrides of AbstractConversation

		protected override void Dispose(bool disposing) {}

		protected override void DoStart() {}

		protected override void DoFlushAndPause() {}

		protected override void DoPause() {}

		protected override void DoResume() {}

		protected override void DoEnd() {}

		protected override void DoAbort() {}

		#endregion
	}

	public class ThreadLocalConversationContainerStub : ThreadLocalConversationContainer
	{
		// The fixture is executed in one trhead so we need something to clean the cotainer
		// at the end of each test.
		public void Reset()
		{
			if(store != null) store.Clear();
			currentId = null;
		}

		public int BindedConversationCount
		{
			get { return store.Count; }
		}
	}

	[PersistenceConversational(ConversationCreationInterceptor = typeof (ConversationCreationInterceptor))]
	public class InheritedSillyCrudModelWithConcreteConversationCreationInterceptor : SillyCrudModel
	{
		public InheritedSillyCrudModelWithConcreteConversationCreationInterceptor(IDaoFactory factory) : base(factory) {}
	}

	public class ConversationCreationInterceptor : IMyServiceConversationCreationInterceptor
	{
		public const string StartingMessage = "Starting";
		public const string StartedMessage = "Started";

		public ILog Log
		{
			get { return LogManager.GetLogger(typeof (ConversationCreationInterceptor)); }
		}

		#region Implementation of IConversationCreationInterceptor

		public void Configure(IConversation conversation)
		{
			conversation.Starting += ((x, y) => Log.Debug(StartingMessage));
			conversation.Started += ((x, y) => Log.Debug(StartedMessage));
		}

		#endregion
	}

	public interface IMyServiceConversationCreationInterceptor : IConversationCreationInterceptor {}

	[PersistenceConversational(ConversationCreationInterceptor = typeof (IMyServiceConversationCreationInterceptor))]
	public class InheritedSillyCrudModelWithServiceConversationCreationInterceptor : SillyCrudModel
	{
		public InheritedSillyCrudModelWithServiceConversationCreationInterceptor(IDaoFactory factory) : base(factory) {}
	}

	[PersistenceConversational]
	public class InheritedSillyCrudModelWithConvetionConversationCreationInterceptor : SillyCrudModel
	{
		public InheritedSillyCrudModelWithConvetionConversationCreationInterceptor(IDaoFactory factory) : base(factory) {}
	}

	public class ConvetionConversationCreationInterceptor :
		IConversationCreationInterceptorConvention<InheritedSillyCrudModelWithConvetionConversationCreationInterceptor>
	{
		public const string StartingMessage = "Starting with convention";
		public const string StartedMessage = "Started with convention";

		public ILog Log
		{
			get { return LogManager.GetLogger(typeof (ConvetionConversationCreationInterceptor)); }
		}

		#region Implementation of IConversationCreationInterceptor

		public void Configure(IConversation conversation)
		{
			conversation.Starting += ((x, y) => Log.Debug(StartingMessage));
			conversation.Started += ((x, y) => Log.Debug(StartedMessage));
		}

		#endregion
	}

	[PersistenceConversational(DefaultEndMode = EndMode.End)]
	public class SillyCrudModelDefaultEnd : SillyCrudModel
	{
		public SillyCrudModelDefaultEnd(IDaoFactory factory) : base(factory) { }
	}

	public interface ISillyCrudModelExtended : ISillyCrudModel
	{
		string PropertyOutConversation { get; }
		string PropertyInConversation { get; }
		void DoSomethingNoPersistent();
	}

	[PersistenceConversational(MethodsIncludeMode = MethodsIncludeMode.Implicit)]
	public class SillyCrudModelWithImplicit : ISillyCrudModelExtended
	{
		private readonly IDaoFactory factory;

		public SillyCrudModelWithImplicit(IDaoFactory factory)
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

		public virtual IList<Silly> GetEntirelyList()
		{
			return EntityDao.GetAll();
		}

		public virtual Silly GetIfAvailable(Guid id)
		{
			return EntityDao.Get(id);
		}

		public virtual Silly Save(Silly entity)
		{
			return EntityDao.MakePersistent(entity);
		}

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

		public string PropertyOutConversation
		{
			[PersistenceConversation(Exclude = true)]
			get { return null; }
		}

		public string PropertyInConversation
		{
			get { return null; }
		}

		[PersistenceConversation(Exclude = true)]
		public virtual void DoSomethingNoPersistent() {}
	}
}