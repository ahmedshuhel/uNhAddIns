using NHibernate;
using NUnit.Framework;
using uNhAddIns.SessionEasier;
using uNhAddIns.TestUtils.NhIntegration;

namespace uNhAddIns.Test.SessionEasier
{
	[TestFixture]
	public class FakeSessionWrapperFixture : FunctionalTestCase
	{
		[Test]
		public void DoNotWrapTheSession()
		{
			// 
			var fsw = new FakeSessionWrapper();
			using (ISession session = SessionFactory.OpenSession())
			{
				ISession actual = fsw.Wrap(session, null, null);
				Assert.That(actual, Is.SameAs(session));
			}
		}

		[Test]
		public void AnyKindOfSessionIsWrapped()
		{
			var fsw = new FakeSessionWrapper();
			using (ISession session = SessionFactory.OpenSession())
			{
				Assert.That(fsw.IsWrapped(session));
			}
			Assert.That(!fsw.IsWrapped(null));
		}
	}
}