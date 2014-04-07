using NHibernate;
using NHibernate.Cfg;
using NUnit.Framework;

namespace Chinook.Data.NH.Test.Data
{
    [TestFixture]
    public class DataTestFixtureBase
    {
        protected ISessionFactory SessionFactory;

        [TestFixtureSetUp]
        public void SetUp()
        {
            var cfg = new Configuration();
            cfg.Configure();
            SessionFactory = cfg.BuildSessionFactory();
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            SessionFactory.Close();
        }
    }
}