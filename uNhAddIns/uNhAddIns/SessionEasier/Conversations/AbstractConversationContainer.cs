using System;
using System.Collections.Generic;

namespace uNhAddIns.SessionEasier.Conversations
{
	[Serializable]
	public abstract class AbstractConversationContainer : IConversationContainer
	{
		protected abstract string CurrentId { get; set; }
		protected abstract IDictionary<string, IConversation> Store{ get;}

		#region Implementation of IConversationContainer

		public IConversation Get(string conversationId)
		{
			if (conversationId == null)
			{
				throw new ArgumentNullException("conversationId");
			}
			IConversation result;
			if (Store.TryGetValue(conversationId, out result))
			{
				return result;
			}
			return null;
		}

		public IConversation CurrentConversation
		{
			get
			{
				return CurrentId != null ? Get(CurrentId) : null;
			}
		}

		public IConversation Unbind(string conversationId)
		{
			if (conversationId == null)
			{
				return null;
			}
			IConversation c = Get(conversationId);
			if (c != null)
			{
				Store.Remove(c.Id);
			}
			SetCurrentIfNecessary();
			return c;
		}

		public void Bind(IConversation conversation)
		{
			if (conversation == null)
			{
				throw new ArgumentNullException("conversation");
			}
			if(AutoUnbindAfterEndConversation)
			{
				conversation.Ended += ((x, y) => Unbind(conversation.Id));
			}
			Store[conversation.Id] = conversation;
			SetCurrentIfNecessary();
		}

		protected virtual void SetCurrentIfNecessary()
		{
			IDictionary<string, IConversation> s = Store;
			if(s.Count > 1)
			{
				return;
			}
			else if (s.Count == 1)
			{
				var kenum= s.Keys.GetEnumerator();
				kenum.MoveNext();
				CurrentId = kenum.Current;
			}
			else
			{
				CurrentId = null;
			}
		}

		public void SetAsCurrent(IConversation conversation)
		{
			if (conversation == null)
			{
				throw new ArgumentNullException("conversation");
			}
			IConversation c = Get(conversation.Id);
			if (c != null)
			{
				CurrentId = c.Id;
			}
			else
			{
				Bind(conversation);
				CurrentId = conversation.Id;
			}
		}

		public void SetAsCurrent(string conversationId)
		{
			IConversation c = Get(conversationId);
			if (c != null)
			{
				CurrentId = c.Id;
			}
			else
			{
				throw new ConversationException(string.Format("Conversation not available (#{0})", conversationId));
			}
		}

		#endregion

		public virtual bool AutoUnbindAfterEndConversation { get; set; }
	}
}