using System.Collections.Generic;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Metadata;
using NHibernate.Tool.hbm2ddl;
using NUnit.Framework;

namespace Chinook.Data.NH.Test.Schema
{
    [TestFixture]
    public class SchemaTestFixture
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            _cfg = new Configuration();
            _cfg.Configure();
        }

        #endregion

        private Configuration _cfg;

        [Test]
        public void AllNHibernateMappingAreOkay()
        {
            ISessionFactory sessionFactory = _cfg.BuildSessionFactory();


            using (ISession session = sessionFactory.OpenSession())
            {
                IDictionary<string, IClassMetadata> allClassMetadata = session.SessionFactory.GetAllClassMetadata();

                foreach (var entry in allClassMetadata)
                {
                    session.CreateCriteria(entry.Key)
                        .SetMaxResults(0).List();
                }
            }
        }

        [Test]
        public void ValidateSchema()
        {
            
            var schemaValidator = new SchemaValidator(_cfg);
            schemaValidator.Validate();
        }

        [Test]
        public void CreateSchema()
        {
            SchemaExport se = new SchemaExport(_cfg);
            se.Create(true, true);
        }
    }
}