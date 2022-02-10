using System.Collections.Generic;

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

		public decimal AmountRemaining { get; set; }
		public decimal AmountPaid { get; set; }
		public List<Payment> PaymentsMade { get; set; }

	}
}