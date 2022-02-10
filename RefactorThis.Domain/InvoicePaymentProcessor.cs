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

		public string ProcessPayment( Payment payment )
		{
			var inv = _invoiceRepository.GetInvoice( payment.Reference );

			var responseMessage = string.Empty;

			if ( inv == null )
			{
				throw new InvalidOperationException( "There is no invoice matching this payment" );
			}
			
			if ( inv.AmountRemaining == 0 ) 
			{
					if ( inv.PaymentsMade == null || !inv.PaymentsMade.Any( ) )
					{
						responseMessage = "no payment needed";
					}
					else
					{
						throw new InvalidOperationException( "The invoice is in an invalid state, it has an amount of 0 and it has payments." );
					}
			}
			else
			{
				if ( inv.PaymentsMade != null && inv.PaymentsMade.Any( ) )
				{
					if ( inv.PaymentsMade.Sum( x => x.Amount ) != 0 && inv.AmountRemaining == inv.PaymentsMade.Sum( x => x.Amount ) )
					{
						responseMessage = "invoice was already fully paid";
					}
					else if ( inv.PaymentsMade.Sum( x => x.Amount ) != 0 && payment.Amount > ( inv.AmountRemaining - inv.AmountPaid ) )
					{
						responseMessage = "the payment is greater than the partial amount remaining";
					}
					else
					{
						if ( ( inv.AmountRemaining - inv.AmountPaid ) == payment.Amount )
						{
							inv.AmountPaid += payment.Amount;
							inv.PaymentsMade.Add( payment );
							responseMessage = "final partial payment received, invoice is now fully paid";
						}
						else
						{
							inv.AmountPaid += payment.Amount;
							inv.PaymentsMade.Add( payment );
							responseMessage = "another partial payment received, still not fully paid";
						}
					}
				}
				else
				{
					if ( payment.Amount > inv.AmountRemaining )
					{
						responseMessage = "the payment is greater than the invoice amount";
					}
					else if ( inv.AmountRemaining == payment.Amount )
					{
						inv.AmountPaid = payment.Amount;
						inv.PaymentsMade.Add( payment );
						responseMessage = "invoice is now fully paid";
					}
					else
					{
						inv.AmountPaid = payment.Amount;
						inv.PaymentsMade.Add( payment );
						responseMessage = "invoice is now partially paid";
					}
				}
			}
		
			
			inv.Save();

			return responseMessage;
		}
	}
}