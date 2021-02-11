using CryptoCurrencyQuote.Models;
using CryptoCurrencyQuote.Models.Dto;
using CryptoCurrencyQuote.Models.ExternalDto;
using CryptoCurrencyQuote.Proxies;
using CryptoCurrencyQuote.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CryptoCurrencyQuote.UnitTest.Services
{
	public class CryptoCurrencyServiceTest
	{
		private ICryptoCurrencyService _cryptoCurrencyService;
		private Mock<ICoinMarketCapProxy> _coinMarketCapProxy;
		private Mock<IExchangeRatesProxy> _exchangeRatesProxy;
		private Mock<ILogger> _logger;
		private AppSettings _appSettings;

		private void Initialize()
		{
			var loggerFactory = new Mock<ILoggerFactory>();
			_logger = new Mock<ILogger>();
			_logger.Setup(x => x.Log(It.IsAny<LogLevel>(), It.IsAny<EventId>(), It.Is<It.IsAnyType>((v, t) => true), It.IsAny<Exception>(), It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true))).Verifiable();
			loggerFactory.Setup(x => x.CreateLogger(It.IsAny<string>())).Returns(_logger.Object);

			_coinMarketCapProxy = new Mock<ICoinMarketCapProxy>();
			_exchangeRatesProxy = new Mock<IExchangeRatesProxy>();
			_appSettings = new AppSettings();

			_cryptoCurrencyService = new CryptoCurrencyService(loggerFactory.Object, _coinMarketCapProxy.Object, _exchangeRatesProxy.Object, Options.Create(_appSettings));
		}

		[Fact]
		public async Task GetCryptoCurrenciesAsync_WithoutActiveCryptoCurrency_ReturnListIsEmpty()
		{
			// Arrange
			Initialize();

			var getCryptoCurrencies = new GetCryptoCurrencies
			{
				Status = new StatusData
				{
					ErrorCode = 0
				},
				CryptoCurrencies = new List<CryptoCurrencyData>
				{
					new CryptoCurrencyData
					{
						Id = 1,
						IsActive = false,
						Name = "Bitcoin"
					}
				}
			};
			_coinMarketCapProxy.Setup(x => x.GetCryptoCurrenciesAsync()).ReturnsAsync(getCryptoCurrencies);

			// Act
			var result = await _cryptoCurrencyService.GetCryptoCurrenciesAsync();

			// Assert
			Assert.NotNull(result);
			Assert.True(result.Count() == 0);
			_coinMarketCapProxy.Verify(x => x.GetCryptoCurrenciesAsync(), Times.Once);
		}

		[Fact]
		public async Task GetCryptoCurrenciesAsync_WithActiveCryptoCurrency_ReturnListContainsActiveItems()
		{
			// Arrange
			Initialize();

			var getCryptoCurrencies = new GetCryptoCurrencies
			{
				Status = new StatusData
				{
					ErrorCode = 0
				},
				CryptoCurrencies = new List<CryptoCurrencyData>
				{
					new CryptoCurrencyData
					{
						Id = 1,
						IsActive = false,
						Name = "Bitcoin"
					},
					new CryptoCurrencyData
					{
						Id = 3,
						Name = "Namecoin",
						IsActive = true
					},
					new CryptoCurrencyData
					{
						Id = 4,
            Name ="Terracoin",
						IsActive = true
					}
				}
			};
			_coinMarketCapProxy.Setup(x => x.GetCryptoCurrenciesAsync()).ReturnsAsync(getCryptoCurrencies);

			// Act
			var result = await _cryptoCurrencyService.GetCryptoCurrenciesAsync();

			// Assert
			Assert.NotNull(result);
			Assert.True(result.Count() == 2);

			var namecoinCryptoCurrency = result.FirstOrDefault(x => x.Name == "Namecoin");
			Assert.NotNull(namecoinCryptoCurrency);
			Assert.Equal(3, namecoinCryptoCurrency.Id);
			Assert.True(namecoinCryptoCurrency.IsActive);

			var terracoinCryptoCurrency = result.FirstOrDefault(x => x.Name == "Terracoin");
			Assert.NotNull(terracoinCryptoCurrency);
			Assert.Equal(4, terracoinCryptoCurrency.Id);
			Assert.True(terracoinCryptoCurrency.IsActive);

			_coinMarketCapProxy.Verify(x => x.GetCryptoCurrenciesAsync(), Times.Once);
		}

		[Theory]
		[MemberData(nameof(GetInvalidRequest))]
		public async Task GetCryptoCurrencyQuoteAsync_InvalidRequest_ResultIsNotSuccess(GetCryptoCurrencyQuoteRequest request, string message)
		{
			// Arrange
			Initialize();

			// Act
			var result = await _cryptoCurrencyService.GetCryptoCurrencyQuoteAsync(request).ConfigureAwait(false);

			// Assert
			Assert.NotNull(result);
			Assert.False(result.Success);
			Assert.Equal(message, result.Message);
			Assert.Null(result.Quotes);
			Assert.Equal(string.Empty, result.CryptoCurrencySymbol);

			_coinMarketCapProxy.Verify(x => x.GetCryptoCurrenciesAsync(), Times.Never);
		}

		[Fact]
		public async Task GetCryptoCurrencyQuoteAsync_InvalidCryptoCurrency_ReturnMessageIsInvalidCryptoCurrencySymbol()
		{
			// Arrange
			Initialize();

			var request = new GetCryptoCurrencyQuoteRequest
			{
				CryptoCurrencySymbol = "BTN"
			};

			GetCryptoCurrencies getCryptoCurrenciesResponse = new GetCryptoCurrencies
			{
				CryptoCurrencies = new List<CryptoCurrencyData>
				{
					new CryptoCurrencyData
					{
						Id = 10,
						Name = "Lala",
						Symbol = "LLL",
						IsActive = true
					}
				},
				Status = new StatusData
				{
					ErrorCode = 0
				}
			};
			_coinMarketCapProxy.Setup(x => x.GetCryptoCurrenciesAsync()).ReturnsAsync(getCryptoCurrenciesResponse);

			// Act
			var result = await _cryptoCurrencyService.GetCryptoCurrencyQuoteAsync(request).ConfigureAwait(false);

			// Assert
			Assert.NotNull(result);
			Assert.False(result.Success);
			Assert.Equal("Invalid Crypto Currency Symbol.", result.Message);
			Assert.Null(result.Quotes);
			Assert.Equal(request.CryptoCurrencySymbol, result.CryptoCurrencySymbol);

			_coinMarketCapProxy.Verify(x => x.GetCryptoCurrenciesAsync(), Times.Once);
		}

		[Fact]
		public async Task GetCryptoCurrencyQuoteAsync_InactiveCryptoCurrency_ReturnRequestedCryptoCurrencyIsNotActiveInMessage()
		{
			// Arrange
			Initialize();

			var request = new GetCryptoCurrencyQuoteRequest
			{
				CryptoCurrencySymbol = "BTN"
			};

			GetCryptoCurrencies getCryptoCurrenciesResponse = new GetCryptoCurrencies
			{
				CryptoCurrencies = new List<CryptoCurrencyData>
				{
					new CryptoCurrencyData
					{
						Id = 1,
						Name = "Bitcoin",
						Symbol = "BTN",
						IsActive = false
					}
				},
				Status = new StatusData
				{
					ErrorCode = 0
				}
			};
			_coinMarketCapProxy.Setup(x => x.GetCryptoCurrenciesAsync()).ReturnsAsync(getCryptoCurrenciesResponse);

			// Act
			var result = await _cryptoCurrencyService.GetCryptoCurrencyQuoteAsync(request).ConfigureAwait(false);

			// Assert
			Assert.NotNull(result);
			Assert.False(result.Success);
			Assert.Equal("Requested Crypto Currency Is Not Active.", result.Message);
			Assert.Null(result.Quotes);
			Assert.Equal(string.Empty, result.CryptoCurrencySymbol);

			_coinMarketCapProxy.Verify(x => x.GetCryptoCurrenciesAsync(), Times.Once);
		}

		[Fact]
		public async Task GetCryptoCurrencyQuoteAsync_CryptoCurrencyQuoteOnlyCallCoinMarketCapApproach_VerifyCoinMarketCapProxyMethodsVerifyExchangeRatesProxyNever()
		{
			// Arrange
			Initialize();
			_appSettings.CalculateQuoteApproach = Constants.CryptoCurrencyQuoteOnlyCallCoinMarketCapApproach;
			_appSettings.Currencies = new List<string> { "USD", "EUR" };
			_appSettings.BaseCurrency = "USD";
			_appSettings.MaximumFloatingPointDigit = 3;

			string cryptoCurrencySymbol = "BTN";
			int cryptoCurrencyId = 1;

			var request = new GetCryptoCurrencyQuoteRequest
			{
				CryptoCurrencySymbol = cryptoCurrencySymbol
			};

			GetCryptoCurrencies getCryptoCurrenciesResponse = new GetCryptoCurrencies
			{
				CryptoCurrencies = new List<CryptoCurrencyData>
				{
					new CryptoCurrencyData
					{
						Id = cryptoCurrencyId,
						Name = "Bitcoin",
						Symbol = cryptoCurrencySymbol,
						IsActive = true
					}
				},
				Status = new StatusData
				{
					ErrorCode = 0
				}
			};
			_coinMarketCapProxy.Setup(x => x.GetCryptoCurrenciesAsync()).ReturnsAsync(getCryptoCurrenciesResponse);

			GetCryptoCurrencyQuotes getCryptoCurrencyQuotesRequestForUSD = null;
			decimal usdPrice = 100000;
			CryptoCurrencyQuotesData getCryptoCurrencyQuotesResponseForUSD = new CryptoCurrencyQuotesData
			{
				Data = new Dictionary<string, CryptoCurrencyQuoteData> { { cryptoCurrencyId.ToString(), new CryptoCurrencyQuoteData { Id = cryptoCurrencyId, Quotes = new Dictionary<string, QuoteData> { { "USD", new QuoteData { Price = usdPrice } } } } } }
			};
			_coinMarketCapProxy.Setup(x => x.GetCryptoCurrencyQuotesAsync(It.Is<GetCryptoCurrencyQuotes>(c => c.ConvertCurrencies.Contains("USD"))))
												 .Callback<GetCryptoCurrencyQuotes>(x => getCryptoCurrencyQuotesRequestForUSD = x)
												 .ReturnsAsync(getCryptoCurrencyQuotesResponseForUSD);

			GetCryptoCurrencyQuotes getCryptoCurrencyQuotesRequestForEUR = null;
			decimal eurPrice = 1000.123456m;
			CryptoCurrencyQuotesData getCryptoCurrencyQuotesResponseForEUR = new CryptoCurrencyQuotesData
			{
				Data = new Dictionary<string, CryptoCurrencyQuoteData> { { cryptoCurrencyId.ToString(), new CryptoCurrencyQuoteData { Id = cryptoCurrencyId, Quotes = new Dictionary<string, QuoteData> { { "EUR", new QuoteData { Price = eurPrice } } } } } }
			};
			_coinMarketCapProxy.Setup(x => x.GetCryptoCurrencyQuotesAsync(It.Is<GetCryptoCurrencyQuotes>(c => c.ConvertCurrencies.Contains("EUR"))))
												 .Callback<GetCryptoCurrencyQuotes>(x => getCryptoCurrencyQuotesRequestForEUR = x)
												 .ReturnsAsync(getCryptoCurrencyQuotesResponseForEUR);

			// Act
			var result = await _cryptoCurrencyService.GetCryptoCurrencyQuoteAsync(request).ConfigureAwait(false);

			// Assert
			Assert.NotNull(result);
			Assert.True(result.Success);
			Assert.Equal("Success", result.Message);
			Assert.NotNull(result.Quotes);
			Assert.Equal(cryptoCurrencySymbol, result.CryptoCurrencySymbol);

			Assert.True(result.Quotes.Count() == 2);
			var usdQuote = result.Quotes.FirstOrDefault(x => x.Key == "USD");
			Assert.NotNull(usdQuote);
			Assert.Equal(usdPrice, usdQuote.Value);
			var eurQuote = result.Quotes.FirstOrDefault(x => x.Key == "EUR");
			Assert.NotNull(eurQuote);
			Assert.Equal(decimal.Round(eurPrice, _appSettings.MaximumFloatingPointDigit), eurQuote.Value);


			_coinMarketCapProxy.Verify(x => x.GetCryptoCurrenciesAsync(), Times.Once);
			_exchangeRatesProxy.Verify(x => x.GetExchangeRateAsync(It.IsAny<GetExchangeData>()), Times.Never);
			_coinMarketCapProxy.Verify(x => x.GetCryptoCurrencyQuotesAsync(It.Is<GetCryptoCurrencyQuotes>(c => c.ConvertCurrencies.Contains("USD"))), Times.Once);

			Assert.NotNull(getCryptoCurrencyQuotesRequestForUSD);
			Assert.NotNull(getCryptoCurrencyQuotesRequestForUSD.CryptoCurrencies);
			Assert.True(getCryptoCurrencyQuotesRequestForUSD.CryptoCurrencies.Count() == 1);
			Assert.Contains(cryptoCurrencyId, getCryptoCurrencyQuotesRequestForUSD.CryptoCurrencies);

			Assert.NotNull(getCryptoCurrencyQuotesRequestForUSD.ConvertCurrencies);
			Assert.True(getCryptoCurrencyQuotesRequestForUSD.ConvertCurrencies.Count() == 1);
			Assert.Contains("USD", getCryptoCurrencyQuotesRequestForUSD.ConvertCurrencies);

			_coinMarketCapProxy.Verify(x => x.GetCryptoCurrencyQuotesAsync(It.Is<GetCryptoCurrencyQuotes>(c => c.ConvertCurrencies.Contains("EUR"))), Times.Once);

			Assert.NotNull(getCryptoCurrencyQuotesRequestForEUR);
			Assert.NotNull(getCryptoCurrencyQuotesRequestForEUR.CryptoCurrencies);
			Assert.True(getCryptoCurrencyQuotesRequestForEUR.CryptoCurrencies.Count() == 1);
			Assert.Contains(cryptoCurrencyId, getCryptoCurrencyQuotesRequestForEUR.CryptoCurrencies);

			Assert.NotNull(getCryptoCurrencyQuotesRequestForEUR.ConvertCurrencies);
			Assert.True(getCryptoCurrencyQuotesRequestForEUR.ConvertCurrencies.Count() == 1);
			Assert.Contains("EUR", getCryptoCurrencyQuotesRequestForEUR.ConvertCurrencies);

		}

		[Fact]
		public async Task GetCryptoCurrencyQuoteAsync_CryptoCurrencyQuoteWithExchangeApproach_VerifyCoinMarketCapProxyMethodsVerifyExchangeRatesProxyOnce()
		{
			// Arrange
			Initialize();
			_appSettings.CalculateQuoteApproach = "CryptoCurrencyQuoteWithExchangeApproach";
			_appSettings.Currencies = new List<string> { "USD", "EUR" };
			_appSettings.BaseCurrency = "USD";
			_appSettings.MaximumFloatingPointDigit = 3;

			string cryptoCurrencySymbol = "BTN";
			int cryptoCurrencyId = 1;

			var request = new GetCryptoCurrencyQuoteRequest
			{
				CryptoCurrencySymbol = cryptoCurrencySymbol
			};

			GetCryptoCurrencies getCryptoCurrenciesResponse = new GetCryptoCurrencies
			{
				CryptoCurrencies = new List<CryptoCurrencyData>
				{
					new CryptoCurrencyData
					{
						Id = cryptoCurrencyId,
						Name = "Bitcoin",
						Symbol = cryptoCurrencySymbol,
						IsActive = true
					}
				},
				Status = new StatusData
				{
					ErrorCode = 0
				}
			};
			_coinMarketCapProxy.Setup(x => x.GetCryptoCurrenciesAsync()).ReturnsAsync(getCryptoCurrenciesResponse);

			GetCryptoCurrencyQuotes getCryptoCurrencyQuotesRequestForUSD = null;
			decimal usdPrice = 100000;
			CryptoCurrencyQuotesData getCryptoCurrencyQuotesResponseForUSD = new CryptoCurrencyQuotesData
			{
				Data = new Dictionary<string, CryptoCurrencyQuoteData> { { cryptoCurrencyId.ToString(), new CryptoCurrencyQuoteData { Id = cryptoCurrencyId, Quotes = new Dictionary<string, QuoteData> { { "USD", new QuoteData { Price = usdPrice } } } } } }
			};
			_coinMarketCapProxy.Setup(x => x.GetCryptoCurrencyQuotesAsync(It.IsAny<GetCryptoCurrencyQuotes>()))
												 .Callback<GetCryptoCurrencyQuotes>(x => getCryptoCurrencyQuotesRequestForUSD = x)
												 .ReturnsAsync(getCryptoCurrencyQuotesResponseForUSD);


			decimal eurRateToUsd = 0.1m;
			GetExchangeData exchangeRequest = null;
			ExchangeData exchangeResponse = new ExchangeData
			{
				Base = _appSettings.BaseCurrency,
				Rates = new Dictionary<string, decimal> { { "EUR", eurRateToUsd } }
			};
			_exchangeRatesProxy.Setup(x => x.GetExchangeRateAsync(It.IsAny<GetExchangeData>())).Callback<GetExchangeData>(x => exchangeRequest = x).ReturnsAsync(exchangeResponse);


			// Act
			var result = await _cryptoCurrencyService.GetCryptoCurrencyQuoteAsync(request).ConfigureAwait(false);

			// Assert
			Assert.NotNull(result);
			Assert.True(result.Success);
			Assert.Equal("Success", result.Message);
			Assert.NotNull(result.Quotes);
			Assert.Equal(cryptoCurrencySymbol, result.CryptoCurrencySymbol);

			Assert.True(result.Quotes.Count() == 2);
			var usdQuote = result.Quotes.FirstOrDefault(x => x.Key == "USD");
			Assert.NotNull(usdQuote);
			Assert.Equal(usdPrice, usdQuote.Value);
			var eurQuote = result.Quotes.FirstOrDefault(x => x.Key == "EUR");
			Assert.NotNull(eurQuote);
			Assert.Equal(decimal.Round(eurRateToUsd * usdPrice, _appSettings.MaximumFloatingPointDigit), eurQuote.Value);


			_coinMarketCapProxy.Verify(x => x.GetCryptoCurrenciesAsync(), Times.Once);
			_exchangeRatesProxy.Verify(x => x.GetExchangeRateAsync(It.IsAny<GetExchangeData>()), Times.Once);
			Assert.NotNull(exchangeRequest);
			Assert.Equal(_appSettings.BaseCurrency, exchangeRequest.BaseCurrency);
			Assert.NotNull(exchangeRequest.TargetCurrencies);
			Assert.True(_appSettings.Currencies.Count() == exchangeRequest.TargetCurrencies.Count());

			foreach (var currency in _appSettings.Currencies)
			{
				Assert.Contains(currency, exchangeRequest.TargetCurrencies);
			}

			_coinMarketCapProxy.Verify(x => x.GetCryptoCurrencyQuotesAsync(It.IsAny<GetCryptoCurrencyQuotes>()), Times.Once);

			Assert.NotNull(getCryptoCurrencyQuotesRequestForUSD);
			Assert.NotNull(getCryptoCurrencyQuotesRequestForUSD.CryptoCurrencies);
			Assert.True(getCryptoCurrencyQuotesRequestForUSD.CryptoCurrencies.Count() == 1);
			Assert.Contains(cryptoCurrencyId, getCryptoCurrencyQuotesRequestForUSD.CryptoCurrencies);

			Assert.NotNull(getCryptoCurrencyQuotesRequestForUSD.ConvertCurrencies);
			Assert.True(getCryptoCurrencyQuotesRequestForUSD.ConvertCurrencies.Count() == 1);
			Assert.Contains("USD", getCryptoCurrencyQuotesRequestForUSD.ConvertCurrencies);

		}

		public static TheoryData<GetCryptoCurrencyQuoteRequest, string> GetInvalidRequest()
		{
			var result = new TheoryData<GetCryptoCurrencyQuoteRequest, string>();

			result.Add(null, "Request Is Null");
			result.Add(new GetCryptoCurrencyQuoteRequest(), "CryptoCurrencySymbol Is Null Or Empty");
			result.Add(new GetCryptoCurrencyQuoteRequest { CryptoCurrencySymbol = " " }, "CryptoCurrencySymbol Is Null Or Empty");

			return result;
		}
	}
}
