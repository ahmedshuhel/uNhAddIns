using System.Collections.Generic;
using log4net.Config;
using Microsoft.Practices.ServiceLocation;
using NHibernate;
using NHibernate.Bytecode;
using NHibernate.Cfg;
using NHibernate.Engine;
using NHibernate.Tool.hbm2ddl;
using NUnit.Framework;

namespace uNhAddIns.Adapters.CommonTests.EnhancedBytecodeProvider
{
	public abstract class AbstractEntityInjectionFixture
	{
		private ISessionFactoryImplementor sessions;
		private Configuration cfg;

		static AbstractEntityInjectionFixture()
		{
			XmlConfigurator.Configure();
		}

		/// <summary>
		/// Initialize the ServiceLocator registering all services needed by this test.
		/// </summary>
		/// <remarks>
		/// The initialization must include the initialization of <see cref="ServiceLocator"/>.
		/// 
		/// Services needed, in this test, are:
		/// 
		/// - uNhAddIns.Adapters.CommonTests.EnhancedBytecodeProvider.IInvoiceTotalCalculator 
		///		Implementation: uNhAddIns.Adapters.CommonTests.EnhancedBytecodeProvider.SumAndTaxTotalCalculator
		/// 
		/// - uNhAddIns.Adapters.CommonTests.EnhancedBytecodeProvider.IInvoice
		///		Implementation : uNhAddIns.Adapters.CommonTests.EnhancedBytecodeProvider.Invoice
		///		Lyfe : Transient/Prototype
		/// </remarks>
		protected abstract void InitializeServiceLocator();

		protected abstract IBytecodeProvider GetBytecodeProvider();

		[TestFixtureSetUp]
		public void CreateDb()
		{
			InitializeServiceLocator();
			cfg = new Configuration();
			Environment.BytecodeProvider = GetBytecodeProvider();
			cfg.Configure();
			cfg.AddResource("uNhAddIns.Adapters.CommonTests.EnhancedBytecodeProvider.Domain.Spechbm.xml",
			                typeof (AbstractEntityInjectionFixture).Assembly);
			new SchemaExport(cfg).Create(false, true);
			sessions = (ISessionFactoryImplementor) cfg.BuildSessionFactory();
		}

		[TestFixtureTearDown]
		public void CloseSessionFactory()
		{
			if (sessions != null)
			{
				sessions.Dispose();
			}
			new SchemaExport(cfg).Drop(false, true);
			sessions = null;
		}

		[Test]
		public void CRUDUsingRefectionOptimizer()
		{
			IServiceLocator serviceLocator = ServiceLocator.Current;

			Product p1;
			Product p2;
			using (ISession s = sessions.OpenSession())
			{
				using (ITransaction tx = s.BeginTransaction())
				{
					p1 = new Product { Description = "P1", Price = 10 };
					p2 = new Product { Description = "P2", Price = 20 };
					s.Save(p1);
					s.Save(p2);
					tx.Commit();
				}
			}

			var invoice = serviceLocator.GetInstance<IInvoice>();
			invoice.Tax = 1000;
			invoice.AddItem(p1, 1);
			invoice.AddItem(p2, 2);
			Assert.That(invoice.Total, Is.EqualTo((decimal)(10 + 40 + 1000)));

			object savedInvoice;
			using (ISession s = sessions.OpenSession())
			{
				using (ITransaction tx = s.BeginTransaction())
				{
					savedInvoice = s.Save(invoice);
					tx.Commit();
				}
			}

			using (ISession s = sessions.OpenSession())
			{
				invoice = s.Get<Invoice>(savedInvoice);
				Assert.That(invoice.Total, Is.EqualTo((decimal)(10 + 40 + 1000)));
			}

			using (ISession s = sessions.OpenSession())
			{
				invoice = (IInvoice)s.Load(typeof(Invoice), savedInvoice);
				Assert.That(invoice.Total, Is.EqualTo((decimal)(10 + 40 + 1000)));
			}

			using (ISession s = sessions.OpenSession())
			{
				IList<IInvoice> l = s.CreateQuery("from Invoice").List<IInvoice>();
				invoice = l[0];
				Assert.That(invoice.Total, Is.EqualTo((decimal)(10 + 40 + 1000)));
			}

			using (ISession s = sessions.OpenSession())
			{
				using (ITransaction tx = s.BeginTransaction())
				{
					s.Delete("from Invoice");
					s.Delete("from Product");
					tx.Commit();
				}
			}
		}

	}
}