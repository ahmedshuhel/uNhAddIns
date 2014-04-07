using Chinook.Domain;
using NHibernate;
using NUnit.Framework;

namespace Chinook.Data.NH.Test.Data
{
    [TestFixture]
    public class EmployeeDataFixture : DataTestFixtureBase
    {
        [Test]
        public void can_get_employee_1()
        {
            using (ISession session = SessionFactory.OpenSession())
            {
                var employee = session.Get<Employee>(2);
                employee.FirstName.Should().Be.EqualTo("Nancy");
                employee.LastName.Should().Be.EqualTo("Edwards");
            } 
        }
        
    }
}