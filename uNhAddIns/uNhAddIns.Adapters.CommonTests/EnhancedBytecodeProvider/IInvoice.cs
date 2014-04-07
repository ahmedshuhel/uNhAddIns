using System.Collections.Generic;

namespace uNhAddIns.Adapters.CommonTests.EnhancedBytecodeProvider
{
	public interface IInvoice
	{
		string Description { get; set; }
		IList<InvoiceItem> Items { get; set; }
		InvoiceItem AddItem(Product product, int quantity);
		decimal Tax { get; set; }
		decimal Total { get; }
	}
}