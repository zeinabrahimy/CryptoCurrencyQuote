using System.Collections.Generic;

namespace CryptoCurrencyQuote.Models.Dto
{
	public class GetCryptoCurrencyQuoteResponse
	{
		public bool Success { get; set; }
		public string Message { get; set; }
		public string CryptoCurrencySymbol { get; set; }
		public Dictionary<string, decimal> Quotes { get; set; }
	}
}
