using System;
using System.Collections;
using Server;
using Server.Prompts;
using Server.Mobiles;
using Server.Network;
using Server.Gumps;
using Server.Items;

namespace Server.Items
{ 
	public class StatBall : Item 
	{ 
		[Constructable] 
		public StatBall() : base( 0x186D ) 
		{ 
			Movable = true;
           	Weight = 1.0; 
			Name = "a stat ball";
			LootType = LootType.Blessed;
		} 

		public override void OnDoubleClick( Mobile from ) 
		{ 
			if ( !IsChildOf( from.Backpack ) ) 
			{
	  			from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
				return;
			}
			else if ( from is PlayerMobile )
			{
				from.SendGump( new StatBallGump( (PlayerMobile)from, this ) );
			}
		} 
		
		public override bool DisplayLootType{ get{ return false; } }

		public StatBall( Serial serial ) : base( serial ) 
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
	} 
}

namespace Server.Items
{
	public class StatBallGump : Gump
	{
		private PlayerMobile m_From;
		private StatBall m_Ball;
		
		private int str;
		private int dex;
		private int intel;
		
//	Uncomment the line below to use a defined number instead of the standard Stat Cap
		private int Cap;
				
		public StatBallGump( PlayerMobile from, StatBall ball ): base( 50, 50 )
		{
			m_From = from;
			m_Ball = ball;
			
			this.Closable=true;
			this.Disposable=true;
			this.Dragable=true;
			this.Resizable=false;
			this.AddPage(0);
			this.AddBackground(50, 50, 437, 215, 2620);
			this.AddLabel(200, 67, 1160, "Stat Ball Selection");
			this.AddLabel(114, 96, 1160, "Choose your Strength, Dexterity, and Intelligence");
			this.AddLabel(84, 156, 1152, "STR");
			this.AddLabel(228, 156, 1152, "DEX");
			this.AddLabel(368, 156, 1152, "INT");
			this.AddTextEntry(134, 156, 50, 20, 1359, 0, "100");
			this.AddTextEntry(278, 156, 50, 20, 1359, 1, "100");
			this.AddTextEntry(411, 156, 50, 20, 1359, 2, "100");
			this.AddButton(405, 221, 238, 240, 4, GumpButtonType.Reply, 0);
			
//  Swap these comment lines if using custom Cap variable.  Then set the number in the gump display here
//  Remember to comment out ALL 3 lines, and uncomment the 1 line.
//			this.AddLabel(114, 200, 1152, "* Stat totals should equal ");
//			this.AddLabel(282, 200, 1152, m_From.StatCap.ToString() );
//			this.AddLabel(313, 200, 1152, "*");
			this.AddLabel(114, 200, 1152, "* Stat totals should equal 300 *");

		}

		public override void OnResponse( NetState sender, RelayInfo info )
		{
			if ( m_Ball.Deleted )
				return;

//	Uncomment and change the 200 to the maximum value that people can set for the 3 stats combined
			Cap = 300;
						
            TextRelay s = info.GetTextEntry( 0 );
            try
            {
           		 str = Convert.ToInt32(s.Text);
            }
            catch
            {
                 m_From.SendMessage("Bad strength entry. A number was expected.");
            }
            
            TextRelay d = info.GetTextEntry( 1 );
            try
            {
           		 dex = Convert.ToInt32(d.Text);
            }
            catch
            {
                 m_From.SendMessage("Bad dexterity entry. A number was expected.");
            }
            
            TextRelay i = info.GetTextEntry( 2 );
            try
            {
           		 intel = Convert.ToInt32(i.Text);
            }
            catch
            {
                 m_From.SendMessage("Bad intelligence entry. A number was expected.");
            }
			
			if ( str > 0 && dex > 0 && intel > 0 )
			{
//	Uncomment the line line below, and add a comment to the line under it to use a defined number instead of the standard Stat Cap
				if ( ( ( str + dex + intel ) > Cap ) || ( ( str + dex + intel ) < Cap ) || ( str < 10 ) || ( dex < 10 ) || ( intel < 10 ) )
//				if ( ( ( str + dex + intel ) > m_From.StatCap ) || ( ( str + dex + intel ) < m_From.StatCap ) || ( str < 10 ) || ( dex < 10 ) || ( intel < 10 ) )
					m_From.SendMessage( "Your choice totals are invalid.  Please try again!" );
					
				else
				{
					m_From.RawStr = str;
					m_From.RawDex = dex;
					m_From.RawInt = intel;
			
					m_Ball.Delete();
				}
			}
		}				
	}
}
