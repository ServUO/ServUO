using System;
using Server;
using Server.Items;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;

namespace Server.Items
{
	public class PowerScrollRewardDeed : Item
	{
		[Constructable]
		public PowerScrollRewardDeed() : base()
		{
			ItemID = 8792;
			LootType = LootType.Blessed;
			Name = "Power Scroll Reward Deed";
			Hue = 1153;
		}

		public PowerScrollRewardDeed( Serial serial ) : base( serial )
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
			if ( !IsChildOf( from.Backpack ) )
			{
				from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
			}
			else if ( from.HasGump( typeof( SelectionGump ) ) )
			{
				from.SendMessage( "You have already opened the Power Scroll selection menu" );
			}
			else
			{
				if ( from is PlayerMobile )
				{
					from.SendGump( new SelectionGump( from, this ) );
				}
			}
		}
	}

	
	public class SelectionGump : Gump
	{
		private PowerScrollRewardDeed m_Ball;
		private Mobile m_Mobile;
		private static Skill m_Skill;

		public SelectionGump( Mobile mobile, PowerScrollRewardDeed ball ) : base( 50, 50 )
		{
			m_Ball = ball;
			m_Mobile = mobile;

			m_Mobile.CloseGump( typeof(SelectionGump) );

			AddPage( 0 );

			AddBackground( 0, 0, 571, 593, 9250 );
			AddLabel(490,559, 0, "zerodowned");
			AddAlphaRegion( 14,16, 542, 563);
			
			AddHtml(146, 26, 50, 20, String.Format("<basefont color=#FFFFFF><small>curr cap"), false, false);
			
			AddHtml(187, 15, 320, 20, String.Format("<basefont color=#FFFFFF><big>120 Power Scroll selection"), false, false);
			
			int xInterval = 0;
			int yInterval = 0;
			int PerColumn = 20;
			
			for ( int i = 0; i < m_Mobile.Skills.Length; ++i, yInterval++ )
			{
				AddButton( 20 + ( xInterval * 180 ), 45 + ( yInterval * 25 ), 5601, 5601, 1 + i, GumpButtonType.Reply, 0 ); 
				AddLabel( 40 + ( xInterval * 180 ), 42 + ( yInterval * 25 ), 1071, m_Mobile.Skills[i].Name.ToString() );
				AddLabel( 165 + ( xInterval * 180 ), 42 + ( yInterval * 25 ), 1071, m_Mobile.Skills[i].Cap.ToString() );
				
				if( i == PerColumn  || i == ((PerColumn * 2) +1) )
				{
					xInterval = (i == ((PerColumn * 2) +1) ? 2 : 1);
					yInterval = -1;
				}
			}
			
			for ( int i = 0; i < 20; ++i )
			{
				AddImageTiled( 20, 65+ ( i * 25 ), 530, 1, 96 );
			}
			
			AddImageTiled( 190, 44, 8, 521, 10150 );
			AddImageTiled( 370, 44, 8, 521, 10150 );
		}

	

		public override void OnResponse( NetState state, RelayInfo info )
		{
			PlayerMobile from = state.Mobile as PlayerMobile;

			if( info.ButtonID > 0 )
			{
				from.AddToBackpack( new PowerScroll( (SkillName)info.ButtonID - 1 , 120 ));
				from.CloseGump( typeof( SelectionGump ) );
				m_Ball.Delete();
			}
			else
				from.CloseGump( typeof( SelectionGump ) );
		}
	}
}