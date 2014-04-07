using Chinook.Domain;
using NHibernate;
using NUnit.Framework;

namespace Chinook.Data.NH.Test.Data
{
    [TestFixture]
    public class CustomerDataFixture : DataTestFixtureBase
    {
        [Test]
        public void can_get_customer_1()
        {
            using (ISession session = SessionFactory.OpenSession())
            {
                var customer = session.Get<Customer>(1);
                customer.FirstName.Should().Be.EqualTo("Luís");
                customer.LastName.Should().Be.EqualTo("Gonçalves");
                customer.Company.Should().Be.EqualTo("Embraer - Empresa Brasileira de Aeronáutica S.A.");
                customer.Address.Should().Be.EqualTo("Av. Brigadeiro Faria Lima, 2170");
                customer.City.Should().Be.EqualTo("São José dos Campos");
                customer.State.Should().Be.EqualTo("SP");
                customer.Country.Should().Be.EqualTo("Brazil");
                customer.PostalCode.Should().Be.EqualTo("12227-000");
                customer.Phone.Should().Be.EqualTo("+55 (12) 3923-5555");
            }
        }
    }
}