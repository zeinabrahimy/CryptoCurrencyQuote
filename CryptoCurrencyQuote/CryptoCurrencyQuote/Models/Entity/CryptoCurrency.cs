namespace CryptoCurrencyQuote.Models.Entity
{
	public class CryptoCurrency
	{
		public int Id { get; set; }
		public bool IsActive { get; set; }
		public string Name { get; set; }
		public string Symbol { get; set; }
	}
}
