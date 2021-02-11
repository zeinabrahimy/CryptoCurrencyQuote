using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace CryptoCurrencyQuote.Models.ExternalDto
{
	public class ExchangeData
	{

		[JsonProperty("rates")]
		public Dictionary<string, decimal> Rates { get; set; }

		[JsonProperty("base")]
		public string Base { get; set; }

		[JsonProperty("date")]
		public DateTime Date { get; set; }
	}
}
