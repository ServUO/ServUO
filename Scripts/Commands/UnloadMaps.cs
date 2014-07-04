// Engine r93
#define RunUo2_0
using System;
using System.Collections.Generic;
using Server;
using Server.Gumps;
using Server.Network;
using Server.Commands;

namespace Server.Gumps
{
    public class UnloadCurrentGump : Gump
    {
        Mobile caller;

        public static void Initialize()
        {
#if(RunUo2_0)
            CommandSystem.Register("UnloadCurrent", AccessLevel.Administrator, new CommandEventHandler(UnloadCurrent_OnCommand));
#else
            Register("UnloadCurrent", AccessLevel.Administrator, new CommandEventHandler(UnloadCurrent_OnCommand));
#endif
        }

        [Usage("UnloadCurrent")]
        [Description("Unload PremiumSpawners around the world with a gump.")]
        public static void UnloadCurrent_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            if (from.HasGump(typeof(UnloadCurrentGump)))
                from.CloseGump(typeof(UnloadCurrentGump));
            from.SendGump(new UnloadCurrentGump(from));
        }

        public UnloadCurrentGump(Mobile from) : this()
        {
            caller = from;
        }

		public void AddBlackAlpha( int x, int y, int width, int height )
		{
			AddImageTiled( x, y, width, height, 2624 );
			AddAlphaRegion( x, y, width, height );
		}

        public UnloadCurrentGump() : base( 30, 30 )
        {
            this.Closable=true;
			this.Disposable=true;
			this.Dragable=true;
			AddPage(1);
			AddBackground(58, 22, 474, 540, 9200);
			AddImage(305, 306, 1418); // Castle
			AddBlackAlpha(66, 30, 458, 33);
			AddLabel(213, 37, 52, @"SELECT MAPS TO UNLOAD");
			AddBlackAlpha(66, 87, 239, 447);
			AddLabel(69, 67, 200, @"DUNGEONS");
			AddLabel(69, 90, 52, @"Blighted Grove");
			AddLabel(69, 112, 52, @"Britain Sewer");
			AddLabel(69, 133, 52, @"Covetous");
			AddLabel(69, 154, 52, @"Deceit");
			AddLabel(69, 174, 52, @"Despise");
			AddLabel(69, 196, 52, @"Destard");
			AddLabel(69, 217, 52, @"Fire");
			AddLabel(69, 237, 52, @"Graveyards");
			AddLabel(69, 258, 52, @"Hythloth");
			AddLabel(69, 280, 52, @"Ice");
			AddLabel(69, 301, 52, @"Khaldun");
			AddLabel(69, 322, 52, @"Orc Caves");
			AddLabel(69, 343, 52, @"Painted Caves");
			AddLabel(69, 363, 52, @"Palace of Paroxysmus");
			AddLabel(69, 384, 52, @"Prism of Light");
			AddLabel(69, 405, 52, @"Sanctuary");
			AddLabel(69, 427, 52, @"Shame");
			AddLabel(69, 448, 52, @"Solen Hive");
			AddLabel(69, 469, 52, @"Terathan Keep");
			AddLabel(69, 489, 52, @"Trinsic Passage");
			AddLabel(69, 510, 52, @"Wrong");
			AddLabel(194, 66, 200, @"Felucca");
			AddCheck(210, 91, 210, 211, true, 201);
			AddCheck(210, 112, 210, 211, true, 202);
			AddCheck(210, 133, 210, 211, true, 203);
			AddCheck(210, 154, 210, 211, true, 204);
			AddCheck(210, 175, 210, 211, true, 205);
			AddCheck(210, 196, 210, 211, true, 206);
			AddCheck(210, 217, 210, 211, true, 207);
			AddCheck(210, 238, 210, 211, true, 208);
			AddCheck(210, 259, 210, 211, true, 209);
			AddCheck(210, 280, 210, 211, true, 210);
			AddCheck(210, 301, 210, 211, true, 228);
			AddCheck(210, 322, 210, 211, true, 212);
			AddCheck(210, 343, 210, 211, true, 214);
			AddCheck(210, 364, 210, 211, true, 215);
			AddCheck(210, 385, 210, 211, true, 216);
			AddCheck(210, 406, 210, 211, true, 217);
			AddCheck(210, 427, 210, 211, true, 219);
			AddCheck(210, 448, 210, 211, true, 220);
			AddCheck(210, 469, 210, 211, true, 221);
			AddCheck(210, 490, 210, 211, true, 224);
			AddCheck(210, 511, 210, 211, true, 227);
			AddLabel(250, 66, 200, @"Trammel");
			AddCheck(268, 91, 210, 211, true, 101);
			AddCheck(268, 112, 210, 211, true, 102);
			AddCheck(268, 133, 210, 211, true, 103);
			AddCheck(268, 154, 210, 211, true, 104);
			AddCheck(268, 175, 210, 211, true, 105);
			AddCheck(268, 196, 210, 211, true, 106);
			AddCheck(268, 217, 210, 211, true, 107);
			AddCheck(268, 238, 210, 211, true, 108);
			AddCheck(268, 259, 210, 211, true, 109);
			AddCheck(268, 280, 210, 211, true, 110);
			//There is no Khaldun in Trammel (ID 128 reserved)
			AddCheck(268, 322, 210, 211, true, 112);
			AddCheck(268, 343, 210, 211, true, 114);
			AddCheck(268, 364, 210, 211, true, 115);
			AddCheck(268, 385, 210, 211, true, 116);
			AddCheck(268, 406, 210, 211, true, 117);
			AddCheck(268, 427, 210, 211, true, 119);
			AddCheck(268, 448, 210, 211, true, 120);
			AddCheck(268, 469, 210, 211, true, 121);
			AddCheck(268, 490, 210, 211, true, 124);
			AddCheck(268, 511, 210, 211, true, 127);
			AddBlackAlpha(311, 87, 213, 70);
			AddLabel(315, 67, 200, @"TOWNS");
			AddLabel(315, 91, 52, @"Animals");
			AddLabel(315, 112, 52, @"People (*)");
			AddLabel(315, 133, 52, @"Vendors");
			AddLabel(413, 66, 200, @"Felucca");
			AddCheck(429, 91, 210, 211, true, 222);
			AddCheck(429, 112, 210, 211, true, 223);
			AddCheck(429, 133, 210, 211, true, 225);
			AddLabel(469, 66, 200, @"Trammel");
			AddCheck(487, 91, 210, 211, true, 122);
			AddCheck(487, 112, 210, 211, true, 123);
			AddCheck(487, 133, 210, 211, true, 125);
			AddBlackAlpha(311, 183, 213, 114);
			AddLabel(315, 162, 200, @"OUTDOORS");
			AddLabel(316, 187, 52, @"Animals");
			AddLabel(316, 207, 52, @"Lost Lands");
			AddLabel(316, 229, 52, @"Spawns");
			AddLabel(316, 249, 52, @"Reagents");
			AddLabel(316, 270, 52, @"Sea Life");
			AddLabel(413, 162, 200, @"Felucca");
			AddCheck(429, 187, 210, 211, true, 226);
			AddCheck(429, 208, 210, 211, true, 211);
			AddCheck(429, 229, 210, 211, true, 213);
			AddCheck(429, 250, 210, 211, true, 229);
			AddCheck(429, 271, 210, 211, true, 218);
			AddLabel(469, 162, 200, @"Trammel");
			AddCheck(487, 187, 210, 211, true, 126);
			AddCheck(487, 208, 210, 211, true, 111);
			AddCheck(487, 229, 210, 211, true, 113);
			AddCheck(487, 250, 210, 211, true, 129);
			AddCheck(487, 271, 210, 211, true, 118);
			AddLabel(316, 305, 200, @"(*) Escortables, Hireables,");
			AddLabel(316, 324, 200, @"Town Criers, Order and Chaos");
			AddLabel(316, 344, 200, @"guards etc.");
			// END
			AddLabel(361, 453, 52, @"Page: 1/2"); //Page
			AddButton(423, 455, 5601, 5605, 0, GumpButtonType.Page, 2); // Change Page

			//PAGE 2
			AddPage(2);
			AddBackground(58, 22, 474, 540, 9200);
			AddImage(305, 306, 1418); // Castle
			AddBlackAlpha(66, 30, 458, 33);
			AddLabel(213, 37, 52, @"SELECT MAPS TO SPAWN");
			AddBlackAlpha(66, 87, 174, 300);
			AddLabel(74, 67, 200, @"ILSHENAR");
			AddLabel(74, 90, 52, @"Ancient Lair");
			AddLabel(74, 112, 52, @"Ankh");
			AddLabel(74, 133, 52, @"Blood");
			AddLabel(74, 154, 52, @"Exodus");
			AddLabel(74, 174, 52, @"Mushroom");
			AddLabel(74, 196, 52, @"Outdoors");
			AddLabel(74, 217, 52, @"Ratman Cave");
			AddLabel(74, 237, 52, @"Rock");
			AddLabel(74, 258, 52, @"Sorcerers");
			AddLabel(74, 280, 52, @"Spectre");
			AddLabel(74, 301, 52, @"Towns");
			AddLabel(74, 322, 52, @"Twisted Weald");
			AddLabel(74, 343, 52, @"Vendors");
			AddLabel(74, 363, 52, @"Wisp");
			AddCheck(215, 91, 210, 211, true, 301);
			AddCheck(215, 112, 210, 211, true, 302);
			AddCheck(215, 133, 210, 211, true, 303);
			AddCheck(215, 154, 210, 211, true, 304);
			AddCheck(215, 175, 210, 211, true, 305);
			AddCheck(215, 196, 210, 211, true, 306);
			AddCheck(215, 217, 210, 211, true, 307);
			AddCheck(215, 238, 210, 211, true, 308);
			AddCheck(215, 259, 210, 211, true, 309);
			AddCheck(215, 280, 210, 211, true, 310);
			AddCheck(215, 301, 210, 211, true, 311);
			AddCheck(215, 322, 210, 211, true, 314);
			AddCheck(215, 343, 210, 211, true, 312);
			AddCheck(215, 364, 210, 211, true, 313);
			AddBlackAlpha(66, 414, 174, 133);
			AddLabel(74, 393, 200, @"TOKUNO");
			AddLabel(74, 416, 52, @"Fan Dancers Dojo");
			AddLabel(74, 438, 52, @"Outdoors");
			AddLabel(74, 459, 52, @"Towns Life");
			AddLabel(74, 480, 52, @"Vendors");
			AddLabel(74, 500, 52, @"Wild Life");
			AddLabel(74, 522, 52, @"Yomutso Mines");
			AddCheck(215, 417, 210, 211, true, 501);
			AddCheck(215, 438, 210, 211, true, 502);
			AddCheck(215, 459, 210, 211, true, 503);
			AddCheck(215, 480, 210, 211, true, 504);
			AddCheck(215, 501, 210, 211, true, 505);
			AddCheck(215, 522, 210, 211, true, 506);
			AddBlackAlpha(246, 87, 174, 178);
			AddLabel(253, 67, 200, @"MALAS");
			AddLabel(253, 90, 52, @"Bedlam");
			AddLabel(253, 112, 52, @"Citadel");
			AddLabel(253, 133, 52, @"Doom");
			AddLabel(253, 154, 52, @"Labyrinth");
			AddLabel(253, 174, 52, @"North (*)");
			AddLabel(253, 196, 52, @"Orc Forts");
			AddLabel(253, 217, 52, @"South (*)");
			AddLabel(253, 238, 52, @"Vendors");
			AddCheck(394, 91, 210, 211, true, 408);
			AddCheck(394, 112, 210, 211, true, 406);
			AddCheck(394, 133, 210, 211, true, 401);
			AddCheck(394, 154, 210, 211, true, 407);
			AddCheck(394, 175, 210, 211, true, 402);
			AddCheck(394, 196, 210, 211, true, 403);
			AddCheck(394, 217, 210, 211, true, 404);
			AddCheck(394, 238, 210, 211, true, 405);
			AddLabel(428, 91, 200, @"(*) Wild");
			AddLabel(428, 109, 200, @"Animals and");
			AddLabel(428, 129, 200, @"monsters.");
			AddBlackAlpha(246, 292, 174, 117);
			AddLabel(253, 272, 200, @"TER MUR");
			AddLabel(253, 295, 52, @"Abyss");
			AddLabel(253, 314, 52, @"TerMur");
			AddLabel(253, 337, 52, @"Underworld");
			AddLabel(253, 358, 52, @"Vendors");
			AddCheck(394, 296, 210, 211, true, 601);
			AddCheck(394, 315, 210, 211, true, 602);
			AddCheck(394, 338, 210, 211, true, 603);
			AddCheck(394, 359, 210, 211, true, 604);
			//END
			AddLabel(381, 453, 52, @"Page: 2/2"); //Page
			AddButton(361, 455, 5603, 5607, 0, GumpButtonType.Page, 1); //Change Page
			AddButton(282, 452, 240, 239, 1, GumpButtonType.Reply, 0); // Apply
        }

		public static void UnloadThis( Mobile from, List<int> ListSwitches, int switche )
		{
			string prefix = Server.Commands.CommandSystem.Prefix;

			if( ListSwitches.Contains( switche ) == true )
				CommandSystem.Handle( from, String.Format( "{0}Spawngen unload {1}", prefix, switche ) );
		}

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;

            switch(info.ButtonID)
            {
                case 0: //Closed or Cancel
				{
					break;
				}
				default:
				{
					// Make sure that the APPLY button was pressed
					if( info.ButtonID == 1 )
					{
						// Get the array of switches selected
						List<int> Selections = new List<int>( info.Switches );

						//TRAMMEL
						// DUNGEONS
						UnloadThis(from, Selections, 101);
						UnloadThis(from, Selections, 102);
						UnloadThis(from, Selections, 103);
						UnloadThis(from, Selections, 104);
						UnloadThis(from, Selections, 105);
						UnloadThis(from, Selections, 106);
						UnloadThis(from, Selections, 107);
						UnloadThis(from, Selections, 108);
						UnloadThis(from, Selections, 109);
						UnloadThis(from, Selections, 110);
						//There is no Khaldun (118)
						UnloadThis(from, Selections, 112);
						UnloadThis(from, Selections, 114);
						UnloadThis(from, Selections, 115);
						UnloadThis(from, Selections, 116);
						UnloadThis(from, Selections, 117);
						UnloadThis(from, Selections, 119);
						UnloadThis(from, Selections, 120);
						UnloadThis(from, Selections, 121);
						UnloadThis(from, Selections, 124);
						UnloadThis(from, Selections, 127);
						//TOWNS
						UnloadThis(from, Selections, 122);
						UnloadThis(from, Selections, 123);
						UnloadThis(from, Selections, 125);
						//OUTDOORS
						UnloadThis(from, Selections, 126);
						UnloadThis(from, Selections, 111);
						UnloadThis(from, Selections, 113);
						UnloadThis(from, Selections, 129);
						UnloadThis(from, Selections, 118);

						//FELUCCA
						// DUNGEONS
						UnloadThis(from, Selections, 201);
						UnloadThis(from, Selections, 202);
						UnloadThis(from, Selections, 203);
						UnloadThis(from, Selections, 204);
						UnloadThis(from, Selections, 205);
						UnloadThis(from, Selections, 206);
						UnloadThis(from, Selections, 207);
						UnloadThis(from, Selections, 208);
						UnloadThis(from, Selections, 209);
						UnloadThis(from, Selections, 210);
						UnloadThis(from, Selections, 228);
						UnloadThis(from, Selections, 212);
						UnloadThis(from, Selections, 214);
						UnloadThis(from, Selections, 215);
						UnloadThis(from, Selections, 216);
						UnloadThis(from, Selections, 217);
						UnloadThis(from, Selections, 219);
						UnloadThis(from, Selections, 220);
						UnloadThis(from, Selections, 221);
						UnloadThis(from, Selections, 224);
						UnloadThis(from, Selections, 227);
						//TOWNS
						UnloadThis(from, Selections, 222);
						UnloadThis(from, Selections, 223);
						UnloadThis(from, Selections, 225);
						//OUTDOORS
						UnloadThis(from, Selections, 226);
						UnloadThis(from, Selections, 211);
						UnloadThis(from, Selections, 213);
						UnloadThis(from, Selections, 229);
						UnloadThis(from, Selections, 218);
						
						//ILSHENAR
						UnloadThis(from, Selections, 301);
						UnloadThis(from, Selections, 302);
						UnloadThis(from, Selections, 303);
						UnloadThis(from, Selections, 304);
						UnloadThis(from, Selections, 305);
						UnloadThis(from, Selections, 306);
						UnloadThis(from, Selections, 307);
						UnloadThis(from, Selections, 308);
						UnloadThis(from, Selections, 309);
						UnloadThis(from, Selections, 310);
						UnloadThis(from, Selections, 311);
						UnloadThis(from, Selections, 314);
						UnloadThis(from, Selections, 312);
						UnloadThis(from, Selections, 313);
						
						//MALAS
						UnloadThis(from, Selections, 408);
						UnloadThis(from, Selections, 406);
						UnloadThis(from, Selections, 401);
						UnloadThis(from, Selections, 407);
						UnloadThis(from, Selections, 402);
						UnloadThis(from, Selections, 403);
						UnloadThis(from, Selections, 404);
						UnloadThis(from, Selections, 405);
						
						//TOKUNO
						UnloadThis(from, Selections, 501);
						UnloadThis(from, Selections, 502);
						UnloadThis(from, Selections, 503);
						UnloadThis(from, Selections, 504);
						UnloadThis(from, Selections, 505);
						UnloadThis(from, Selections, 506);
						
						//TER MUR
						UnloadThis(from, Selections, 601);
						UnloadThis(from, Selections, 602);
						UnloadThis(from, Selections, 603);
						UnloadThis(from, Selections, 604);
						
						from.Say( "SPAWN UNLOAD COMPLETED" );
					}
					break;
				}
            }
        }
    }
}