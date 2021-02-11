using CryptoCurrencyQuote.Models;
using CryptoCurrencyQuote.Models.Dto;
using CryptoCurrencyQuote.Models.ExternalDto;
using CryptoCurrencyQuote.Models.Entity;
using CryptoCurrencyQuote.Proxies;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoCurrencyQuote.Services
{
	public class CryptoCurrencyService : ICryptoCurrencyService
	{
		private readonly ICoinMarketCapProxy _coinMarketCapProxy;
		private readonly IExchangeRatesProxy _exchangeRatesProxy;
		private readonly ILogger _logger;
		private readonly AppSettings _appSettings;
		private IEnumerable<CryptoCurrency> CryptoCurrencies;
		private int _maximumFloatingPointDigit => _appSettings.MaximumFloatingPointDigit;
		private string _baseCurrency => _appSettings.BaseCurrency;
		private List<string> _currencies => _appSettings.Currencies;

		public CryptoCurrencyService(ILoggerFactory loggerFactory,
																 ICoinMarketCapProxy coinMarketCapProxy,
																 IExchangeRatesProxy exchangeRatesProxy,
																 IOptions<AppSettings> appsettings)
		{
			_logger = loggerFactory.CreateLogger(GetType());
			_coinMarketCapProxy = coinMarketCapProxy;
			_exchangeRatesProxy = exchangeRatesProxy;
			_appSettings = appsettings.Value;
			CryptoCurrencies = null;
		}

		#region Implementation Of ICryptoCurrencyService

		public async Task<GetCryptoCurrencyQuoteResponse> GetCryptoCurrencyQuoteAsync(GetCryptoCurrencyQuoteRequest request)
		{
			_logger.LogInformation($"Getting Crypto Currency Quote Started. Request: {request}");
			if (request == null)
			{
				return new GetCryptoCurrencyQuoteResponse
				{
					CryptoCurrencySymbol = string.Empty,
					Quotes = null,
					Success = false,
					Message = "Request Is Null"
				};
			}
			if (string.IsNullOrWhiteSpace(request.CryptoCurrencySymbol))
			{
				return new GetCryptoCurrencyQuoteResponse
				{
					CryptoCurrencySymbol = string.Empty,
					Quotes = null,
					Success = false,
					Message = "CryptoCurrencySymbol Is Null Or Empty"
				};
			}

			await SetCryptoCurrencies().ConfigureAwait(false);

			var cryptoCurrencySymbolMinLength = CryptoCurrencies.Min(x => x.Symbol.Length);
			var cryptoCurrencySymbolMaxLength = CryptoCurrencies.Max(x => x.Symbol.Length);
			if (request.CryptoCurrencySymbol.Trim().Length < cryptoCurrencySymbolMinLength || request.CryptoCurrencySymbol.Trim().Length > cryptoCurrencySymbolMaxLength)
			{
				return new GetCryptoCurrencyQuoteResponse
				{
					CryptoCurrencySymbol = request.CryptoCurrencySymbol,
					Quotes = null,
					Success = false,
					Message = $"CryptoCurrencySymbol Lentgh Is Invalid. Should Be Between {cryptoCurrencySymbolMinLength} And {cryptoCurrencySymbolMaxLength}"
				};
			}

			var cryptoCurrency = CryptoCurrencies.FirstOrDefault(x => x.Symbol.ToLower() == request.CryptoCurrencySymbol.ToLower());

			if (cryptoCurrency == null)
			{
				return new GetCryptoCurrencyQuoteResponse
				{
					Success = false,
					Message = "Invalid Crypto Currency Symbol.",
					CryptoCurrencySymbol = request.CryptoCurrencySymbol,
					Quotes = null
				};
			}

			if (!cryptoCurrency.IsActive)
			{
				return new GetCryptoCurrencyQuoteResponse
				{
					Success = false,
					Message = "Requested Crypto Currency Is Not Active.",
					CryptoCurrencySymbol = string.Empty,
					Quotes = null
				};
			}

			if (IsCryptoCurrencyQuoteOnlyCallCoinMarketCapApproach())
			{
				return await CryptoCurrencyQuoteOnlyCallCoinMarketCapApproach(request.CryptoCurrencySymbol, cryptoCurrency.Id).ConfigureAwait(false);
			}

			return await CryptoCurrencyQuoteWithExchangeApproach(request.CryptoCurrencySymbol, cryptoCurrency.Id).ConfigureAwait(false);
		}

		public async Task<IEnumerable<CryptoCurrency>> GetCryptoCurrenciesAsync()
		{
			await SetCryptoCurrencies().ConfigureAwait(false);
			return CryptoCurrencies.Where(x => x.IsActive);
		}

		#endregion

		#region Private Methods

		private async Task SetCryptoCurrencies()
		{
			if (CryptoCurrencies == null)
			{
				_logger.LogInformation($"Getting Crypto Currencies Started.");

				var response = await _coinMarketCapProxy.GetCryptoCurrenciesAsync().ConfigureAwait(false);

				if (response != null && response.Status != null && response.Status.ErrorCode > 0)
				{
					_logger.LogError($"An Error Has Occured {response.Status.ErrorCode } -- {response.Status.ErrorMessage}");
				}

				bool crypotCurrenciesExist = response != null && response.CryptoCurrencies != null;

				_logger.LogInformation($"There Are/Is { (crypotCurrenciesExist ? response.CryptoCurrencies.Count() : '0')} Crypto Currency/ies");

				if (crypotCurrenciesExist)
				{
					CryptoCurrencies = new List<CryptoCurrency>();
					CryptoCurrencies = response.CryptoCurrencies.Select(x => new CryptoCurrency
					{
						Id = x.Id,
						Name = x.Name,
						Symbol = x.Symbol,
						IsActive = x.IsActive
					});
				}

				_logger.LogInformation($"Getting Crypto Currencies Finished.");
			}
		}
		
		private async Task<GetCryptoCurrencyQuoteResponse> CryptoCurrencyQuoteWithExchangeApproach(string cryptoCurrencySymbol, int cryptoCurrencyId)
		{
			var result = new GetCryptoCurrencyQuoteResponse();
			var cryptoCurrencyQuote = await GetCryptoCurrencyQuote(cryptoCurrencyId, _baseCurrency).ConfigureAwait(false);

			if (!cryptoCurrencyQuote.Success)
			{
				result.Message = cryptoCurrencyQuote.Message;
				return result;
			}

			var getExchangeData = new GetExchangeData
			{
				BaseCurrency = _baseCurrency.ToString(),
				TargetCurrencies = _currencies
			};

			var exchangeRates = await _exchangeRatesProxy.GetExchangeRateAsync(getExchangeData).ConfigureAwait(false);

			if (exchangeRates == null || exchangeRates.Rates == null)
			{
				result.Message = "Can Not Exchange Base Currency Rate ";
				return result;
			}

			result.Quotes = _currencies.ToDictionary(a => a, t => t == _baseCurrency ?
																																 decimal.Round(cryptoCurrencyQuote.Price, _maximumFloatingPointDigit):
																																 GetPriceInTargetCurrency(t, cryptoCurrencyQuote.Price, exchangeRates));

			result.Success = true;
			result.CryptoCurrencySymbol = cryptoCurrencySymbol;
			result.Message = "Success";
			return result;
		}

		private decimal GetPriceInTargetCurrency(string targetCurrency, decimal price, ExchangeData baseCurrencyExchangeRate) =>
						decimal.Round(baseCurrencyExchangeRate.Rates[targetCurrency] * price, _maximumFloatingPointDigit);

		private async Task<GetCryptoCurrencyQuoteResponse> CryptoCurrencyQuoteOnlyCallCoinMarketCapApproach(string cryptoCurrencySymbol, int cryptoCurrencyId)
		{
			var cryptoCurrencyPriceTasks = new List<Task<CryptoCurrencyQuoteDto>>();
			_currencies.ForEach(targetCurrency =>
			{
				cryptoCurrencyPriceTasks.Add(GetCryptoCurrencyQuote(cryptoCurrencyId, targetCurrency));
			});

			await Task.WhenAll(cryptoCurrencyPriceTasks).ConfigureAwait(false);

			var result = new GetCryptoCurrencyQuoteResponse()
			{
				Success = true,
				CryptoCurrencySymbol = cryptoCurrencySymbol,
				Quotes = new Dictionary<string, decimal>(),
				Message = "Success"
			};

			foreach (var task in cryptoCurrencyPriceTasks)
			{
				var cryptoCurrencyQuote = await task;
				result.Quotes.Add(cryptoCurrencyQuote.BaseCurrency, cryptoCurrencyQuote.Success ? decimal.Round(cryptoCurrencyQuote.Price, _maximumFloatingPointDigit) : 0);
			}

			return result;

		}

		private async Task<CryptoCurrencyQuoteDto> GetCryptoCurrencyQuote(int cryptoCurrencyId, string baseCurrency)
		{
			var cryptoCurrencyQuotes = await _coinMarketCapProxy.GetCryptoCurrencyQuotesAsync(new GetCryptoCurrencyQuotes
			{
				CryptoCurrencies = new List<int> { cryptoCurrencyId },
				ConvertCurrencies = new List<string> { baseCurrency }
			}).ConfigureAwait(false);

			if (cryptoCurrencyQuotes == null || cryptoCurrencyQuotes.Data == null)
			{
				_logger.LogError($"Can Not Get Crypto Currency Quotes. cryptoCurrencyId: {cryptoCurrencyId} - baseCurrency: {baseCurrency} ");

				return new CryptoCurrencyQuoteDto
				{
					BaseCurrency = baseCurrency,
					Message = "Can Not Get Crypto Currency Quotes.",
					Price = 0,
					Success = false
				};
			}

			var requestedCryptoCurrencyData = cryptoCurrencyQuotes.Data.FirstOrDefault(x => x.Value.Id == cryptoCurrencyId).Value;

			if (requestedCryptoCurrencyData == null || requestedCryptoCurrencyData.Quotes == null || requestedCryptoCurrencyData.Quotes.Count() == 0)
			{
				_logger.LogError($"Can Not Get Crypto Currency Quotes For Requested CryptoCurrencyId. cryptoCurrencyId: {cryptoCurrencyId} - baseCurrency: {baseCurrency}");
				return new CryptoCurrencyQuoteDto
				{
					BaseCurrency = baseCurrency,
					Message = "Can Not Get Crypto Currency Quotes For Requested CryptoCurrencyId.",
					Price = 0,
					Success = false
				};
			}

			var quotes = requestedCryptoCurrencyData.Quotes.Where(x => x.Key.ToLower() == baseCurrency.ToString().ToLower());

			if (!quotes.Any() || quotes.Count() > 1)
			{
				_logger.LogError($"Can Not Get Crypto Currency Quotes For Requested Base Currency. cryptoCurrencyId: {cryptoCurrencyId} - baseCurrency: {baseCurrency}");
				return new CryptoCurrencyQuoteDto
				{
					BaseCurrency = baseCurrency,
					Message = "Can Not Get Crypto Currency Quotes For Requested Base Currency.",
					Price = 0,
					Success = false
				};
			}

			return new CryptoCurrencyQuoteDto
			{
				Success = true,
				Price = quotes.Single().Value.Price,
				Message = "Success",
				BaseCurrency = baseCurrency
			};
		}

		private bool IsCryptoCurrencyQuoteOnlyCallCoinMarketCapApproach() => _appSettings.CalculateQuoteApproach.ToLower() == Constants.CryptoCurrencyQuoteOnlyCallCoinMarketCapApproach;

		#endregion
	}
}
