using System;
using System.Collections.Generic;

namespace uNhAddIns.SessionEasier.Conversations
{
	[Serializable]
	public abstract class AbstractConversation : IConversation
	{
		private readonly IDictionary<string, object> context = new Dictionary<string, object>(5);
		private readonly string id;

		protected AbstractConversation() : this(Guid.NewGuid().ToString()) {}

		protected AbstractConversation(string id)
		{
			if (string.IsNullOrEmpty(id))
			{
				throw new ArgumentNullException("id",
				                                "Conversation Id is not optional; Use the empty Ctor if you don't have an available Id.");
			}
			this.id = id;
		}

		#region Implementation of IDisposable

		public void Dispose()
		{
			// When the session is disposed we close every pending session.
			// If End was not called then it will not be flushed and commited as expected for a Dispose
			DoAbort();
			RaiseEnded(true);
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected abstract void Dispose(bool disposing);

		~AbstractConversation()
		{
			Dispose(false);
		}

		#endregion

		#region Implementation of IConversation

		public string Id
		{
			get { return id; }
		}

		public IDictionary<string, object> Context
		{
			get { return context; }
		}

		public virtual void Start()
		{
			RaiseStarting();
			EncloseWithExceptionManagement(ConversationAction.Start, DoStart);
			RaiseStarted();
		}

		public virtual void Pause()
		{
			RaisePausing();
			EncloseWithExceptionManagement(ConversationAction.Pause, DoPause);
			RaisePaused();
		}

		public virtual void FlushAndPause()
		{
			RaisePausing();
			EncloseWithExceptionManagement(ConversationAction.FlushAndPause, DoFlushAndPause);
			RaisePaused();
		}

		public virtual void Resume()
		{
			RaiseResuming();
			EncloseWithExceptionManagement(ConversationAction.Resume, DoResume);
			RaiseResumed();
		}

		public virtual void End()
		{
			RaiseEnding();
			EncloseWithExceptionManagement(ConversationAction.End, DoEnd);
			RaiseEnded(false);
		}

		public virtual void Abort()
		{
			RaiseAborting();
			EncloseWithExceptionManagement(ConversationAction.Abort, DoAbort);
			RaiseEnded(false);
		}

		protected bool RaiseOnException(ConversationAction actionType, Exception e)
		{
			bool reThrow = true;
			if (OnException != null)
			{
				var eventArgs = new OnExceptionEventArgs(actionType, e);
				OnException(this, eventArgs);
				reThrow = eventArgs.ReThrow;
			}
			return reThrow;
		}

		protected delegate void ConvAction();

		private void EncloseWithExceptionManagement(ConversationAction actionType, ConvAction action)
		{
			try
			{
				action();
			}
			catch (Exception e)
			{
				bool reThrow = RaiseOnException(actionType, e);
				if (reThrow)
				{
					throw new ConversationException(
						string.Format("Exception during persistent conversation {0}:{1}", actionType, e.Message), e);
				}
			}
		}

		public event EventHandler<EventArgs> Starting;
		public event EventHandler<EventArgs> Started;
		public event EventHandler<EventArgs> Pausing;
		public event EventHandler<EventArgs> Paused;
		public event EventHandler<EventArgs> Resuming;
		public event EventHandler<EventArgs> Resumed;
		public event EventHandler<EventArgs> Ending;
		public event EventHandler<EventArgs> Aborting;
		public event EventHandler<EndedEventArgs> Ended;
		public event EventHandler<OnExceptionEventArgs> OnException;

		protected abstract void DoStart();

		protected void RaiseStarting()
		{
			if (Starting != null)
			{
				Starting(this, new EventArgs());
			}
		}

		private void RaiseStarted()
		{
			if (Started != null)
			{
				Started(this, new EventArgs());
			}
		}

		protected abstract void DoFlushAndPause();

		protected abstract void DoPause();

		protected void RaisePausing()
		{
			if (Pausing != null)
			{
				Pausing(this, new EventArgs());
			}
		}

		protected void RaisePaused()
		{
			if (Paused != null)
			{
				Paused(this, new EventArgs());
			}
		}

		protected abstract void DoResume();

		protected void RaiseResuming()
		{
			if (Resuming != null)
			{
				Resuming(this, new EventArgs());
			}
		}

		protected void RaiseResumed()
		{
			if (Resumed != null)
			{
				Resumed(this, new EventArgs());
			}
		}

		protected abstract void DoEnd();

		protected void RaiseEnding()
		{
			if (Ending != null)
			{
				Ending(this, new EventArgs());
			}
		}

		protected void RaiseEnded(bool disposing)
		{
			if (Ended != null)
			{
				Ended(this, new EndedEventArgs(disposing));
			}
		}

		protected abstract void DoAbort();

		protected void RaiseAborting()
		{
			if (Aborting != null)
			{
				Aborting(this, new EventArgs());
			}
		}

		#endregion

		public override bool Equals(object obj)
		{
			var that = obj as IConversation;
			return Equals(that);
		}

		public bool Equals(IConversation obj)
		{
			if (null == obj)
			{
				return false;
			}
			return ReferenceEquals(this, obj) || obj.Id.Equals(id);
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}

		#region Implementation of IEqualityComparer<IConversation>

		public bool Equals(IConversation x, IConversation y)
		{
			if (x == null && y == null)
			{
				return true;
			}
			if (x == null || y == null)
			{
				return false;
			}
			return x.Id.Equals(y.Id);
		}

		public int GetHashCode(IConversation obj)
		{
			return obj != null ? obj.GetHashCode() : 0;
		}

		#endregion
	}
}