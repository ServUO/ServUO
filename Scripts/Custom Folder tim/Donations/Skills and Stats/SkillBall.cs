using System;
using Server;
using Server.Items;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;

/************
SkillBall25
SkillBall50
SkillBall75
SkillBall100
SkillBall <number> // " [add SkillBall 1000 "
*************/

namespace Server.Items
{
	public class SkillBall25 : SkillBall
	{
		[Constructable]
		public SkillBall25() : base( 25 )
		{}
		public SkillBall25( Serial serial ) : base( serial ){}
		
		public override void Serialize( GenericWriter writer ) 
		{ base.Serialize( writer ); writer.Write( (int) 0 ); }
		public override void Deserialize( GenericReader reader )
		{ base.Deserialize( reader ); int version = reader.ReadInt(); }
		
	}
	
	public class SkillBall50 : SkillBall
	{
		[Constructable]
		public SkillBall50() : base( 50 )
		{}
		public SkillBall50( Serial serial ) : base( serial ){}
		
		public override void Serialize( GenericWriter writer ) 
		{ base.Serialize( writer ); writer.Write( (int) 0 ); }
		public override void Deserialize( GenericReader reader )
		{ base.Deserialize( reader ); int version = reader.ReadInt(); }
		
	}
	
	public class SkillBall75 : SkillBall
	{
		[Constructable]
		public SkillBall75() : base( 75 )
		{}
		public SkillBall75( Serial serial ) : base( serial ){}
		
		public override void Serialize( GenericWriter writer ) 
		{ base.Serialize( writer ); writer.Write( (int) 0 ); }
		public override void Deserialize( GenericReader reader )
		{ base.Deserialize( reader ); int version = reader.ReadInt(); }
		
	}
	
	public class SkillBall100 : SkillBall
	{
		[Constructable]
		public SkillBall100() : base( 100 )
		{}
		public SkillBall100( Serial serial ) : base( serial ){}
		
		public override void Serialize( GenericWriter writer ) 
		{ base.Serialize( writer ); writer.Write( (int) 0 ); }
		public override void Deserialize( GenericReader reader )
		{ base.Deserialize( reader ); int version = reader.ReadInt(); }
		
	}
	
	public class SkillBall : Item
	{
		private int m_Points;

		[CommandProperty( AccessLevel.GameMaster )]
		public int Points { get { return m_Points; } set { m_Points = value; InvalidateProperties(); } }

		[Constructable]
		public SkillBall() : this( 15, 50 )
		{
		}
		
		[Constructable]
		public SkillBall( int pointsMin, int pointsMax ) : this( Utility.Random( pointsMin, pointsMax - pointsMin ) )
		{
		}

		[Constructable]
		public SkillBall( int points ) : base( 0xE73 )
		{
			LootType = LootType.Blessed;
			Movable = true;
			m_Points = points;
			Name = "a skill ball";
			Hue = 2222;
		}

		public SkillBall( Serial serial ) : base( serial )
		{
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			if ( m_Points > 0 )
				list.Add( "  {0} Points!", m_Points.ToString() );
			else list.Add( " *Empty* " );
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
			writer.Write( (int) m_Points );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
			m_Points = reader.ReadInt();
		}


		public override void OnDoubleClick( Mobile from )
		{
			PlayerMobile m;

			if ( m_Points < 1 )
			{
				from.SendMessage("That is empty!");
			}
			else if ( !IsChildOf( from.Backpack ) )
			{
				from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
			}
			else if ( from.HasGump( typeof( IncreaseSkillsGump ) ) )
			{
				from.SendMessage( "You may not have more than one Skill Gump running." );
			}
			else
			{
				if ( from is PlayerMobile )
				{
					m = from as PlayerMobile;
					m.SendGump( new IncreaseSkillsGump( m, this, true, 0 ) );
				}
			}
		}
	}

	//******************************************
	//**   Here is where the Gump starts     ***
	//******************************************
	public class IncreaseSkillsGump : Gump
	{
		private SkillBall m_Ball;
		private static int m_Points;
		private PlayerMobile m_Mobile;
		private int m_Page;
		private int m_StartPos;
		private static double[] m_StartValue;
		private static Skill m_Skill;

		public IncreaseSkillsGump( PlayerMobile mobile, SkillBall ball, double[] values, int page ) : this( mobile, ball, false, page )
		{
			int zz = Core.TOL? 58: Core.ML? 55: Core.SE? 54: Core.AOS? 52: 49;
			for (int x=0; x<zz; x++)
				m_StartValue[x] = values[x];
		}

		public IncreaseSkillsGump( PlayerMobile mobile, SkillBall ball, bool first, int page ) : base( 50, 50 )
		{
			m_Page = page;
			m_Ball = ball;
			m_Points = m_Ball.Points;
			m_Mobile = mobile;

			if ( first )
			{
				int zz = Core.TOL? 58: Core.ML? 55: Core.SE? 54: Core.AOS? 52: 49;
				m_StartValue = new double[zz];
				m_Page = 0;
				for (int x=0; x<zz; x++)
					m_StartValue[x] = m_Mobile.Skills[x].Value;
			}

			m_Mobile.CloseGump( typeof(IncreaseSkillsGump) );

			AddPage( 0 );

			AddBackground( 0, 0, 476, 440, 0x13BE );

			AddLabel( 10, 7, 2100, "Choose Skills" );

			if ( m_Page > 0 )
			{
				AddButton( 275, 7, 250, 251, 2, GumpButtonType.Reply, 0 ); // Prev Page
			}

			if ( m_Page < 5 )
			{
				AddButton( 275, 395, 252, 253, 3, GumpButtonType.Reply, 0 ); // Next Page
			}

			AddLabel( 160, 7, 2100, "Points Left: " + m_Points.ToString() );

//			We only need this "if" condition if we want to make them use it all up right now.
//			if ( m_Points == 0 )
//			{
				AddButton( 305, 335, 0xFB7, 0xFB9, 1, GumpButtonType.Reply, 0 ); // OK button
//			}

			AddImage( 170, 10, 0x58A );
			
			m_StartPos = m_Page * 11;
			int z = 0;
			int max;
			if( Core.TOL && m_Page == 5 )
				max = 3;
			else if ( m_Page == 4 )
				max = Core.ML? 11: Core.SE? 10: Core.AOS? 8: 5;
			else
				max = 11;

			for ( int i = m_StartPos; i < m_StartPos + max; i++ )
			{
				int y = 20 + ( 30 * ( ++z ) );

				m_Skill = m_Mobile.Skills[i];

				AddLabel( 10, y, 2124, m_Skill.Name.ToString() );

				AddLabel( 170, y, 2100, m_Skill.Value.ToString() );

				if ( CanLowerSkill( m_Skill, i, 1 ) )
					AddButton( 220, y, 0x1519, 0x1519, 1000 + i, GumpButtonType.Reply, 0 ); // Decrease

				if ( CanRaiseSkill( m_Skill, i, 1 ) )
					AddButton( 240, y, 0x151A, 0x151A, 2000 + i, GumpButtonType.Reply, 0 ); // Increase

				if ( CanLowerSkill( m_Skill, i, 5 ) )
					AddButton( 200, y-2, 2229, 2229, 3000 + i, GumpButtonType.Reply, 0 ); // Decrease by 5

				if ( CanRaiseSkill( m_Skill, i, 5 ) )
					AddButton( 256, y-2, 2229, 2229, 4000 + i, GumpButtonType.Reply, 0 ); // Increase by 5
			}
		}

		public bool CanLowerSkill( Skill skill, int pos, int amount )
		{
			if ( skill.Value - amount >= m_StartValue[pos] )
				return true;
			else if ( m_Mobile.AccessLevel >= AccessLevel.GameMaster ) // Why should we limit a GM? hehe
				return true;
			else return false;
		}

		public bool CanRaiseSkill( Skill skill, int pos, int amount )
		{
			if (( m_Points >= amount ) && ( ( skill.Value + amount ) <= m_Mobile.Skills[pos].Cap ))
				return true;
			else if ( m_Mobile.AccessLevel >= AccessLevel.GameMaster ) // Why should we limit a GM? hehe
				return true;
			else return false;
		}

		public override void OnResponse( NetState state, RelayInfo info )
		{
			PlayerMobile from = state.Mobile as PlayerMobile;

			if ( info.ButtonID == 1 ) // "Ok"
			{
				from.CloseGump( typeof( IncreaseSkillsGump ) );
			}
			else if ( info.ButtonID >= 4000 ) // Increase by 5
			{
				from.Skills[info.ButtonID - 4000].Base += 5.0;
				m_Ball.Points -= 5;
				if (m_Ball.Points <= 0) m_Ball.Consume();
				else from.SendGump( new IncreaseSkillsGump( from, m_Ball, m_StartValue, m_Page ) );
			}
			else if ( info.ButtonID >= 3000 ) // Decrease by 5
			{
				from.Skills[info.ButtonID - 3000].Base -= 5.0;
				m_Ball.Points += 5;
				from.SendGump( new IncreaseSkillsGump( from, m_Ball, m_StartValue, m_Page ) );
			}
			else if ( info.ButtonID >= 2000 ) // Increase
			{
				from.Skills[info.ButtonID - 2000].Base += 1.0;
				m_Ball.Points -= 1;
				if (m_Ball.Points <= 0) m_Ball.Consume();
				else from.SendGump( new IncreaseSkillsGump( from, m_Ball, m_StartValue, m_Page ) );
			}
			else if ( info.ButtonID >= 1000 ) // Decrease
			{
				from.Skills[info.ButtonID - 1000].Base -= 1.0;
				m_Ball.Points += 1;
				from.SendGump( new IncreaseSkillsGump( from, m_Ball, m_StartValue, m_Page ) );
			}
			else if ( info.ButtonID == 2 ) // Previous Page
			{
				--m_Page;
				from.SendGump( new IncreaseSkillsGump( from, m_Ball, m_StartValue, m_Page ) );
			}
			else if ( info.ButtonID == 3 ) // Next Page
			{
				++m_Page;
				from.SendGump( new IncreaseSkillsGump( from, m_Ball, m_StartValue, m_Page ) );
			}
			else
				from.CloseGump( typeof( IncreaseSkillsGump ) );
		}
	}
}