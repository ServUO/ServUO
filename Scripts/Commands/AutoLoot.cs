using Server.Mobiles;

namespace Server.Commands
{
	public class AutoLoot
	{
		[CallPriority(100)]
		public static void Initialize()
		{
			if (Config.Get("QualityOfLifeFeatures.AllowGoldAutoLoot", false))
			{
				CommandSystem.Register("ToggleAutoLoot", AccessLevel.Player, ToggleAutoLoot_OnCommand);
				CommandSystem.Register("ToggleAutoLootMessages", AccessLevel.Player, ToggleAutoLootMessages_OnCommand);
				CommandSystem.Register("ToggleAutoLootShareWithParty", AccessLevel.Player, ToggleAutoLootShareWithParty_OnCommand);
			}
		}

		[Usage("ToggleAutoLoot")]
		[Description("Toggles the auto loot of gold feature on and off")]
		private static void ToggleAutoLoot_OnCommand(CommandEventArgs e)
		{
			if (e?.Mobile is PlayerMobile)
			{
				var playerMobile = (PlayerMobile)e.Mobile;

				playerMobile.AllowAutoLoot = !playerMobile.AllowAutoLoot;

				if (playerMobile.AllowAutoLoot)
				{
					playerMobile.SendMessage("Auto looting has been enabled.");
				}
				else
				{
					playerMobile.SendMessage("Auto looting has been disabled.");
				}
			}
		}

		[Usage("ToggleAutoLootMessages")]
		[Description("Toggles the messages that appear when you auto loot on and off")]
		private static void ToggleAutoLootMessages_OnCommand(CommandEventArgs e)
		{
			if (e?.Mobile is PlayerMobile)
			{
				var playerMobile = (PlayerMobile)e.Mobile;

				playerMobile.ShowAutoLootMessages = !playerMobile.ShowAutoLootMessages;

				if (playerMobile.ShowAutoLootMessages)
				{
					playerMobile.SendMessage("Auto looting messages have been enabled.");
				}
				else
				{
					playerMobile.SendMessage("Auto looting messages have been disabled.");
				}
			}
		}

		[Usage("ToggleAutoLootShareWithParty")]
		[Description("Toggles the messages that appear when you auto loot on and off")]
		private static void ToggleAutoLootShareWithParty_OnCommand(CommandEventArgs e)
		{
			if (e?.Mobile is PlayerMobile)
			{
				var playerMobile = (PlayerMobile)e.Mobile;

				playerMobile.ShareAutoLootWithParty = !playerMobile.ShareAutoLootWithParty;

				if (playerMobile.ShareAutoLootWithParty)
				{
					playerMobile.SendMessage("Sharing auto loot with party members has been enabled.");
				}
				else
				{
					playerMobile.SendMessage("Sharing auto loot with party members has been disabled.");
				}
			}
		}
	}
}