namespace Server.Gumps
{
	public class CorruptedCrystalPortalGump : Gump
	{
        public override int GetTypeID()
        {
            return 0x237B;
        }

		public CorruptedCrystalPortalGump(Mobile from)
			: base(25, 25)
		{
            from.CloseGump(typeof(CorruptedCrystalPortalGump));

            AddImage(0, 0, 0x1FE);
            AddPage(1);
            AddHtmlLocalized(40, 30, 150, 48, 1150074, 0, false, false); // Corrupted Crystal Portal
            AddHtmlLocalized(40, 160, 150, 16, 1113300, 0, false, false); // by
            AddHtmlLocalized(40, 180, 150, 32, 1113299, 0, false, false); // <center>(unknown)</center>
            AddHtmlLocalized(230, 30, 145, 160, 1150076, 0, false, false); // This corrupted portal allows you to teleport directly to a dungeon.<br><br>For
            // Trammel ruleset, say "dungeon" followed by the name of the dungeon (e.g. "dungeon shame"). 

            AddLabel(250, 200, 0, "1"); // todo: get

            AddButton(356, 0, 0x200, 0x200, 0, GumpButtonType.Page, 2);

            AddPage(2);

            AddHtmlLocalized(40, 35, 150, 160, 1150077, 0, false, false); // For Felucca, say "fel" then same rules as above. So "fel dungeon shame".

            AddHtmlLocalized(230, 30, 145, 160, 1150075, 0, false, false); // DUNGEON NAMES:<br>covetous, deceit, despise, destard, ice, fire, hythloth, 
            // orc, shame, wrong, wind, doom, citadel, fandancer, mines, bedlam, labyrinth,
            //underworld, abyss, grove, caves, palace, prism,

            AddLabel(90, 200, 0, "2"); // toto: get
            AddLabel(250, 200, 0, "3"); // todo : get

            AddButton(0, 0, 0x1FF, 0x1FF, 0, GumpButtonType.Page, 1);
            AddButton(356, 0, 0x200, 0x200, 0, GumpButtonType.Page, 3);

            AddPage(3);

            AddHtmlLocalized(40, 35, 150, 160, 1155586, 0, false, false); // sanctuary, blackthorn.
            AddHtmlLocalized(230, 30, 145, 160, 1150078, 0, false, false); // The same teleportation rules apply regarding criminal flagging, weight, etc.

            AddLabel(90, 200, 0, "4"); // todo: get
            AddLabel(250, 200, 0, "5"); // todo: get

            AddButton(0, 0, 0x1FF, 0x1FF, 0, GumpButtonType.Page, 2);
		}
	}
}