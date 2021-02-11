using System.Threading.Tasks;

namespace CryptoCurrencyQuote.Services
{
	public interface IApplicationService
	{
		Task Run(string[] args);
	}
}
