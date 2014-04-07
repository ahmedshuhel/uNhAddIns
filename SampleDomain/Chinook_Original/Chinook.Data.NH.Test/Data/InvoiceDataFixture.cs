using System;
using Chinook.Domain;
using NHibernate;
using NUnit.Framework;

namespace Chinook.Data.NH.Test.Data
{
    [TestFixture]
    public class InvoiceDataFixture : DataTestFixtureBase
    {
        [Test]
        public void can_get_invoice_1()
        {
            using (ISession session = SessionFactory.OpenSession())
            {
                var invoice = session.Get<Invoice>(1);
                invoice.Customer.CustomerId.Should().Be.EqualTo(46);
                invoice.InvoiceDate.Should().Be.EqualTo(new DateTime(2007, 01, 02));
                invoice.BillingAddress.Should().Be.EqualTo("3 Chatham Street");
                invoice.BillingCity.Should().Be.EqualTo("Dublin");
                invoice.BillingState.Should().Be.EqualTo("Dublin");
                invoice.BillingCountry.Should().Be.EqualTo("Ireland");
                invoice.BillingPostalCode.Should().Be.Null();
                invoice.Total.Should().Be.EqualTo((decimal)3.96);
                invoice.Lines.Count.Should().Be.EqualTo(4);

                var invoiceLine = invoice.Lines[0];
                invoiceLine.InvoiceLineId.Should().Be.EqualTo(1);
                invoiceLine.UnitPrice.Should().Be.EqualTo((decimal)0.99);
                invoiceLine.Quantity.Should().Be.EqualTo(1);

            }
        }
    }
}