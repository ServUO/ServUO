#region Header
// **********
// ServUO - CurrentExpansion.cs
// **********
#endregion

#region References
using Server.Network;
#endregion

namespace Server
{
	public class CurrentExpansion
	{
		private static readonly Expansion Expansion = Expansion.HS;

		public static void Configure()
		{
			Core.Expansion = Expansion;

			bool Enabled = Core.AOS;

			Mobile.InsuranceEnabled = Enabled;
			ObjectPropertyList.Enabled = Enabled;
			Mobile.VisibleDamageType = Enabled ? VisibleDamageType.Related : VisibleDamageType.None;
			Mobile.GuildClickMessage = !Enabled;
			Mobile.AsciiClickMessage = !Enabled;

			if (Enabled)
			{
				AOS.DisableStatInfluences();

				if (ObjectPropertyList.Enabled)
				{
					PacketHandlers.SingleClickProps = true; // single click for everything is overriden to check object property list
				}

				Mobile.ActionDelay = 1000;
				Mobile.AOSStatusHandler = AOS.GetStatus;
			}
		}
	}
}