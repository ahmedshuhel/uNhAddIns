using System;
using System.Linq.Expressions;
using NHibernate;
using NUnit.Framework;
using uNhAddIns.SessionEasier;
using uNhAddIns.TestUtils;
using uNhAddIns.TestUtils.NhIntegration;

namespace uNhAddIns.Test.SessionEasier
{
	[TestFixture]
	public class TransactionProtectionWrapperFixture : FunctionalTestCase
	{
		public class BasicTransactionProtectionWrapperCrack: BasicTransactionProtectionWrapper
		{
			public BasicTransactionProtectionWrapperCrack(ISession realSession, SessionCloseDelegate closeDelegate) : base(realSession, closeDelegate) {}
			public BasicTransactionProtectionWrapperCrack(ISession realSession, SessionCloseDelegate closeDelegate, SessionDisposeDelegate disposeDelegate) : base(realSession, closeDelegate, disposeDelegate) {}

			public static object ContinueExecutionMarker
			{
				get { return InvokeImplementation; }
			}
		}
	
		[Test]
		public void CtorProtection()
		{
			Assert.Throws<ArgumentNullException>(() => new BasicTransactionProtectionWrapper(null, null));
		}

		[Test]
		public void ShouldProtectMethodsNeedingTransaction()
		{
			var methods = new Expression<Action<ISession>>[]
			              	{
			              		s => s.CreateQuery("from System.Object"), 
												s => s.CreateCriteria(typeof(object)),
												s => s.Load(typeof(object), 0),
												s => s.Get(typeof(object), 0)
			              	};

			var session = SessionFactory.OpenSession();
			var tpw = new BasicTransactionProtectionWrapper(session, null);
			foreach (var protectedMethod in Reflector.MethodsInfos(methods))
			{
				Assert.Throws<HibernateException>(() => tpw.Invoke(protectedMethod, null));				
			}
			session.Close();
		}

		[Test]
		public void ShouldNotProtectMethodsNotNeedingTransaction()
		{
			var notProtectedMethods = new[]
			              	{
			              		Reflector.PropertyGetter<ISession>(s => s.Statistics),
			              		Reflector.PropertyGetter<ISession>(s => s.IsOpen),
			              		Reflector.PropertyGetter<ISession>(s => s.FlushMode)
			              	};
			var session = SessionFactory.OpenSession();
			var tpw = new BasicTransactionProtectionWrapper(session, null);
			foreach (var notProtectedMethod in notProtectedMethods)
			{
				Assert.That(tpw.Invoke(notProtectedMethod, null), Is.EqualTo(BasicTransactionProtectionWrapperCrack.ContinueExecutionMarker));
			}
			session.Close();
		}

		[Test]
		public void AllowAccessToInternalProxy()
		{
			var session = SessionFactory.OpenSession();
			var tpw = new BasicTransactionProtectionWrapper(session, null);
			var methodInfo = Reflector.PropertyGetter<ISessionProxy>(p=> p.InvocationHandler);
			Assert.That(tpw.Invoke(methodInfo, null), Is.EqualTo(tpw));
			session.Close();
		}

		[Test]
		public void ShouldCallTheCloseActionInExplictSessionClose()
		{
			bool closeActionCalled = false;
			var session = SessionFactory.OpenSession();
			var tpw = new BasicTransactionProtectionWrapper(session, s =>
			                                                         	{
			                                                         		closeActionCalled = true;
			                                                         		return s;
			                                                         	});
			var methodInfo = Reflector.MethodInfo<ISession>(s => s.Close());
			tpw.Invoke(methodInfo, null);
			Assert.That(closeActionCalled);
		}

		[Test]
		public void ShouldCallTheDisposeActionInExplictSessionDispose()
		{
			bool disposeActionCalled = false;
			var session = SessionFactory.OpenSession();
			var tpw = new BasicTransactionProtectionWrapper(session, null, s => disposeActionCalled = true);
			var methodInfo = Reflector.MethodInfo<ISession>(s => s.Dispose());
			tpw.Invoke(methodInfo, null);
			Assert.That(disposeActionCalled);
		}

		[Test]
		public void ShouldCallTheCloseAndDisposeActionInExplictSessionDispose()
		{
			bool closeActionCalled = false;
			bool disposeActionCalled = false;
			var session = SessionFactory.OpenSession();
			var tpw = new BasicTransactionProtectionWrapper(session, s =>
			                                                         	{
			                                                         		closeActionCalled = true;
			                                                         		return s;
			                                                         	}, 
																																s => disposeActionCalled = true);
			var methodInfo = Reflector.MethodInfo<ISession>(s => s.Dispose());
			tpw.Invoke(methodInfo, null);
			Assert.That(closeActionCalled);
			Assert.That(disposeActionCalled);
		}
	}
}