using System;
using System.Collections.Generic;

namespace uNhAddIns.SessionEasier.Conversations
{
	/// <summary>
	/// Contract of a pesistence conversation.
	/// </summary>
	public interface IConversation : IDisposable, IEqualityComparer<IConversation>
	{
		/// <summary>
		/// Conversation identifier.
		/// </summary>
		string Id { get; }

		/// <summary>
		/// Conversation context.
		/// </summary>
		IDictionary<string, object> Context { get; }

		/// <summary>
		/// Start the conversation.
		/// </summary>
		void Start();

		/// <summary>
		/// Pause the conversation.
		/// </summary>
		void Pause();

		/// <summary>
		/// Pause and Flushes the conversation.
		/// </summary>
		void FlushAndPause();

		/// <summary>
		/// Resume the conversation.
		/// </summary>
		void Resume();

		/// <summary>
		/// Finalize the conversation.
		/// </summary>
		void End();

		/// <summary>
		/// Abort the conversation
		/// </summary>
		void Abort();

		/// <summary>
		/// Fired before start the conversation.
		/// </summary>
		event EventHandler<EventArgs> Starting;

		/// <summary>
		/// Fired after start the conversation.
		/// </summary>
		event EventHandler<EventArgs> Started;

		/// <summary>
		/// Fired before pause the conversation.
		/// </summary>
		event EventHandler<EventArgs> Pausing;

		/// <summary>
		/// Fired after pause the conversation.
		/// </summary>
		event EventHandler<EventArgs> Paused;

		/// <summary>
		/// Fired before resume the conversation.
		/// </summary>
		event EventHandler<EventArgs> Resuming;

		/// <summary>
		/// Fired after resume the conversation.
		/// </summary>
		event EventHandler<EventArgs> Resumed;

		/// <summary>
		/// Fired before end the conversation.
		/// </summary>
		event EventHandler<EventArgs> Ending;

		/// <summary>
		/// Fired before abort the conversation.
		/// </summary>
		event EventHandler<EventArgs> Aborting;

		/// <summary>
		/// Fired after end the conversation.
		/// </summary>
		event EventHandler<EndedEventArgs> Ended;

		/// <summary>
		/// Fired when a conversation-method exits with an exception.
		/// </summary>
		event EventHandler<OnExceptionEventArgs> OnException;
	}

	/// <summary>
	/// An <see cref="EventArgs"/> for <see cref="IConversation.Ended"/>
	/// </summary>
	public class EndedEventArgs: EventArgs
	{
		/// <summary>
		/// true if the event happen during disposing; false when the <see cref="IConversation.End"/>
		/// is explicit called. 
		/// </summary>
		public bool Disposing { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="EndedEventArgs"/> class
		/// </summary>
		/// <param name="disposing">true if the event happen during disposing</param>
		public EndedEventArgs(bool disposing)
		{
			Disposing = disposing;
		}
	}

	public enum ConversationAction
	{
		Start,
		Pause,
		FlushAndPause,
		Resume,
		End,
		Abort
	}

	public class OnExceptionEventArgs : EventArgs
	{
		public OnExceptionEventArgs(ConversationAction action, Exception exception)
		{
			Action = action;
			Exception = exception;
			ReThrow = true;
		}

		public ConversationAction Action { get; set; }
		public Exception Exception { get; private set; }
		public bool ReThrow { get; set; }
	}
}