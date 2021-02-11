﻿using CryptoCurrencyQuote.Models;
using CryptoCurrencyQuote.Proxies;
using CryptoCurrencyQuote.Services;
using CryptoCurrencyQuote.Wrappers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.IO;

namespace CryptoCurrencyQuote
{
	public class Startup
	{
		public Startup(string appSettingsFile = "appsettings.json")
		{
			var builder = new ConfigurationBuilder()
												.SetBasePath(Directory.GetCurrentDirectory())
												.AddJsonFile(appSettingsFile, optional: true, reloadOnChange: true);

			Configuration = builder.Build();
		}

		public IConfiguration Configuration { get; }

		public ServiceProvider ConfigureServices()
		{
			IServiceCollection services = new ServiceCollection();

			services.AddOptions();

			var appSettingsSection = Configuration.GetSection("AppSettings");
			services.Configure<AppSettings>(appSettingsSection);

			services.AddHttpClient();
			services.AddLogging(loggingBuilder =>
			{
				loggingBuilder.AddLog4Net("log4net.config");
			});

			services.AddScoped<ICryptoCurrencyService, CryptoCurrencyService>();
			services.AddScoped<IExchangeRatesProxy, ExchangeRatesProxy>();
			services.AddScoped<ICoinMarketCapProxy, CoinMarketCapProxy>();
			services.AddSingleton<IConsoleWrapper, ConsoleWrapper>();
			services.AddScoped<IApplicationService, ApplicationService>();

			return services.BuildServiceProvider();
		}
	}
}
