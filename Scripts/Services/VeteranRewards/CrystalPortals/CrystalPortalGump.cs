namespace Server.Gumps
{
	public class CrystalPortalGump : Gump
	{
        public override int GetTypeID()
        {
            return 0x237B;
        }

		public CrystalPortalGump(Mobile from)
			: base(245, 200)
		{
            from.CloseGump(typeof(CrystalPortalGump));

            AddImage(0, 0, 0x1FE);
            AddPage(1);
            AddHtmlLocalized(40, 30, 150, 48, 1113945, 0, false, false); // Crystal Portal
            AddHtmlLocalized(40, 160, 150, 16, 1113300, 0, false, false); // by
            AddHtmlLocalized(40, 180, 150, 32, 1113299, 0, false, false); // <center>(unknown)</center>
            AddHtmlLocalized(230, 30, 145, 160, 1113946, 0, false, false); // This crystal portal allows you to teleport directly to a bank
                                                                           // or a moongate.<br><br>For Trammel ruleset, say the city's name
                                                                           // followed by "mint" (e.g. "minoc mint"). For a 

            AddLabel(250, 200, 0, "1"); // todo: get

            AddButton(356, 0, 0x200, 0x200, 0, GumpButtonType.Page, 2);

            AddPage(2);

            AddHtmlLocalized(40, 35, 150, 160, 1113947, 0, false, false); // moongate, say the gate's name and "moongate" (eg. "minoc moongate").
                                                                          // <br><br>For Felucca, say "fel" then same rules as above. So "fel minoc mint" 
                                                                          // or "fel minoc moongate".

            AddHtmlLocalized(230, 30, 145, 160, 1113948, 0, false, false); // CITY NAMES:<br>britain, bucs, cove, delucia, haven, jhelom, magincia, minoc, 
                                                                           // moonglow, nujelm, ocllo, papua, serpent, skara, trinsic, vesper, wind, yew, luna, 
                                                                           // umbra, zento, termur, ilshenar

            AddLabel(90, 200, 0, "2"); // toto: get
            AddLabel(250, 200, 0, "3"); // todo : get

            AddButton(0, 0, 0x1FF, 0x1FF, 0, GumpButtonType.Page, 1);
            AddButton(356, 0, 0x200, 0x200, 0, GumpButtonType.Page, 3);

            AddPage(3);

            AddHtmlLocalized(40, 35, 150, 160, 1113949, 0, false, false); // MOONGATE NAMES<br>moonglow, britain, jhelom, yew, minoc, trinsic, skara,
                                                                          // magincia, haven, bucs, vesper, compassion, honesty, honor, humility, justice,
                                                                          // sacrifice, spirituality, valor, chaos, 

            AddHtmlLocalized(230, 30, 145, 160, 1113950, 0, false, false); // luna, umbra, isamu, makoto, homare, termur, eodon<br><br><br>The same teleportation 
                                                                           // rules apply regarding criminal flagging, weight, etc.

            AddLabel(90, 200, 0, "4"); // todo: get
            AddLabel(250, 200, 0, "5"); // todo: get

            AddButton(0, 0, 0x1FF, 0x1FF, 0, GumpButtonType.Page, 2);
		}
	}
}