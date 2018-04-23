#region References

using System;
using System.Collections;
using System.Linq;
using Server.Items;
using Server.Mobiles;
using Server.Regions;

#endregion

namespace Server.Commands
{
    public class AccountVaultCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register("AccountVault", AccessLevel.Player, AccountVault_OnCommand);
        }

        private static void AccountVault_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            PlayerMobile pm = from as PlayerMobile;

            if (from.Region.IsPartOf(typeof (TownRegion)))
            {
                string myaccount = from.Account.ToString();
                int toX = from.X;
                int toY = from.Y;
                int toZ = from.Z;

                ArrayList vaultlist = new ArrayList();

                foreach (AccountVault av in from item in World.Items.Values.OfType<AccountVault>()
                    select item
                    into av
                    let boxaccount = av.BoxAccount
                    where boxaccount == myaccount
                    select av)
                {
                    vaultlist.Add(av);
                }

                if (vaultlist.Count <= 0)
                {
                    AccountVault npav = new AccountVault(myaccount)
                    {
                        Z = toZ,
                        X = toX,
                        Y = toY,
                        Map = @from.Map,
                        Visible = true
                    };

                    npav.DisplayTo(from);
                    npav.DoContainerHideTimer(npav);

                    from.SendMessage("You summon your account vault.  It will disappear in twenty seconds.");
                }
                else
                {
                    AccountVault acctv = vaultlist[0] as AccountVault;

                    if (acctv != null)
                    {
                        if (acctv.LastSummoned + TimeSpan.FromMinutes(5.0) > DateTime.UtcNow)
                        {
                            from.SendMessage("You may only summon your account vault once every five minutes.");
                            return;
                        }

                        acctv.Z = toZ;
                        acctv.X = toX;
                        acctv.Y = toY;
                        acctv.Map = @from.Map;
                        acctv.Visible = true;
                        acctv.DisplayTo(@from);
                        acctv.DoContainerHideTimer(acctv);
                        acctv.LastSummoned = DateTime.UtcNow;
                    }

                    from.SendMessage("You summon your account vault.  It will disappear in twenty seconds.");
                }
            }
            else
            {
                from.SendMessage("You may only access your account vault while in town.");
            }
        }
    }
}