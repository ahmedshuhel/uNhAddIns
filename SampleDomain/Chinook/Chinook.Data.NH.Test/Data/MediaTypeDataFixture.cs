using Chinook.Domain;
using NHibernate;
using NUnit.Framework;

namespace Chinook.Data.NH.Test.Data
{
    [TestFixture]
    public class MediaTypeDataFixture : DataTestFixtureBase
    {
        [Test]
        public void can_get_mediatype_1()
        {
            using (ISession session = SessionFactory.OpenSession())
            {
                var mediaType = session.Get<MediaType>(1);
                mediaType.Name.Should().Be.EqualTo("MPEG audio file");
            }
        }
    }
}