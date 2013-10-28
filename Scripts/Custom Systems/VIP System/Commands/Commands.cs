using System;
using Server;
using Server.Commands;
using Server.Mobiles;

namespace CustomsFramework.Systems.VIPSystem
{
    public partial class VIPCore
    {
        [Usage("VIP")]
        [Description("Opens a gump to view your VIP status and settings.")]
        private static void Command_VIP(CommandEventArgs e)
        {
            PlayerMobile from = e.Mobile as PlayerMobile;

            if (from != null && !from.Deleted)
            {
                VIPModule module = from.GetModule(typeof(VIPModule)) as VIPModule;

                if (module == null)
                {
                    from.SendMessage("You haven't donated to become a VIP player yet.");
                    return;
                }
                else
                {
                    if (module.Tier == VIPTier.None)
                    {
                        from.SendMessage("You currently have the following bonuses enabled.");

                        foreach (Bonus bonus in module.Bonuses)
                        {
                            if (bonus.Enabled)
                                from.SendMessage(String.Format("{0} - Time Left: {1}", bonus.BonusName, ((bonus.TimeStarted + bonus.ServicePeriod) - DateTime.UtcNow).Days));
                        }
                    }
                    else if (module.Tier == VIPTier.Bronze)
                    {
                        from.SendMessage("Thanks for donating to be a Bronze player!");
                        from.SendMessage("You currently have the following bonuses enabled.");

                        foreach (Bonus bonus in module.Bonuses)
                        {
                            if (bonus.Enabled)
                                from.SendMessage(String.Format("{0} - Time Left: {1}", bonus.BonusName, ((bonus.TimeStarted + bonus.ServicePeriod) - DateTime.UtcNow).Days));
                        }
                    }
                    else if (module.Tier == VIPTier.Silver)
                    {
                        from.SendMessage("Thanks for donating to be a Silver player!");
                        from.SendMessage("You currently have the following bonuses enabled.");

                        foreach (Bonus bonus in module.Bonuses)
                        {
                            if (bonus.Enabled)
                                from.SendMessage(String.Format("{0} - Time Left: {1}", bonus.BonusName, ((bonus.TimeStarted + bonus.ServicePeriod) - DateTime.UtcNow).Days));
                        }
                    }
                    else if (module.Tier == VIPTier.Gold)
                    {
                        from.SendMessage("Thanks for donating to be a Gold player!!!");
                        from.SendMessage("You currently have the following bonuses enabled.");

                        foreach (Bonus bonus in module.Bonuses)
                        {
                            if (bonus.Enabled)
                                from.SendMessage(String.Format("{0} - Time Left: {1}", bonus.BonusName, ((bonus.TimeStarted + bonus.ServicePeriod) - DateTime.UtcNow).Days));
                        }
                    }
                }
            }
        }
    }
}