using System;
using Newtonsoft.Json;

namespace CryptoCurrencyQuote.Models.ExternalDto
{
	public class StatusData
	{

		[JsonProperty("timestamp")]
		public DateTime Timestamp { get; set; }

		[JsonProperty("error_code")]
		public int ErrorCode { get; set; }

		[JsonProperty("error_message")]
		public string ErrorMessage { get; set; }

		[JsonProperty("elapsed")]
		public int Elapsed { get; set; }

		[JsonProperty("credit_count")]
		public int CreditCount { get; set; }
	}
}
