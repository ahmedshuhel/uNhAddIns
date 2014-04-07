using Chinook.Domain;
using NHibernate;
using NUnit.Framework;

namespace Chinook.Data.NH.Test.Data
{
    [TestFixture]
    public class PlaylistDataFixture : DataTestFixtureBase
    {
        [Test]
        public void can_get_Playlist_1()
        {
            using (ISession session = SessionFactory.OpenSession())
            {
                var playlist = session.Get<Playlist>(1);
                playlist.Name.Should().Be.EqualTo("Music");
                playlist.Tracks.Count.Should().Be.EqualTo(3290);
                playlist.Tracks[0].Name.Should().Be.EqualTo("For Those About To Rock (We Salute You)");
                playlist.Tracks[0].MediaType.MediaTypeId.Should().Be.EqualTo(1);
                playlist.Tracks[0].Composer.Should().Be.EqualTo("Angus Young, Malcolm Young, Brian Johnson");
            }
        }
    }
}