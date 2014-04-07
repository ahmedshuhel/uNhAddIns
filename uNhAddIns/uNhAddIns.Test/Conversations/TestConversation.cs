using log4net;
using uNhAddIns.SessionEasier.Conversations;

namespace uNhAddIns.Test.Conversations
{
	public class TestConversation : AbstractConversation
	{
		public static readonly string DisposeMessage = "Dispose called.";
		public static readonly string StartMessage = "DoStart called.";
		public static readonly string PauseMessage = "DoPause called.";
		public static readonly string FlushAndPauseMessage = "DoPauseAndFlush called.";
		public static readonly string ResumeMessage = "DoResume called.";
		public static readonly string EndMessage = "DoEnd called.";
		public static readonly string AbortMessage = "DoAbort called.";
		public TestConversation() {}

		public TestConversation(string id) : base(id) {}

		public ILog Log
		{
			get { return LogManager.GetLogger(typeof (TestConversation)); }
		}

		#region Overrides of AbstractConversation

		protected override void Dispose(bool disposing)
		{
			if (disposing && Log.IsDebugEnabled)
			{
				Log.Debug(DisposeMessage);
			}
		}

		protected override void DoStart()
		{
			if (Log.IsDebugEnabled)
			{
				Log.Debug(StartMessage);
			}
		}

		protected override void DoPause()
		{
			if (Log.IsDebugEnabled)
			{
				Log.Debug(PauseMessage);
			}
		}

		protected override void DoFlushAndPause()
		{
			if (Log.IsDebugEnabled)
			{
				Log.Debug(FlushAndPauseMessage);
			}
		}

		protected override void DoResume()
		{
			if (Log.IsDebugEnabled)
			{
				Log.Debug(ResumeMessage);
			}
		}

		protected override void DoEnd()
		{
			if (Log.IsDebugEnabled)
			{
				Log.Debug(EndMessage);
			}
		}

		protected override void DoAbort()
		{
			if (Log.IsDebugEnabled)
			{
				Log.Debug(AbortMessage);
			}
		}

		#endregion
	}
}