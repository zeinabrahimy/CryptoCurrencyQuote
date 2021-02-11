using System.Collections.Generic;

namespace CryptoCurrencyQuote.Models.ExternalDto
{
	public class GetCryptoCurrencyQuotes
	{
		public IEnumerable<string> ConvertCurrencies { get; set; }
		public IEnumerable<int> CryptoCurrencies { get; set; }
	}
}
