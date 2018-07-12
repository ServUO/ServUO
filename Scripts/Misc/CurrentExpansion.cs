#region Header
// **********
// ServUO - CurrentExpansion.cs
// **********
#endregion

#region References
using System;

using Server.Accounting;
using Server.Network;
#endregion

namespace Server
{
	public class CurrentExpansion
	{
		public static readonly Expansion Expansion = Config.GetEnum("Expansion.CurrentExpansion", Expansion.EJ);

		[CallPriority(Int32.MinValue)]
		public static void Configure()
		{
			Core.Expansion = Expansion;

			AccountGold.Enabled = Core.TOL;
			AccountGold.ConvertOnBank = true;
			AccountGold.ConvertOnTrade = false;
			VirtualCheck.UseEditGump = true;
            Server.Services.TownCryer.TownCryerSystem.Enabled = Core.TOL;

			ObjectPropertyList.Enabled = Core.AOS;

            Mobile.InsuranceEnabled = Core.AOS && !Siege.SiegeShard;
			Mobile.VisibleDamageType = Core.AOS ? VisibleDamageType.Related : VisibleDamageType.None;
			Mobile.GuildClickMessage = !Core.AOS;
			Mobile.AsciiClickMessage = !Core.AOS;

			if (!Core.AOS)
			{
				return;
			}

			AOS.DisableStatInfluences();

			if (ObjectPropertyList.Enabled)
			{
				PacketHandlers.SingleClickProps = true; // single click for everything is overriden to check object property list
			}

			Mobile.ActionDelay = Core.TOL ? 500 : Core.AOS ? 1000 : 500;
			Mobile.AOSStatusHandler = AOS.GetStatus;
		}
	}
}