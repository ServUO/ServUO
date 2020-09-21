using Server.Accounting;
using Server.Commands;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Server.Items
{
    public static class AccountGoldCheck
    {
        public static void Initialize()
        {
            CommandSystem.Register("CheckAccountGold", AccessLevel.Administrator, e =>
                {
                    double currency = 0.0;

                    Dictionary<string, long> table = new Dictionary<string, long>();

                    foreach (Account account in Accounts.GetAccounts().OfType<Account>())
                    {
                        table[account.Username] = (long)(account.TotalCurrency * Account.CurrencyThreshold);
                        currency += account.TotalCurrency;
                    }

                    using (StreamWriter op = new StreamWriter("TotalAccountGold.txt", true))
                    {
                        foreach (KeyValuePair<string, long> kvp in table.OrderBy(k => -k.Value))
                        {
                            op.WriteLine(
                                string.Format("{0} currency: {1}", kvp.Key, kvp.Value.ToString("N0", System.Globalization.CultureInfo.GetCultureInfo("en-US"))));
                        }

                        op.WriteLine("");
                        op.WriteLine("Total Accounts: {0}", table.Count);
                        op.WriteLine("Total Shard Gold: {0}", (currency * Account.CurrencyThreshold).ToString("N0", System.Globalization.CultureInfo.GetCultureInfo("en-US")));
                    }
                });
        }
    }
}