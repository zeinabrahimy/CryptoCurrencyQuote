namespace CryptoCurrencyQuote.Models.Dto
{
	public class CryptoCurrencyQuoteDto
	{
		public bool Success { get; set; }
		public decimal Price { get; set; }
		public string Message { get; set; }
		public string BaseCurrency { get; set; }
	}
}
