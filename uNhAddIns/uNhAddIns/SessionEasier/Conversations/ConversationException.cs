using System;
using System.Runtime.Serialization;

namespace uNhAddIns.SessionEasier.Conversations
{
	[Serializable]
	public class ConversationException : ApplicationException
	{
		public ConversationException(string message) : base(message) {}
		public ConversationException(string message, Exception inner) : base(message, inner) {}

		protected ConversationException(SerializationInfo info,
		                                StreamingContext context) : base(info, context) {}
	}
}