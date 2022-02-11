using System;
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
			PaymentsMade = new List<Payment>();
		}

		public void Save( )
		{
			_repository.SaveInvoice( this );
		}

		public decimal TotalAmount { get; set; }
		public decimal AmountPaid { get; set; }
		public List<Payment> PaymentsMade { get; }

		public decimal AddPayment(Payment payment)
		{
			PaymentsMade.Add(payment);
			AmountPaid += payment.Amount;
			return RemainingBalance();
		}

		public decimal RemainingBalance()
		{
			var sumRemainingAmount = TotalAmount - PaymentsMade.Sum(x => x.Amount);
			var minusRemainingAmount = TotalAmount - AmountPaid;

			if (sumRemainingAmount != minusRemainingAmount)
			{
				throw new InvalidOperationException( "The PaymentsMade does not match the AmountPaid" );
			}
			
			return sumRemainingAmount;
		}

	}
}