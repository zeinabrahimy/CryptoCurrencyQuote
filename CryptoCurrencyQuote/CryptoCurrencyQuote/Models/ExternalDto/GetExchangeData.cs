using System.Collections.Generic;

namespace CryptoCurrencyQuote.Models.ExternalDto
{
	public class GetExchangeData
	{
		public string BaseCurrency { get; set; }
		public IEnumerable<string> TargetCurrencies { get; set; }
	}
}
