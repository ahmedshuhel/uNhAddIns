using System.Collections.Generic;

namespace uNhAddIns.Adapters.CommonTests.EnhancedBytecodeProvider
{
	public class Invoice : IInvoice
	{
		private readonly IInvoiceTotalCalculator calculator;

		public Invoice(IInvoiceTotalCalculator calculator)
		{
			this.calculator = calculator;
			Items = new List<InvoiceItem>();
		}

		#region IInvoice Members

		public string Description { get; set; }
		public decimal Tax { get; set; }
		public IList<InvoiceItem> Items { get; set; }

		public decimal Total
		{
			get { return calculator.GetTotal(this); }
		}

		public InvoiceItem AddItem(Product product, int quantity)
		{
			var result = new InvoiceItem(product, quantity);
			Items.Add(result);
			return result;
		}

		#endregion
	}
}