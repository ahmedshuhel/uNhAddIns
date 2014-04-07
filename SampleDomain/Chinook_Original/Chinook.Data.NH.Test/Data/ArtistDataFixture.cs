using Chinook.Domain;
using NHibernate;
using NUnit.Framework;

namespace Chinook.Data.NH.Test.Data
{
    [TestFixture]
    public class ArtistDataFixture : DataTestFixtureBase
    {
        [Test]
        public void can_get_artist_1()
        {
            using (ISession session = SessionFactory.OpenSession())
            {
                var artist = session.Get<Artist>(1);

                Assert.AreEqual("AC/DC", artist.Name);
            }
        }
    }
}