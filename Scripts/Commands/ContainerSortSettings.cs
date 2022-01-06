using Server.Mobiles;
using Server.Services.ContainerSort;

namespace Server.Commands
{
	public class ContainerSortSettings
	{
		[CallPriority(100)]
		public static void Initialize()
		{
			if (Config.Get("QualityOfLifeFeatures.AllowContainerAutoSort", false))
			{
				CommandSystem.Register("ContainerSortSettings", AccessLevel.Player, ContainerSortSettings_OnCommand);
			}
		}

		[Usage("ContainerSortSettings")]
		[Description("Activates a target selector to pick the container to change the settings for.")]
		private static void ContainerSortSettings_OnCommand(CommandEventArgs e)
		{
			if (e?.Mobile is PlayerMobile)
			{
				e.Mobile.Target = new SortSettingTarget();
			}
		}
	}
}