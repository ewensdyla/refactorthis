using System.Collections.Generic;
using System.Linq;

namespace RefactorThis.Persistence
{
	public class Invoice
	{
		private readonly InvoiceRepository _repository;
		public Invoice( InvoiceRepository repository )
		{
			_repository = repository;
		}

		public void Save( )
		{
			_repository.SaveInvoice( this );
		}

		public decimal TotalAmount { get; set; }
		public decimal AmountPaid { get; set; }
		public List<Payment> PaymentsMade { get; set; }

		public decimal RemainingBalance()
		{
			var remainingAmount = PaymentsMade.Sum(x => x.Amount);
			return remainingAmount;
		}

	}
}