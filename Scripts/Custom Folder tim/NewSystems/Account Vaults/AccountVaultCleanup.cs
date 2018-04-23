#region References

using System;
using System.Collections;
using System.Linq;
using Server.Items;

#endregion

namespace Server.Commands
{
    public class AccountVaultCleanupCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register("AccountVaultCleanup", AccessLevel.Owner, AccountVaultCleanup_OnCommand);
        }

        private static void AccountVaultCleanup_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            int numberremoved = 0;
            ArrayList vaultlist = new ArrayList();

            foreach (AccountVault av in World.Items.Values.OfType<AccountVault>().Select(item => item as AccountVault))
            {
                vaultlist.Add(av);
            }

            foreach (AccountVault av in from item in vaultlist.OfType<AccountVault>() select item as AccountVault into av let list = av.Items where list.Count <= 0 select av)
            {
                av.Delete();
                numberremoved++;
            }

            from.SendMessage("{0} empty account vaults cleaned up.", numberremoved.ToString());
        }
    }

    public class AccountVaultCleanupTimer : Timer
    {
        public static void Initialize()
        {
            new AccountVaultCleanupTimer();
        }

        public AccountVaultCleanupTimer()
            : base(TimeSpan.FromSeconds(5), TimeSpan.FromHours(4))
        {
            this.Start();
        }

        protected override void OnTick()
        {
            Console.WriteLine("Cleaning Empty Account Vaults.");

            int numberremoved = 0;
            ArrayList vaultlist = new ArrayList();

            foreach (AccountVault av in World.Items.Values.OfType<AccountVault>().Select(item => item as AccountVault))
            {
                vaultlist.Add(av);
            }

            foreach (AccountVault av in from item in vaultlist.OfType<AccountVault>() select item as AccountVault into av let list = av.Items where list.Count <= 0 select av)
            {
                av.Delete();
                numberremoved++;
            }

            Console.WriteLine("{0} empty account vaults cleaned up.", numberremoved);
        }
    }
}