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
	public class CoinMarketCapProxy : ICoinMarketCapProxy
	{
		private readonly HttpClient _httpClient;
		private readonly ILogger _logger;

		public CoinMarketCapProxy(IHttpClientFactory httpClientFactory,
															IOptions<AppSettings> appsettings,
															ILoggerFactory loggerFactory)
		{
			_httpClient = httpClientFactory.CreateClient();
			_httpClient.BaseAddress = new Uri(appsettings.Value.CoinMarketCapSetting.ApiUrl);
			_httpClient.DefaultRequestHeaders.Add("X-CMC_PRO_API_KEY", appsettings.Value.CoinMarketCapSetting.ApiKey);
			_httpClient.DefaultRequestHeaders.Add("Accepts", "application/json");
			_logger = loggerFactory.CreateLogger(GetType());
		}

		#region Impelmentation Of ICoinMarketCapProxy 

		public async Task<CryptoCurrencyQuotesData> GetCryptoCurrencyQuotesAsync(GetCryptoCurrencyQuotes request)
		{
			try
			{
				_logger.LogDebug($"Getting Crypto Currency Quotes. Request: {request}");

				var response = await _httpClient.GetAsync($"cryptocurrency/quotes/latest?id={string.Join(',', request.CryptoCurrencies)}&convert={string.Join(',', request.ConvertCurrencies)}");
				if (!response.IsSuccessStatusCode)
				{
					_logger.LogError($"Getting Crypto Currency Quotes Failed. Request: {request}");
					return null;
				}

				var result = JsonConvert.DeserializeObject<CryptoCurrencyQuotesData>(await response.Content.ReadAsStringAsync());

				_logger.LogDebug($"Getting Crypto Currency Quotes Finished. Request: {request} Response: {result}");

				return result;

			}
			catch(Exception ex)
			{
				_logger.LogError(ex, "An Error Has Occureated In Getting Crypto Currencies");

				return null;
			}
		}

		public async Task<GetCryptoCurrencies> GetCryptoCurrenciesAsync()
		{
			try
			{
				_logger.LogDebug($"Getting Crypto Currencies Startes.");

				var response = await _httpClient.GetAsync("cryptocurrency/map");
				if (!response.IsSuccessStatusCode)
				{
					_logger.LogError($"Getting Crypto Currencies Failed.");
					return null;
				}

				var result = JsonConvert.DeserializeObject<GetCryptoCurrencies>(await response.Content.ReadAsStringAsync());
				_logger.LogDebug($"Getting Crypto Currencies Finished. Response: {result}");
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
