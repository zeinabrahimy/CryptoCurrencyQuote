using CryptoCurrencyQuote.Proxies;
using CryptoCurrencyQuote.Services;
using CryptoCurrencyQuote.Wrappers;
using CryptoCurrencyQuote.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http;
using Xunit;

namespace CryptoCurrencyQuote.UnitTest
{
	public class StartUpTest
	{
		[Fact]
		public void ConfigureServices_WhenInvoke_RegistersDependenciesAndSettings()
		{
			// Arrange
			var serviceProvider = new Startup("testappsettings.json").ConfigureServices();

			// Act
			using var scope = serviceProvider.CreateScope();

			// Assert
			Assert.NotNull(scope);
			var scopeServiceProvider = scope.ServiceProvider;

			Assert.NotNull(scopeServiceProvider.GetRequiredService<ICryptoCurrencyService>());
			Assert.NotNull(scopeServiceProvider.GetRequiredService<IApplicationService>());
			Assert.NotNull(scopeServiceProvider.GetRequiredService<ICoinMarketCapProxy>());
			Assert.NotNull(scopeServiceProvider.GetRequiredService<IExchangeRatesProxy>());
			Assert.NotNull(scopeServiceProvider.GetRequiredService<ILoggerFactory>());
			Assert.NotNull(scopeServiceProvider.GetRequiredService<IHttpClientFactory>());
			Assert.NotNull(scopeServiceProvider.GetRequiredService<IConsoleWrapper>());

			var appSettings = scopeServiceProvider.GetRequiredService<IOptions<AppSettings>>();
			Assert.NotNull(appSettings);
			Assert.Equal("USD", appSettings.Value.BaseCurrency);
			Assert.Equal("cryptocurrencyquotewithexchangeapproach", appSettings.Value.CalculateQuoteApproach);
			Assert.Equal("https://api.exchangeratesapi.io/", appSettings.Value.ExchangeRatesApiUrl);
			Assert.Equal(10, appSettings.Value.MaximumFloatingPointDigit);

			Assert.NotNull(appSettings.Value.Currencies);
			Assert.Contains("USD", appSettings.Value.Currencies);
			Assert.Contains("EUR", appSettings.Value.Currencies);
			Assert.Contains("BRL", appSettings.Value.Currencies);
			Assert.Contains("GBP", appSettings.Value.Currencies);
			Assert.Contains("AUD", appSettings.Value.Currencies);

			Assert.NotNull(appSettings.Value.CoinMarketCapSetting);
			Assert.Equal("https://pro-api.coinmarketcap.com/v1/", appSettings.Value.CoinMarketCapSetting.ApiUrl);
			Assert.Equal("c5af7d50-80bd-4788-8248-cbcb1f949618", appSettings.Value.CoinMarketCapSetting.ApiKey);
		}
	}
}
