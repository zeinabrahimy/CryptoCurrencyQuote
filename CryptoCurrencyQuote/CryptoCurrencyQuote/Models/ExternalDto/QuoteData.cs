using Newtonsoft.Json;

namespace CryptoCurrencyQuote.Models.ExternalDto
{
	public class QuoteData
	{

		[JsonProperty("price")]
		public decimal Price { get; set; }
	}
}
