using System;
namespace Assignment_2
{
	public class Token
	{
		public string Symbol { get; set; }

		public Token(string symbol)
		{
			Symbol = symbol;
		}
		public string GetSymbol()
		{
			return Symbol;
        }
	}
}

