using System;
using log4net.Config;
using NUnit.Framework;
using uNhAddIns.SessionEasier.Conversations;
using uNhAddIns.TestUtils.Logging;

namespace uNhAddIns.Test.Conversations
{
	[TestFixture]
	public class ConversationFixture
	{
		public ConversationFixture()
		{
			XmlConfigurator.Configure();
		}

		[Test]
		public void Ctors()
		{
			Assert.That((new TestConversation()).Id, Is.Not.Null);
			Assert.Throws<ArgumentNullException>(() => new TestConversation(null));
			Assert.Throws<ArgumentNullException>(() => new TestConversation(""));
		}

		[Test]
		public void Equality()
		{
			var t = new TestConversation();
			Assert.That(t.Equals(t));
			Assert.That(!t.Equals(new TestConversation()));
			Assert.That(!t.Equals(null));
			Assert.That(!t.Equals(new object()));

			string key1 = "ModelName" + Guid.NewGuid();
			string key2 = Guid.NewGuid().ToString();
			t = new TestConversation(key1);
			Assert.That(t.Equals(new TestConversation(key1)));
			Assert.That(!t.Equals(new TestConversation(key2)));
			Assert.That(t.Equals(null, null));
			Assert.That(!t.Equals(new TestConversation(key1), null));
			Assert.That(!t.Equals(null, new TestConversation(key1)));
			Assert.That(t.Equals(new TestConversation(key1), new TestConversation(key1)));
			Assert.That(!t.Equals(new TestConversation(key1), new TestConversation(key2)));
		}

		[Test]
		public void HashCode()
		{
			string key1 = "ModelName" + Guid.NewGuid();
			string key2 = Guid.NewGuid().ToString();
			var t = new TestConversation(key1);
			Assert.That(t.GetHashCode(), Is.EqualTo(key1.GetHashCode()));
			Assert.That(t.GetHashCode(new TestConversation(key2)), Is.EqualTo(key2.GetHashCode()));
		}

		[Test]
		public void StartCallSequence()
		{
			// I'm using log instad a mock, I know
			using (var t = new TestConversation())
			{
				const string starting = "Starting called.";
				const string started = "Started called.";
				t.Starting += ((x, y) => t.Log.Debug(starting));
				t.Started += ((x, y) => t.Log.Debug(started));

				Assert.That(Spying.Logger<TestConversation>()
					.Execute(t.Start)
					.MessageSequence, 
					Is.EqualTo(new[] { starting, TestConversation.StartMessage,started }));
			}
		}

		[Test]
		public void PauseCallSequence()
		{
			using (var t = new TestConversation())
			{
				const string pausing = "Pausing called.";
				const string paused = "Paused called.";
				t.Pausing += ((x, y) => t.Log.Debug(pausing));
				t.Paused += ((x, y) => t.Log.Debug(paused));
				Assert.That(Spying.Logger<TestConversation>()
					.Execute(t.Pause)
					.MessageSequence,
					Is.EqualTo(new[] { pausing, TestConversation.PauseMessage, paused}));
			}
		}

		[Test]
		public void PauseAndFlushCallSequence()
		{
			using (var t = new TestConversation())
			{
				const string pausing = "Pausing called.";
				const string paused = "Paused called.";
				t.Pausing += ((x, y) => t.Log.Debug(pausing));
				t.Paused += ((x, y) => t.Log.Debug(paused));
				Assert.That(Spying.Logger<TestConversation>()
					.Execute(t.FlushAndPause)
					.MessageSequence,
					Is.EqualTo(new[] { pausing, TestConversation.FlushAndPauseMessage, paused }));
			}
		}

		[Test]
		public void ResumeCallSequence()
		{
			using (var t = new TestConversation())
			{
				const string resuming = "Resuming called.";
				const string resumed = "Resumed called.";
				t.Resuming += ((x, y) => t.Log.Debug(resuming));
				t.Resumed += ((x, y) => t.Log.Debug(resumed));
				Assert.That(Spying.Logger<TestConversation>()
					.Execute(t.Resume)
					.MessageSequence,
					Is.EqualTo(new[] { resuming, TestConversation.ResumeMessage, resumed}));
			}
		}

		[Test]
		public void EndCallSequence()
		{
			using (var t = new TestConversation())
			{
				const string ending = "Ending called.";
				const string ended = "Ended called.";
				t.Ending += ((x, y) => t.Log.Debug(ending));
				t.Ended += ((x, y) => t.Log.Debug(ended));
				t.Ended += AssertEndedOutOfDisposing;
				Assert.That(Spying.Logger<TestConversation>()
					.Execute(t.End)
					.MessageSequence,
					Is.EqualTo(new[] { ending, TestConversation.EndMessage, ended }));

				t.Ended -= AssertEndedOutOfDisposing;
			}
		}

		public void AssertEndedOutOfDisposing(object x, EndedEventArgs y)
		{
			Assert.That(!y.Disposing);
		}

		[Test]
		public void AbortCallSequence()
		{
			using (var t = new TestConversation())
			{
				const string ended = "Ended called.";
				const string aborting = "Aborting called.";
				t.Ending += ((x, y) => t.Log.Debug("Ending called."));
				t.Aborting += ((x, y) => t.Log.Debug(aborting));
				t.Ended += ((x, y) => t.Log.Debug(ended));
				t.Ended += AssertEndedOutOfDisposing;
				Assert.That(Spying.Logger<TestConversation>()
					.Execute(t.Abort)
					.MessageSequence,
					Is.EqualTo(new[] { aborting, TestConversation.AbortMessage, ended }));

				t.Ended -= AssertEndedOutOfDisposing;
			}
		}

		[Test]
		public void DisposeCallSequence()
		{
			const string ended = "End called.";
			Assert.That(Spying.Logger<TestConversation>()
				.Execute(() =>
				         	{
										using (var t = new TestConversation())
										{
											t.Ended += ((x, y) => t.Log.Debug(ended));
											t.Ended += ((x, y) => Assert.That(y.Disposing));
										}

				         	})
				.MessageSequence,
				Is.EqualTo(new[] { TestConversation.AbortMessage, ended, TestConversation.DisposeMessage }));
		}

		public class ConversationError : AbstractConversation
		{
			#region Overrides of AbstractConversation

			protected override void Dispose(bool disposing) {}

			protected override void DoStart()
			{
				throw new NotImplementedException(ConversationAction.Start.ToString());
			}

			protected override void DoFlushAndPause()
			{
				throw new NotImplementedException(ConversationAction.FlushAndPause.ToString());
			}

			protected override void DoPause()
			{
				throw new NotImplementedException(ConversationAction.Pause.ToString());
			}

			protected override void DoResume()
			{
				throw new NotImplementedException(ConversationAction.Resume.ToString());
			}

			protected override void DoEnd()
			{
				throw new NotImplementedException(ConversationAction.End.ToString());
			}

			protected override void DoAbort()
			{
				throw new NotImplementedException(ConversationAction.Abort.ToString());
			}

			#endregion
		}

		[Test]
		public void OnExceptionWithoutReThrow()
		{
			try
			{
				using (var t = new ConversationError())
				{
					t.OnException += AssertException;
					t.Start();
					t.Pause();
					t.FlushAndPause();
					t.Resume();
					t.End();
					t.Abort();
				}
			}
			catch (NotImplementedException)
			{
				// The Abort during Dispose is not managed by the OnException events
			}
		}

		[Test]
		public void OnExceptionReThrow()
		{
			try
			{
				using (var t = new ConversationError())
				{
					Assert.Throws<ConversationException>(t.Start);
					Assert.Throws<ConversationException>(t.Resume);
					Assert.Throws<ConversationException>(t.Pause);
					Assert.Throws<ConversationException>(t.FlushAndPause);
					Assert.Throws<ConversationException>(t.End);
					Assert.Throws<ConversationException>(t.Abort);
				}
			}
			catch (NotImplementedException)
			{
				// The Abort during Dispose
			}
		}

		private static void AssertException(object conversation, OnExceptionEventArgs args)
		{
			Assert.That(args.Exception, Is.InstanceOf(typeof (NotImplementedException)));
			Assert.That(args.Exception.Message, Is.EqualTo(args.Action.ToString()));
			args.ReThrow = false;
		}
	}
}