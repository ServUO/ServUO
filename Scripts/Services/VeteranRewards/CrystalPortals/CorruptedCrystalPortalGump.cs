namespace Server.Gumps
{
	public class CorruptedCrystalPortalGump : Gump
	{
		public CorruptedCrystalPortalGump(Mobile from)
			: base(25, 25)
		{
			from.CloseGump(typeof(CorruptedCrystalPortalGump));

			Closable = true;
			Disposable = false;
			Dragable = true;
			Resizable = false;

			AddPage(0);
			AddBackground(0, 2, 373, 515, 9200);
			AddHtml(
				14,
				15,
				346,
				484,
				@"<br><br>This corrupted portal allows you to teleport directly to a dungeon.<br><br>For Trammel ruleset, say 'dungeon' followed by the name of the dungeon (e.g. 'dungeon shame'). For Felucca, say 'fel' then same rules as above. So 'fel dungeon shame'.<br><br>DUNGEON NAMES:<br>covetous, deceit, despise, destard, ice, fire, hythloth, orc, shame, wrong, wind, doom, citadel, fandancer, mines, bedlam, labyrinth, underworld, abyss.<br><br>FEL DUNGEON NAMES<br>covetous, deceit, despise, destard, fire, hythloth, ice, orc, shame, wrong, wind<br><br>The same teleportation rules apply regarding criminal flagging, weight, etc.<br><br>",
				true,
				true);
		}
	}
}