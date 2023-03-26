using System;

using Server.Accounting;

namespace Server.Misc
{
	public class AccountPrompt
	{
		public static void Initialize()
		{
			if (Accounts.Count == 0 && !Core.Service)
			{
				ConsoleKeyInfo key;

				do
				{
					Utility.WriteLine(ConsoleColor.Red, "This server has no accounts.");
					Utility.WriteLine(ConsoleColor.Yellow, "Create owner account? (Y/N)");

					key = Console.ReadKey();
				}
				while (key.Key != ConsoleKey.Y && key.Key != ConsoleKey.N);

				if (key.Key == ConsoleKey.Y)
				{
					Console.WriteLine();

					Console.Write("Username: ");
					var username = Console.ReadLine();

					Console.Write("Password: ");
					var password = Console.ReadLine();

					var a = Accounts.Create(username, password);

					if (a?.Deleted == false)
					{
						a.AccessLevel = AccessLevel.Owner;

						Console.WriteLine("Account created.");

						return;
					}
				}

				Console.WriteLine();

				Console.WriteLine("Account not created.");
			}
		}
	}
}
