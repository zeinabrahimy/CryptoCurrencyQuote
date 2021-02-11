using Newtonsoft.Json;
using System.Collections.Generic;


namespace CryptoCurrencyQuote.Models.ExternalDto
{
	public class CryptoCurrencyQuotesData
	{
		[JsonProperty("status")]
		public StatusData Status { get; set; }
		
		[JsonProperty("data")]
		public Dictionary<string, CryptoCurrencyQuoteData> Data { get; set; }
	}
}
