//Created by Ashlar, beloved of Morrigan
using System;
using Server;
using Server.Gumps;
using Server.Network;
using Server.Items;
using Server.Mobiles;
using Server.Multis;
using Server.Regions;

namespace Server.Items
{
 	public class TamersLittleHelperGump : Gump
   	{
		private TamersLittleHelper m_TLH;
		private Mobile m_From;

		public TamersLittleHelperGump( Mobile from, TamersLittleHelper tlh ) : base( 25,25 )
		  {
				m_From = from;
				m_TLH = tlh;

				m_From.CloseGump( typeof( TamersLittleHelperGump ) );

         		AddPage( 0 );

				AddBackground( 50, 10, 455, 199, 5054 );
				AddImageTiled( 58, 20, 438, 180, 2624 );
				AddAlphaRegion( 58, 20, 438, 180 );

	 			AddLabel( 225, 25, 88, "Taming Target Menu");

         		AddButton( 75, 50, 4005, 4007, 1, GumpButtonType.Reply, 0 );
	 			AddLabel( 125, 50, 0x486, "Skill 0 - 11" );

         		AddButton( 75, 75, 4005, 4007, 2, GumpButtonType.Reply, 0 );
         		AddLabel( 125, 75, 0x486, "Skill 11 - 23" );

         		AddButton( 75, 100, 4005, 4007, 3, GumpButtonType.Reply, 0 );
         		AddLabel( 125, 100, 0x486, "Skill 23 - 35" );

         		AddButton( 75, 125, 4005, 4007, 4, GumpButtonType.Reply, 0 );
         		AddLabel( 125, 125, 0x486, "Skill 35 - 41" );

         		AddButton( 75, 150, 4005, 4007, 5, GumpButtonType.Reply, 0 );
         		AddLabel( 125, 150, 0x486, "Skill 41 - 47" );

         		AddButton( 75, 175, 4005, 4007, 6, GumpButtonType.Reply, 0 );
        		AddLabel( 125, 175, 0x486, "Skill  47 - 59" );

         		AddButton( 275, 50, 4005, 4007, 7, GumpButtonType.Reply, 0 );
         		AddLabel( 325, 50, 0x486, "Skill  59 - 65" );

         		AddButton( 275, 75, 4005, 4007, 8, GumpButtonType.Reply, 0 );
         		AddLabel( 325, 75, 0x486, "Skill  65 - 70" );

         		AddButton( 275, 100, 4005, 4007, 9, GumpButtonType.Reply, 0 );
         		AddLabel( 325, 100, 0x486, "Skill  70 - 80" );

         		AddButton( 275, 125, 4005, 4007, 10, GumpButtonType.Reply, 0 );
         		AddLabel( 325, 125, 0x486, "Skill  80 - 90" );

         		AddButton( 275, 150, 4005, 4007, 11, GumpButtonType.Reply, 0 );
         		AddLabel( 325, 150, 0x486, "Skill  90 - 100" );

         		AddButton( 275, 175, 4005, 4007, 12, GumpButtonType.Reply, 0 );
         		AddLabel( 325, 175, 0x486, "Skill  100 - 120" );
		}
		public override void OnResponse( NetState state, RelayInfo info )
		{
			if ( info.ButtonID == 1 )
			{
				Mobile TamingMountainGoat = new TamingMountainGoat(m_From);
				TamingMountainGoat.MoveToWorld( m_From.Location, m_From.Map );
				m_TLH.Delete();
			}
			else if ( info.ButtonID == 2 )
			{
				Mobile TamingSheep = new TamingSheep(m_From);
				TamingSheep.MoveToWorld( m_From.Location, m_From.Map );
				m_TLH.Delete();
			}
			else if ( info.ButtonID == 3 )
			{
				Mobile TamingTimberWolf = new TamingTimberWolf(m_From);
				TamingTimberWolf.MoveToWorld( m_From.Location, m_From.Map );
				m_TLH.Delete();
			}
			else if ( info.ButtonID == 4 )
			{
				Mobile TamingBlackBear = new TamingBlackBear(m_From);
				TamingBlackBear.MoveToWorld( m_From.Location, m_From.Map );
				m_TLH.Delete();
			}
			else if ( info.ButtonID == 5 )
			{
				Mobile TamingBrownBear = new TamingBrownBear(m_From);
				TamingBrownBear.MoveToWorld( m_From.Location, m_From.Map );
				m_TLH.Delete();
			}
			else if ( info.ButtonID == 6 )
			{
				Mobile TamingAlligator = new TamingAlligator(m_From);
				TamingAlligator.MoveToWorld( m_From.Location, m_From.Map );
				m_TLH.Delete();
			}
			else if ( info.ButtonID == 7 )
			{
				Mobile TamingGreatHart = new TamingGreatHart(m_From);
				TamingGreatHart.MoveToWorld( m_From.Location, m_From.Map );
				m_TLH.Delete();
			}
			else if ( info.ButtonID == 8 )
			{
				Mobile TamingGrizzlyBear = new TamingGrizzlyBear(m_From);
				TamingGrizzlyBear.MoveToWorld( m_From.Location, m_From.Map );
				m_TLH.Delete();
			}
			else if ( info.ButtonID == 9 )
			{
				Mobile TamingBull = new TamingBull(m_From);
				TamingBull.MoveToWorld( m_From.Location, m_From.Map );
				m_TLH.Delete();
			}
			else if ( info.ButtonID == 10 )
			{
				Mobile TamingGiantToad = new TamingGiantToad(m_From);
				TamingGiantToad.MoveToWorld( m_From.Location, m_From.Map );
				m_TLH.Delete();
			}
			else if ( info.ButtonID == 11 )
			{
				Mobile TamingBakeKitsune = new TamingBakeKitsune(m_From);
				TamingBakeKitsune.MoveToWorld( m_From.Location, m_From.Map );
				m_TLH.Delete();
			}
			else if ( info.ButtonID == 12 )
			{
				Mobile TamingHiryu = new TamingHiryu(m_From);
				TamingHiryu.MoveToWorld( m_From.Location, m_From.Map );
				m_TLH.Delete();
			}
			else
			{
				m_From.SendLocalizedMessage( 502694 ); // Cancelled action.
			}
		}
	}
}