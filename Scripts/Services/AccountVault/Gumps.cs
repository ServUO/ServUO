using Server.Gumps;
using Server.Mobiles;
using Server.Prompts;
using Server.Engines.UOStore;

using System.Globalization;
using System.Linq;

namespace Server.AccountVault
{
    public class NewVaultPurchaseGump : BaseGump
    {
        public AccountVault Vault { get; private set; }

        public NewVaultPurchaseGump(PlayerMobile pm, AccountVault vault)
            : base(pm, 50, 100)
        {
            Vault = vault;
        }

        public override void AddGumpLayout()
        {
            AddImage(0, 0, 0x9BF2);
            AddButton(12, 15, 0x930, 0x930, 1, GumpButtonType.Reply, 0);
            AddItem(12, 15, 0xE42);

            if (Vault != null)
            {
                AddItemProperty(Vault);
            }
        }

        public override void OnResponse(RelayInfo info)
        {
            if (info.ButtonID == 1 && Vault != null && User.InRange(Vault.GetWorldLocation(), 100))
            {
                Vault.MoveTo(User);
            }
        }
    }

    public class VaultActionsGump : BaseGump
    {
        public AccountVault Vault { get; set; }

        public VaultActionsGump(PlayerMobile pm, AccountVault vault)
            : base(pm, 500, 100)
        {
            Vault = vault;
        }

        public override void AddGumpLayout()
        {
            AddBackground(0, 0, 440, 270, 0x2454);
            AddHtmlLocalized(10, 10, 420, 18, CenterLoc, "#1157978", 0, false, false); // Vault Actions

            bool tokens = SystemSettings.UseTokens;
            var store = UltimaStore.GetProfile(User);
            var acct = User.Account;

            AddHtmlLocalized(10, 33, 165, 18, 1157990, 0x90D, false, false); // Vault Tokens
            AddLabel(185, 33, 0, tokens ? store.VaultTokens.ToString() : "N/A"); // vault token count

            AddHtmlLocalized(10, 51, 165, 18, 1157985, 0x90D, false, false); // Vault Rent Account:

            if (Vault.PastDue)
            {
                AddHtmlLocalized(185, 51, 165, 18, 1158081, false, false); // Past Due
            }
            else
            {
                AddLabel(185, 51, 0, Vault.Balance.ToString("N0", CultureInfo.GetCultureInfo("en-US"))); // vault rent account count
            }

            AddHtmlLocalized(10, 69, 165, 18, 1156044, 0x90D, false, false); // Total Gold:
            AddLabel(185, 69, 0, acct != null ? acct.TotalGold.ToString("N0", CultureInfo.GetCultureInfo("en-US")) : "0");

            AddHtmlLocalized(10, 87, 165, 18, 1156045, 0x90D, false, false); // Total Platinum:
            AddLabel(185, 87, 0, acct != null ? acct.TotalPlat.ToString("N0", CultureInfo.GetCultureInfo("en-US")) : "0");

            //AddHtmlLocalized(10, 105, 165, 18, 1157986, 0x90D, false, false); // Vault Rent Account:
            //AddLabel(185, 105, 0, ""); // vault rend account count???

            AddHtmlLocalized(55, 141, 360, 22, tokens ? 1157983 : 1157979, 0x90D, false, false); // Deposit Vault Token into Vault Rent Account : Deposit Gold into Vault Rent Account
            if (tokens)
            {
                AddTooltip(string.Format("Transfers purchased vault tokens from the player to the vault rent account; capped at {0} tokens. Tokens added to the vault rent account will be used for auto payments of the vault rent.", SystemSettings.MaxBalance.ToString("N0", CultureInfo.GetCultureInfo("en-US"))));
            }
            else
            {
                AddTooltip(string.Format("Transfers gold from the player's bank to the vault rent account; capped at {0} gold. Funds added to the vault rent account will be used for auto payments of the vault rent.", SystemSettings.MaxBalance.ToString("N0", CultureInfo.GetCultureInfo("en-US"))));
            }
            AddButton(15, 141, 0xFA5, 0xFA6, 102, GumpButtonType.Reply, 0);

            AddHtmlLocalized(55, 174, 360, 22, 1158143, 0x90D, false, false);
            AddButton(15, 174, 0xFA5, 0xFA6, 103, GumpButtonType.Reply, 0);
        }

        public override void OnResponse(RelayInfo info)
        {
            switch (info.ButtonID)
            {
                case 102:
                    User.SendLocalizedMessage(1155865); // Enter amount to deposit:
                    User.Prompt = new InternalPrompt(User, Vault);
                    break;
                case 103:
                    SendGump(new VaultLocationsGump(User));
                    break;
            }
        }

        private class InternalPrompt : Prompt
        {
            public PlayerMobile From { get; private set; }
            public AccountVault Vault { get; private set; }

            public InternalPrompt(PlayerMobile from, AccountVault vault)
            {
                From = from;
                Vault = vault;
            }

            public override void OnCancel(Mobile from)
            {
                SendGump(new VaultActionsGump(From, Vault));
            }

            public override void OnResponse(Mobile from, string text)
            {
                var num = Utility.ToInt32(text);

                if (num <= 0 || Vault.Balance + num > SystemSettings.MaxBalance || !SystemSettings.HasBalance(From, num))
                {
                    from.SendLocalizedMessage(1155867); // The amount entered is invalid. Verify that there are sufficient funds to complete this transaction.
                }
                else
                {
                    from.SendLocalizedMessage(1153188); // Transaction successful:

                    SystemSettings.WithdrawBalance(from as PlayerMobile, num);

                    Vault.Balance += num;
                }

                SendGump(new VaultActionsGump(From, Vault));
            }
        }
    }

    public class VaultLocationsGump : BaseGump
    {
        public VaultLocationsGump(PlayerMobile pm)
            : base(pm, 150, 200)
        {
        }

        public override void AddGumpLayout()
        {
            var locs = SystemSettings.VaultRegions.Length;

            AddBackground(0, 0, 304, 106 + (locs * 20), 0x24A4);
            AddHtmlLocalized(35, 10, 250, 20, CenterLoc, "#1158143", 0, false, false); // Vault Locations

            var y = 50;

            for (int i = 0; i < SystemSettings.VaultRegions.Length; i++)
            {
                var region = SystemSettings.VaultRegions[i];

                AddHtmlLocalized(55, y, 100, 18, AlignRightLoc, SystemSettings.RegionLabel(region), 0, false, false); // Royal City
                AddHtml(159, y, 40, 18, VaultCount(region).ToString("N0", CultureInfo.GetCultureInfo("en-US")), false, false);

                y += 20;
            }
        }

        public static int VaultCount(string region)
        {
            return AccountVault.Vaults.Count(v => v.Account == null && Region.Find(v.GetWorldLocation(), v.Map).IsPartOf(region) && AccountVault.ValidateMap(v));
        }
    }
}
