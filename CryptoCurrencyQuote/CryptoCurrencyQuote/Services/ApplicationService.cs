using CryptoCurrencyQuote.Models.Entity;
using CryptoCurrencyQuote.Models.Dto;
using CryptoCurrencyQuote.Wrappers;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoCurrencyQuote.Services
{
	public class ApplicationService : IApplicationService
	{
		private readonly ICryptoCurrencyService _cryptoCurrencyService;
		private readonly ILogger _logger;
		private readonly IConsoleWrapper _console;
		public ApplicationService(ICryptoCurrencyService cryptoCurrencyService,
															ILoggerFactory loggerFactory,
															IConsoleWrapper console)
		{
			_cryptoCurrencyService = cryptoCurrencyService;
			_logger = loggerFactory.CreateLogger(GetType());
			_console = console;
		}


		#region Implementation Of IApplicationService

		public async Task Run(string[] args)
		{
			_console.Clear();
			_logger.LogInformation("Starting Application.");

			await DisplayMenu().ConfigureAwait(false);

			_logger.LogInformation("All done!");
		}

		#endregion

		#region Private Methods

		private async Task DisplayMenu()
		{
			_console.WriteLine("(C)rypto Currencies");
			_console.WriteLine("(E)xit");
			_console.WriteLine("Choose An Option Or Enter Crypto Currency Symbol:");

			var userCommand = _console.ReadLine().ToLower();
			switch (userCommand)
			{
				case "c":
					await DisplayCryptoCurrencies().ConfigureAwait(false);
					break;

				case "e":
					return;

				default:
					await GetCryptoCurrencyQuote(userCommand).ConfigureAwait(false);
					break;
			}
			_console.ReadLine();
		}

		private async Task DisplayCryptoCurrencies()
		{
			_console.WriteLine("Getting Data Started... Please Be Patient!");
			IEnumerable<CryptoCurrency> cryptoCurrencies = await _cryptoCurrencyService.GetCryptoCurrenciesAsync().ConfigureAwait(false);

			bool hasDataToDisplay = cryptoCurrencies != null && cryptoCurrencies.Any();

			_console.WriteLine($"There Is/Are {(hasDataToDisplay ? cryptoCurrencies.Count() : '0')} Crypto Currency/ies");

			if (hasDataToDisplay)
			{
				var maxNameLenght = cryptoCurrencies.ToList().Max(x => x.Name.Length);
				cryptoCurrencies.ToList().ForEach(x =>
				{
					_console.WriteLine($"Name: {x.Name.PadRight(maxNameLenght, '-')} Symbol: {x.Symbol}", System.ConsoleColor.Green);
				});
			}

			await EndOfDisplayData();
		}

		private async Task GetCryptoCurrencyQuote(string cryptoSymbol)
		{
			_console.WriteLine("Getting Data Started... Please Be Patient!");
			var result = await _cryptoCurrencyService.GetCryptoCurrencyQuoteAsync(new GetCryptoCurrencyQuoteRequest
			{
				CryptoCurrencySymbol = cryptoSymbol.Trim()
			}).ConfigureAwait(false);

			if (result != null && result.Quotes != null && result.Quotes.Any())
			{
				result.Quotes.ToList().ForEach(x =>
				{
					_console.WriteLine($"{x.Key} : {x.Value}", System.ConsoleColor.Green);
				});
			}
			else
			{
				_console.WriteLine(string.IsNullOrWhiteSpace(result.Message) ? "Unexpected Error. Try Again" : result.Message, System.ConsoleColor.Red);
			}

			await EndOfDisplayData();
		}

		private async Task EndOfDisplayData()
		{
			_console.WriteLine("____________________________________________________________________________________");
			_console.WriteLine();

			await DisplayMenu().ConfigureAwait(false);
		}


		#endregion
	}
}
