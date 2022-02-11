using System;
using System.Linq;
using RefactorThis.Persistence;

namespace RefactorThis.Domain
{
	public class InvoicePaymentProcessor
	{
		private readonly InvoiceRepository _invoiceRepository;

		public InvoicePaymentProcessor( InvoiceRepository invoiceRepository )
		{
			_invoiceRepository = invoiceRepository;
		}

		private void SafetyChecks ( Invoice invoice, Payment payment )
		{
			if ( invoice == null )
			{
				throw new InvalidOperationException( "There is no invoice matching this payment" );
			}
			
			if ( invoice.TotalAmount < 0 )
			{
				throw new InvalidOperationException( "The invoice is in an invalid state, it has an amount less than 0." );
			}

			if ( payment == null )
			{
				throw new InvalidOperationException( "Critical error, payment is null" );
			}
			
			if ( payment.Amount <= 0 )
			{
				throw new InvalidOperationException( "Critical error, payment must have a value" );
			}
		}

		private string ReceivePayment(Payment payment, Invoice invoice )
		{
			var responseMessage = "";

			if (invoice.RemainingBalance() == 0)
			{
				return "invoice was already fully paid";
			}
			
			if ( invoice.RemainingBalance() < payment.Amount )
			{
				return "the payment is greater than the invoice amount, you owe $" + invoice.RemainingBalance();
			}

			var remainingAmount = invoice.AddPayment(payment);

			if ( remainingAmount == 0 )
			{
				responseMessage = "invoice is now fully paid";
			}
			else
			{
				responseMessage = "invoice has been paid. There is still $" + remainingAmount + " owing";
			}
			
			return responseMessage;
		}

		public string ProcessPayment( Payment payment )
		{
			var inv = _invoiceRepository.GetInvoice( payment.Reference );

			SafetyChecks(inv, payment);
			
			//Check if $0 invoice has received payments.
			if ( inv.TotalAmount == 0 ) 
			{
					if ( !inv.PaymentsMade.Any( ) )
					{
						return "no payment needed";
					}
					else
					{
						throw new InvalidOperationException( "The invoice is in an invalid state, it has an amount of 0 and it has payments." );
					}
			}

			var responseMessage = ReceivePayment(payment, inv);
			
			inv.Save();

			return responseMessage;
		}
	}
}