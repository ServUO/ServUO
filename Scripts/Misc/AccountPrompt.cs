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
                Console.WriteLine("This server has no accounts.");
                Console.Write("Do you want to create the owner account now? (y/n)");

                string key = Console.ReadLine();
 
                if (key.ToUpper() == "Y")
                {
                    Console.WriteLine();

                    Console.Write("Username: ");
                    string username = Console.ReadLine();

                    Console.Write("Password: ");
                    string password = Console.ReadLine();

                    Account a = new Account(username, password);
                    a.AccessLevel = AccessLevel.Owner;

                    Console.WriteLine("Account created.");
                }
                else
                {
                    Console.WriteLine();

                    Console.WriteLine("Account not created.");
                }
            }
        }
    }
}