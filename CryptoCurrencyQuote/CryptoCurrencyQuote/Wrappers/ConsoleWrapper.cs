using System;

namespace CryptoCurrencyQuote.Wrappers
{
	public class ConsoleWrapper : IConsoleWrapper
	{
		public void Write(string message, ConsoleColor color = ConsoleColor.White)
		{
			Console.ForegroundColor = color;
			Console.Write(message);
		}

		public void WriteLine()
		{
			Console.WriteLine();
		}

		public void WriteLine(string message, ConsoleColor color = ConsoleColor.White)
		{
			Console.ForegroundColor = color;
			Console.WriteLine(message);
		}

		public string ReadLine()
		{
			return Console.ReadLine();
		}

		public ConsoleKeyInfo ReadKey()
		{
			return Console.ReadKey();
		}

		public void Clear()
		{
			Console.Clear();
		}
	}
}
