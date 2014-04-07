using Chinook.Domain;
using NHibernate;
using NUnit.Framework;

namespace Chinook.Data.NH.Test.Data
{
    [TestFixture]
    public class AlbumDataFixture : DataTestFixtureBase
    {
        [Test]
        public void can_get_album_1()
        {
            using (ISession session = SessionFactory.OpenSession())
            {
                var album = session.Get<Album>(1);
                album.Artist.ArtistId.Should().Be.EqualTo(1);
                album.Title.Should().Be.EqualTo("For Those About To Rock We Salute You");
                album.Tracks.Count.Should().Be.EqualTo(10);
                album.Tracks[0].Name.Should().Be.EqualTo("For Those About To Rock (We Salute You)"); 
                album.Tracks[0].MediaType.MediaTypeId.Should().Be.EqualTo(1);
                album.Tracks[0].Composer.Should().Be.EqualTo("Angus Young, Malcolm Young, Brian Johnson");
            }
        }
    }
}