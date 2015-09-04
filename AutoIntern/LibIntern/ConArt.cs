using System;

namespace AutoIntern
{
	public static class ConArt
	{
		public static void Out (string message, MessageType msgtype = MessageType.Info)
		{
			ConsoleColor previousColor = Console.ForegroundColor;
			switch (msgtype)
			{
			case MessageType.Error:
				Console.ForegroundColor = ConsoleColor.Red;
				break;
			case MessageType.Warning:
				Console.ForegroundColor = ConsoleColor.Yellow;
				break;
			case MessageType.Info:
				Console.ForegroundColor = ConsoleColor.Blue;
				break;
			}
			Console.Write (msgtype);
			Console.Write (" : ");
			Console.ForegroundColor = previousColor;
			Console.WriteLine (message);
		}

		public static void Ask(string message)
		{
			ConsoleColor previousColor = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.White;
			Console.Write (message);
			Console.Write (" : ");
			Console.ForegroundColor = previousColor;
		}

	}

	public	enum MessageType
	{
		Question,
		Info,
		Warning,
		Error
	}
}

