using CryptoCurrencyQuote.Models;
using CryptoCurrencyQuote.Models.ExternalDto;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace CryptoCurrencyQuote.Proxies
{
	public class ExchangeRatesProxy : IExchangeRatesProxy
	{
		private readonly HttpClient _httpClient;
		private readonly ILogger _logger;

		public ExchangeRatesProxy(IHttpClientFactory httpClientFactory,
															IOptions<AppSettings> appsettings,
															ILoggerFactory loggerFactory)
		{
			_httpClient = httpClientFactory.CreateClient();
			_httpClient.BaseAddress = new Uri(appsettings.Value.ExchangeRatesApiUrl);
			_logger = loggerFactory.CreateLogger(GetType());
		}

		#region Implementation Of ExchangeRequest

		public async Task<ExchangeData> GetExchangeRateAsync(GetExchangeData request)
		{
			try
			{
				_logger.LogDebug($"Getting Exchange Rate Started. Request: {request}");

				var response = await _httpClient.GetAsync($"latest?symbols={string.Join(',', request.TargetCurrencies)}&base={request.BaseCurrency}");
				if (!response.IsSuccessStatusCode)
				{
					_logger.LogError($"Getting Exchange Rate Failed. Request: {request}");
					return null;
				}

				var result = JsonConvert.DeserializeObject<ExchangeData>(await response.Content.ReadAsStringAsync());
				_logger.LogDebug($"Getting Exchange Rate Finished. Request: {request} Response: {result}");
				return result;
			}

			catch(Exception ex)
			{
				_logger.LogError(ex, "An Error Has Occureated In Getting Crypto Currencies.");
				return null;
			}
		}


		#endregion
	}
}
