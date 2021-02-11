using CryptoCurrencyQuote.Models.ExternalDto;
using System.Threading.Tasks;

namespace CryptoCurrencyQuote.Proxies
{
	public interface IExchangeRatesProxy
	{
		Task<ExchangeData> GetExchangeRateAsync(GetExchangeData request);
	}
}
