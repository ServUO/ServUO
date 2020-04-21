#region References
using Server.Accounting;
using Server.Services.TownCryer;
using System;
#endregion

namespace Server
{
    public static class ShardSettings
    {
        [CallPriority(Int32.MinValue)]
        public static void Configure()
        {
            Core.Expansion = Expansion.EJ;

            AccountGold.Enabled = true;
            AccountGold.ConvertOnBank = true;
            AccountGold.ConvertOnTrade = false;
            VirtualCheck.UseEditGump = true;

            TownCryerSystem.Enabled = true;

            Mobile.InsuranceEnabled = !Siege.SiegeShard;
            Mobile.VisibleDamageType = VisibleDamageType.Related;

            AOS.DisableStatInfluences();

            Mobile.ActionDelay = 500;
            Mobile.AOSStatusHandler = AOS.GetStatus;
        }
    }
}
