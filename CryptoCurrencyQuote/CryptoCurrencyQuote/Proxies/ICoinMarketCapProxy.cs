using CryptoCurrencyQuote.Models.ExternalDto;
using System.Threading.Tasks;

namespace CryptoCurrencyQuote.Proxies
{
	public interface ICoinMarketCapProxy
	{
		Task<CryptoCurrencyQuotesData> GetCryptoCurrencyQuotesAsync(GetCryptoCurrencyQuotes request);
		Task<GetCryptoCurrencies> GetCryptoCurrenciesAsync();
	}
}
