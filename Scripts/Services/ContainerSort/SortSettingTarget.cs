using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Services.ContainerSort
{
	public class SortSettingTarget : Target
	{
		public SortSettingTarget()
			: base(-1, false, TargetFlags.None)
		{

		}

		protected override void OnTarget(Mobile from, object targeted)
		{
			if(targeted is Container && from is PlayerMobile)
			{
				var container = targeted as Container;

				if(from.OwnsContainer(container))
				{
					from.SendGump(new ContainerSortSettingsGump(from, container));
				}				
			}
			else
			{
				from.SendMessage("You can't target that.");
			}
		}
	}
}