using Newtonsoft.Json;
using System;

namespace CryptoCurrencyQuote.Models.ExternalDto
{
	public class CryptoCurrencyData
	{

		[JsonProperty("id")]
		public int Id { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("symbol")]
		public string Symbol { get; set; }

		[JsonProperty("platslugform")]
		public string Slug { get; set; }

		[JsonProperty("rank")]
		public int Rank { get; set; }

		[JsonProperty("is_active")]
		public bool IsActive { get; set; }

		[JsonProperty("first_historical_data")]
		public DateTime FirstHistoricalData { get; set; }

		[JsonProperty("ast_historical_data")]
		public DateTime AstHistoricalData { get; set; }

		[JsonProperty("platform")]
		public PlatformData Platform { get; set; }
	}
}
