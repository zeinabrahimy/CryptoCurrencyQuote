using CryptoCurrencyQuote.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace CryptoCurrencyQuote
{
	public class Program
	{
		static async Task Main(string[] args)
		{
			var serviceProvider = new Startup().ConfigureServices();
			var applicationService = serviceProvider.GetService<IApplicationService>();
			await applicationService.Run(args);
		}
	}
}
