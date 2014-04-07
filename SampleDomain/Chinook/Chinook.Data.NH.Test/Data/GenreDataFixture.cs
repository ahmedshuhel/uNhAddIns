using Chinook.Domain;
using NHibernate;
using NUnit.Framework;

namespace Chinook.Data.NH.Test.Data
{
    [TestFixture]
    public class GenreDataFixture : DataTestFixtureBase
    {
        [Test]
        public void can_get_genre_1()
        {
            using (ISession session = SessionFactory.OpenSession())
            {
                var genre = session.Get<Genre>(1);

                genre.Name.Should().Be.EqualTo("Rock");
            }
        }

    }
}