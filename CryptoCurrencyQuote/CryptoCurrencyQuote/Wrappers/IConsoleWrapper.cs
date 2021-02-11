using System;

namespace CryptoCurrencyQuote.Wrappers
{
	public interface IConsoleWrapper
	{
		string ReadLine();
		void Write(string message, ConsoleColor color = ConsoleColor.White);
		void WriteLine(string message, ConsoleColor color = ConsoleColor.White);
		void WriteLine();
		ConsoleKeyInfo ReadKey();
		void Clear();
	}
}
