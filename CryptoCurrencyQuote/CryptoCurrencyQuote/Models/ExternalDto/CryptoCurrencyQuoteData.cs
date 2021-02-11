using Newtonsoft.Json;
using System.Collections.Generic;

namespace CryptoCurrencyQuote.Models.ExternalDto
{
	public class CryptoCurrencyQuoteData
	{

		[JsonProperty("id")]
		public int Id { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("symbol")]
		public string Symbol { get; set; }
		[JsonProperty("quote")]
		public Dictionary<string, QuoteData> Quotes { get; set; }
	}
}
