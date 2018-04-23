//Scripted by Energy
//ICQ 411-144-844

using System;
using Server;
using Server.Mobiles;
using Server.Gumps;
using Server.Network;
using Server.Items;
using Server.Accounting;
using Server.Misc;

namespace Server.Gumps
{
	public class PrizeGump : Gump
	{



        public PrizeGump()
            : base(300, 15)
		{



			AddPage( 0 );

			AddBackground( 98, 139, 451, 330, 3000 );
			AddImage(446, 144, 5536);
            //AddImage(100, 200, 30);

            AddLabel( 435, 445, 172, "Script by Energy");
            AddLabel( 106, 146, 172, "Ultima Online Prize Stone" );

			AddButton( 196, 221, 4005, 4007, 1, GumpButtonType.Reply, 0 );
			AddLabel( 236, 223, 54, "Death Shroud" );

			AddButton( 110, 282, 4005, 4007, 2, GumpButtonType.Reply, 0 );
			AddLabel( 150, 283, 54, "Dryad Bow" );

			AddButton( 110, 312, 4005, 4007, 3, GumpButtonType.Reply, 0 );
            AddLabel( 150, 312, 54, "ArmorOfFortune");

			AddButton( 110, 343, 4005, 4007, 4, GumpButtonType.Reply, 0 );
			AddLabel( 150, 342, 54, "Zyronic Claw" );

			AddButton( 110, 373, 4005, 4007, 5, GumpButtonType.Reply, 0 );
			AddLabel( 150, 370, 54, "Titans Hammer" );

			AddButton( 110, 403, 4005, 4007, 6, GumpButtonType.Reply, 0 );
			AddLabel( 150, 401, 54, "Arcane Shield" );

			AddButton( 110, 433, 4005, 4007, 7, GumpButtonType.Reply, 0 );
			AddLabel( 150, 434, 54, "Tunic Of Fire" );

			AddButton( 287, 282, 4005, 4007, 8, GumpButtonType.Reply, 0 );
			AddLabel( 324, 286, 54, "Jackals Collar" );

			AddButton( 287, 312, 4005, 4007, 9, GumpButtonType.Reply, 0 );
			AddLabel( 324, 314, 54, "Heart Of The Lion" );

			AddButton( 287, 343, 4005, 4007, 10, GumpButtonType.Reply, 0 );
			AddLabel( 324, 345, 54, "Taksmaster" );

			AddButton( 287, 373, 4005, 4007, 11, GumpButtonType.Reply, 0 );
			AddLabel( 324, 375, 0x34, "Ring Of The Vile" );

			AddButton( 287, 403, 4005, 4007, 12, GumpButtonType.Reply, 0 );
			AddLabel( 323, 405, 54, "Spirit Of The Totem" );

			AddButton( 287, 433, 4005, 4007, 13, GumpButtonType.Reply, 0 );
			AddLabel( 324, 433, 0x34, "Fey Leggings" );


		
		}

 	public override void OnResponse(NetState sender, RelayInfo info)
	{
		Mobile m = sender.Mobile;

		//if (m == null)
		//return;

			switch( info.ButtonID ){
				case 0: m.CloseGump( typeof( PrizeGump ) );
					break;
                    
				
				case 1:
			        
                    m.AddToBackpack(new HoodedShroudOfShadows()); 
					m.SendMessage("Death Shroud in your backpack"); 
                    m.SendGump (new PrizeGump()); 
					break;
                case 2:
			        
                    m.AddToBackpack(new TheDryadBow());
                    m.SendMessage("The Dryad Bow in your backpack");
                    m.SendGump (new PrizeGump());
					break;


				case 3:
			        
                    m.AddToBackpack(new ArmorOfFortune());
                    m.SendMessage("Armor Of Fortune in your backpack");
                    m.SendGump (new PrizeGump());
					break;
				case 4:
                    
                    m.AddToBackpack(new ZyronicClaw());
                    m.SendMessage("Zyronic Claw in your backpack");
                    m.SendGump (new PrizeGump());
					break;

				case 5:
                    
                    m.AddToBackpack(new TitansHammer());
                    m.SendMessage("Titans Hammer in your backpack");
                    m.SendGump (new PrizeGump());
					break;

				case 6:
                    
                    m.AddToBackpack(new ArcaneShield());
                    m.SendMessage("ArcaneShield in your backpack");
                    m.SendGump (new PrizeGump());
					break;

				case 7:
                    
                    m.AddToBackpack(new TunicOfFire());
                    m.SendMessage("TunicOfFire in your backpack");
                    m.SendGump (new PrizeGump());
					break;

				case 8:
                    
                    m.AddToBackpack(new JackalsCollar());
                    m.SendMessage("JackalsCollar in your backpack");
                    m.SendGump (new PrizeGump());
					break;

				case 9:
                    
                    m.AddToBackpack(new HeartOfTheLion(1000));
                    m.SendMessage("Heart Of The Lion in your backpack");
                    m.SendGump (new PrizeGump());
					break;

				case 10:
                    
                    m.AddToBackpack(new TheTaskmaster());
                    m.SendMessage("The Taskmaster in your backpack");
                    m.SendGump (new PrizeGump());
					break;


				case 11:
                    
                    m.AddToBackpack(new RingOfTheVile());
                    m.SendMessage("Ring Of The Vile in your backpack");
                    m.SendGump (new PrizeGump());
					break;

				case 12:
                    
                    m.AddToBackpack(new SpiritOfTheTotem());
                    m.SendMessage("Spirit Of The Totem in your backpack");
                    m.SendGump (new PrizeGump());
					break;

				case 13:
                    
                    m.AddToBackpack(new FeyLeggings());
                    m.SendMessage("Fey Leggings in your backpack");
                    m.SendGump (new PrizeGump());
					break;
			}

	}
			

		


	}
} //Конец!

