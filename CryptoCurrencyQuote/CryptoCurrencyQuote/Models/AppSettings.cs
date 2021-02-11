using System.Collections.Generic;

namespace CryptoCurrencyQuote.Models
{
	public class AppSettings
	{
		public CoinMarketCapSetting CoinMarketCapSetting { get; set; }
		public string ExchangeRatesApiUrl { get; set; }
		public List<string> Currencies { get; set; }
		public string CalculateQuoteApproach { get; set; }
		public string BaseCurrency { get; set; }
		public int MaximumFloatingPointDigit { get; set; }
	}	
}
