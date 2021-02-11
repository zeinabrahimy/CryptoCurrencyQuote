using CryptoCurrencyQuote.Models.Entity;
using CryptoCurrencyQuote.Models.Dto;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace CryptoCurrencyQuote.Services
{
	public interface ICryptoCurrencyService
	{
		Task<IEnumerable<CryptoCurrency>> GetCryptoCurrenciesAsync();
		Task<GetCryptoCurrencyQuoteResponse> GetCryptoCurrencyQuoteAsync(GetCryptoCurrencyQuoteRequest request);
	}
}
