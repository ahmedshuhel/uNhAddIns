namespace uNhAddIns.Adapters.CommonTests.EnhancedBytecodeProvider
{
	public class InvoiceItem
	{
		protected InvoiceItem() {}

		public InvoiceItem(Product product, int quantity)
		{
			Product = product;
			Quantity = quantity;
		}

		public Product Product { get; private set; }
		public int Quantity { get; set; }
	}
}