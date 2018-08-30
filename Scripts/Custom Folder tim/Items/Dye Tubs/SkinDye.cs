
using System;
using System.Text;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;

namespace Server.Items
{
	public class SkinDyeBottle : Item
	{
		public override string DefaultName
		{
			get { return "Skin Dye Bottle"; }
		}

		[Constructable]
		public SkinDyeBottle() : base( 0xE26 )
		{
			Weight = 1.0;
			LootType = LootType.Blessed;
            this.Hue = 1153;
		}

		public SkinDyeBottle( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( from.InRange( this.GetWorldLocation(), 1 ) )
			{
				from.CloseGump( typeof( SkinDyeBottleGump ) );
				from.SendGump( new SkinDyeBottleGump( this ) );
				from.SendMessage( "Select a color to dye your skin" );
			}
			else
			{
				from.LocalOverheadMessage( MessageType.Regular, 906, 1019045 ); // I can't reach that.
			}	
					
		}
	}

	public class SkinDyeBottleGump : Gump
	{
		private SkinDyeBottle m_SkinDyeBottle;

		public SkinDyeBottleGump( SkinDyeBottle dye ) : base( 50, 50 )
		{
			m_SkinDyeBottle = dye;

			AddPage( 0 );
			AddBackground( 0, 0, 220, 500, 5054 );
			AddBackground( 10, 10, 200, 480, 3000 );
			AddLabel( 20, 20, 0, "Skin Dye Hue Selector");
			AddLabel( 20, 60, 0, "Select a Hue");
			AddButton( 20, 440, 4005, 4007, 0, GumpButtonType.Reply, 0 );
			AddLabel( 55, 440, 0, "Close");

			AddPage( 1 );
			AddButton( 20,  90, 4005, 4007,  1, GumpButtonType.Reply, 0 );
			AddButton( 20, 110, 4005, 4007,  2, GumpButtonType.Reply, 0 );
			AddButton( 20, 130, 4005, 4007,  3, GumpButtonType.Reply, 0 );
			AddButton( 20, 150, 4005, 4007,  4, GumpButtonType.Reply, 0 );
			AddButton( 20, 170, 4005, 4007,  5, GumpButtonType.Reply, 0 );
			AddButton( 20, 190, 4005, 4007,  6, GumpButtonType.Reply, 0 );
			AddButton( 20, 210, 4005, 4007,  7, GumpButtonType.Reply, 0 );
			AddButton( 20, 230, 4005, 4007,  8, GumpButtonType.Reply, 0 );
			AddButton( 20, 250, 4005, 4007,  9, GumpButtonType.Reply, 0 );
			AddButton( 20, 270, 4005, 4007, 10, GumpButtonType.Reply, 0 );
			AddButton( 20, 290, 4005, 4007, 11, GumpButtonType.Reply, 0 );
			AddButton( 20, 310, 4005, 4007, 12, GumpButtonType.Reply, 0 );
			AddButton( 20, 330, 4005, 4007, 13, GumpButtonType.Reply, 0 );
			AddButton( 20, 350, 4005, 4007, 14, GumpButtonType.Reply, 0 );
			AddButton( 20, 370, 4005, 4007, 15, GumpButtonType.Reply, 0 );
			AddButton( 20, 390, 4005, 4007, 16, GumpButtonType.Reply, 0 );
			AddButton( 20, 410, 4005, 4007, 17, GumpButtonType.Reply, 0 );

			AddLabel( 55,  90, 1152, "Frost Blue");
			AddLabel( 55, 110, 1166, "Bright Melon");
			AddLabel( 55, 130, 1164, "Light Blue");
			AddLabel( 55, 150, 1163, "Greenish Blue");
			AddLabel( 55, 170, 1167, "Purple");
			AddLabel( 55, 190, 1161, "Teal");
			AddLabel( 55, 210, 1160, "Blaze");
			AddLabel( 55, 230, 1165, "Hot Pink");
			AddLabel( 55, 250, 1162, "Shiny Purple");
			AddLabel( 55, 270, 1156, "Blood Red");
			AddLabel( 55, 290, 1155, "Dark Blue");
			AddLabel( 55, 310, 1154, "Olive Green");
			AddLabel( 55, 330, 1153, "Ice Blue");
			AddLabel( 55, 350, 1150, "Ice Green");
			AddLabel( 55, 370, 1149, "Ice White");
			AddLabel( 55, 390, 1171, "Reddish Blue");
			AddLabel( 55, 410, 1174, "Shadow Black");
		}

		public override void OnResponse( NetState from, RelayInfo info )
		{
			if ( m_SkinDyeBottle.Deleted )
				return;

			Mobile m = from.Mobile;
			int[] switches = info.Switches;

			if ( !m_SkinDyeBottle.IsChildOf( m.Backpack ) ) 
			{
				m.SendLocalizedMessage( 1042010 ); //You must have the object in your backpack to use it.
				return;
			}

			switch( info.ButtonID )
			{

			case 0: from.Mobile.SendMessage( "You decide not to hue your skin" ); break;

			case 1: from.Mobile.SendMessage( "You dye your skin Frost Blue" );
				from.Mobile.Hue = 1152;
				m_SkinDyeBottle.Delete();
				m.PlaySound( 0x4E ); break;

			case 2: from.Mobile.SendMessage( "You dye your skin Bright Melon" );
				from.Mobile.Hue = 1167;
				m_SkinDyeBottle.Delete();
				m.PlaySound( 0x4E ); break;

			case 3: from.Mobile.SendMessage( "You dye your skin Light Blue" );
				from.Mobile.Hue = 1165;
				m_SkinDyeBottle.Delete();
				m.PlaySound( 0x4E ); break;

			case 4: from.Mobile.SendMessage( "You dye your skin Greenish Blue" );
				from.Mobile.Hue = 1164;
				m_SkinDyeBottle.Delete();
				m.PlaySound( 0x4E ); break;

			case 5: from.Mobile.SendMessage( "You dye your skin Purple" );
				from.Mobile.Hue = 1168;
				m_SkinDyeBottle.Delete();
				m.PlaySound( 0x4E ); break;

			case 6: from.Mobile.SendMessage( "You dye your skin Teal" );
				from.Mobile.Hue = 1162;
				m_SkinDyeBottle.Delete();
				m.PlaySound( 0x4E ); break;

			case 7: from.Mobile.SendMessage( "You dye your skin Blaze" );
				from.Mobile.Hue = 1161;
				m_SkinDyeBottle.Delete();
				m.PlaySound( 0x4E ); break;

			case 8: from.Mobile.SendMessage( "You dye your skin Hot Pink" );
				from.Mobile.Hue = 1166;
				m_SkinDyeBottle.Delete();
				m.PlaySound( 0x4E ); break;

			case 9: from.Mobile.SendMessage( "You dye your skin Shiny Purple" );
				from.Mobile.Hue = 1163;
				m_SkinDyeBottle.Delete();
				m.PlaySound( 0x4E ); break;

			case 10: from.Mobile.SendMessage( "You dye your skin Blood Red" );
				from.Mobile.Hue = 1157;
				m_SkinDyeBottle.Delete();
				m.PlaySound( 0x4E ); break;

			case 11: from.Mobile.SendMessage( "You dye your skin Dark Blue" );
				from.Mobile.Hue = 1156;
				m_SkinDyeBottle.Delete();
				m.PlaySound( 0x4E ); break;

			case 12: from.Mobile.SendMessage( "You dye your skin Olive Green" );
				from.Mobile.Hue = 1155;
				m_SkinDyeBottle.Delete();
				m.PlaySound( 0x4E ); break;

			case 13: from.Mobile.SendMessage( "You dye your skin Ice Blue" );
				from.Mobile.Hue = 1154;
				m_SkinDyeBottle.Delete();
				m.PlaySound( 0x4E ); break;

			case 14: from.Mobile.SendMessage( "You dye your skin Ice Green" );
				from.Mobile.Hue = 1151;
				m_SkinDyeBottle.Delete();
				m.PlaySound( 0x4E ); break;

			case 15: from.Mobile.SendMessage( "You dye your skin Ice White" );
				from.Mobile.Hue = 1150;
				m_SkinDyeBottle.Delete();
				m.PlaySound( 0x4E ); break;

			case 16: from.Mobile.SendMessage( "You dye your skin Reddish Blue" );
				from.Mobile.Hue = 1172;
				m_SkinDyeBottle.Delete();
				m.PlaySound( 0x4E ); break;

			case 17: from.Mobile.SendMessage( "You dye your skin Shadow Black" );
				from.Mobile.Hue = 1175;
				m_SkinDyeBottle.Delete();
				m.PlaySound( 0x4E ); break;

			}
		}
	}
}