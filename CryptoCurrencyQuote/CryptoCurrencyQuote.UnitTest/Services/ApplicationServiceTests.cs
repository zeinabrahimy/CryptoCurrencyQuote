using CryptoCurrencyQuote.Services;
using CryptoCurrencyQuote.Wrappers;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;
using System.Collections.Generic;
using CryptoCurrencyQuote.Models.Entity;
using CryptoCurrencyQuote.Models.Dto;

namespace CryptoCurrencyQuote.UnitTest.Services
{
	public class ApplicationServiceTests
	{
		private Mock<ILogger> _logger;
		private Mock<ICryptoCurrencyService> _cryptoCurrencyService;
		private IApplicationService _applicationService;
		private Mock<IConsoleWrapper> _console;

		private void Initialize()
		{
			var loggerFactory = new Mock<ILoggerFactory>();
			_logger = new Mock<ILogger>();

			loggerFactory.Setup(x => x.CreateLogger(It.IsAny<string>())).Returns(_logger.Object);


			_console = new Mock<IConsoleWrapper>();
			_cryptoCurrencyService = new Mock<ICryptoCurrencyService>();
			_applicationService = new ApplicationService(_cryptoCurrencyService.Object, loggerFactory.Object, _console.Object);
		}

		[Fact]
		public async Task Run_DisplayMenuAndCryptoCurrenciesThenExit_CheckConsoleAndCryptoCurrencyServiceMethods()
		{
			// Arrange
			Initialize();
			_console.Setup(x => x.Clear());
			_console.Setup(x => x.WriteLine(It.IsAny<string>(), It.IsAny<ConsoleColor>()));
			_console.SetupSequence(x => x.ReadLine()).Returns("c").Returns("e");

			_logger.Setup(x => x.Log(It.Is<LogLevel>(a => a == LogLevel.Information), It.IsAny<EventId>(), It.Is<It.IsAnyType>((v, t) => true), It.IsAny<Exception>(), It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true))).Verifiable();

			List<CryptoCurrency> cryptoCurrencies = new List<CryptoCurrency>();
			_cryptoCurrencyService.Setup(x => x.GetCryptoCurrenciesAsync()).ReturnsAsync(cryptoCurrencies);


			// Act
			await _applicationService.Run(new string[] { });

			// Assert
			_console.Verify(x => x.WriteLine(It.Is<string>(input => input == "(C)rypto Currencies"), It.IsAny<ConsoleColor>()), Times.Exactly(2));
			_console.Verify(x => x.WriteLine(It.Is<string>(input => input == "(E)xit"), It.IsAny<ConsoleColor>()), Times.Exactly(2));
			_console.Verify(x => x.WriteLine(It.Is<string>(input => input == "Choose An Option Or Enter Crypto Currency Symbol:"), It.IsAny<ConsoleColor>()), Times.Exactly(2));
			_console.Verify(x => x.WriteLine(It.Is<string>(input => input == "Getting Data Started... Please Be Patient!"), It.IsAny<ConsoleColor>()), Times.Once);
			_cryptoCurrencyService.Verify(x => x.GetCryptoCurrenciesAsync(), Times.Once);
			_cryptoCurrencyService.Verify(x => x.GetCryptoCurrencyQuoteAsync(It.IsAny<GetCryptoCurrencyQuoteRequest>()), Times.Never);
		}

		[Fact]
		public async Task Run_DisplayMenuThenExitApplication_CheckConsoleAndCryptoCurrencyServiceMethods()
		{
			// Arrange
			Initialize();
			_console.Setup(x => x.Clear());
			_console.Setup(x => x.WriteLine(It.IsAny<string>(), It.IsAny<ConsoleColor>()));
			_console.Setup(x => x.ReadLine()).Returns("e");

			_logger.Setup(x => x.Log(It.Is<LogLevel>(a => a == LogLevel.Information), It.IsAny<EventId>(), It.Is<It.IsAnyType>((v, t) => true), It.IsAny<Exception>(), It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true))).Verifiable();

			// Act
			await _applicationService.Run(new string[] { });

			// Assert
			_console.Verify(x => x.WriteLine(It.Is<string>(input => input == "(C)rypto Currencies"), It.IsAny<ConsoleColor>()), Times.Exactly(1));
			_console.Verify(x => x.WriteLine(It.Is<string>(input => input == "(E)xit"), It.IsAny<ConsoleColor>()), Times.Exactly(1));
			_console.Verify(x => x.WriteLine(It.Is<string>(input => input == "Choose An Option Or Enter Crypto Currency Symbol:"), It.IsAny<ConsoleColor>()), Times.Exactly(1));
			_cryptoCurrencyService.Verify(x => x.GetCryptoCurrenciesAsync(), Times.Never);
			_cryptoCurrencyService.Verify(x => x.GetCryptoCurrencyQuoteAsync(It.IsAny<GetCryptoCurrencyQuoteRequest>()), Times.Never);
		}

		[Fact]
		public async Task Run_DisplayMenuAndDisplayBTCQuoteThenExit_CheckConsoleAndCryptoCurrencyServiceMethods()
		{
			// Arrange
			Initialize();
			string cryptoSymbol = "BTN";
			_console.Setup(x => x.Clear());
			_console.Setup(x => x.WriteLine(It.IsAny<string>(), It.IsAny<ConsoleColor>()));
			_console.SetupSequence(x => x.ReadLine()).Returns(cryptoSymbol).Returns("e");

			_logger.Setup(x => x.Log(It.Is<LogLevel>(a => a == LogLevel.Information), It.IsAny<EventId>(), It.Is<It.IsAnyType>((v, t) => true), It.IsAny<Exception>(), It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true))).Verifiable();


			GetCryptoCurrencyQuoteResponse getCryptoCurrencyQuoteResponse = new GetCryptoCurrencyQuoteResponse();
			GetCryptoCurrencyQuoteRequest getCryptoCurrencyQuoteRequest = null;
			_cryptoCurrencyService.Setup(x => x.GetCryptoCurrencyQuoteAsync(It.IsAny<GetCryptoCurrencyQuoteRequest>()))
														.Callback<GetCryptoCurrencyQuoteRequest>(x => getCryptoCurrencyQuoteRequest = x)
														.ReturnsAsync(getCryptoCurrencyQuoteResponse);


			// Act
			await _applicationService.Run(new string[] { });

			// Assert
			_console.Verify(x => x.WriteLine(It.Is<string>(input => input == "(C)rypto Currencies"), It.IsAny<ConsoleColor>()), Times.Exactly(2));
			_console.Verify(x => x.WriteLine(It.Is<string>(input => input == "(E)xit"), It.IsAny<ConsoleColor>()), Times.Exactly(2));
			_console.Verify(x => x.WriteLine(It.Is<string>(input => input == "Choose An Option Or Enter Crypto Currency Symbol:"), It.IsAny<ConsoleColor>()), Times.Exactly(2));
			_cryptoCurrencyService.Verify(x => x.GetCryptoCurrenciesAsync(), Times.Never);
			_cryptoCurrencyService.Verify(x => x.GetCryptoCurrencyQuoteAsync(It.IsAny<GetCryptoCurrencyQuoteRequest>()), Times.Once);
			Assert.NotNull(getCryptoCurrencyQuoteRequest);
			Assert.NotNull(getCryptoCurrencyQuoteRequest.CryptoCurrencySymbol);
			Assert.Equal(cryptoSymbol.ToLower(), getCryptoCurrencyQuoteRequest.CryptoCurrencySymbol.ToLower());

		}

	}
}
