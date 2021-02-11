using System.Collections.Generic;

namespace CryptoCurrencyQuote.Models.ExternalDto
{
	public class GetCryptoCurrencies
	{

		[Newtonsoft.Json.JsonProperty("status")]
		public StatusData Status { get; set; }

		[Newtonsoft.Json.JsonProperty("data")]
		public IEnumerable<CryptoCurrencyData> CryptoCurrencies { get; set; }
	}
}
