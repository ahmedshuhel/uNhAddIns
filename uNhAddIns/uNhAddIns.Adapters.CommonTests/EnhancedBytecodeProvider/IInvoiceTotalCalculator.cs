namespace uNhAddIns.Adapters.CommonTests.EnhancedBytecodeProvider
{
	public interface IInvoiceTotalCalculator
	{
		decimal GetTotal(IInvoice invoice);
	}
}