using System;
using System.Collections;
using Server;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Engines.PartySystem;

namespace Server.Items
{
	public class PartyTollGate : Moongate
	{
		private Type m_TollItem;
		private int m_TollAmount;

		[CommandProperty(AccessLevel.GameMaster)]
		public Type TollItem
		{
			get { return m_TollItem; }
			set { m_TollItem = value; }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int TollAmount
		{
			get { return m_TollAmount; }
			set { m_TollAmount = value; }
		}

		[Constructable]
		public PartyTollGate() : this( Point3D.Zero, null )
		{
		}

		public PartyTollGate( Point3D target, Map targetMap ) : base( target, targetMap )
		{
			Name = "a party toll gate";
			Movable = false;
			Light = LightType.Circle300;
		}

		public PartyTollGate( Serial serial ) : base( serial )
		{
		}

		public override void OnSingleClick(Mobile from)
		{
			if ( m_TollItem != null && m_TollAmount > 0 )
			{
				string tolabel = String.Format("\nToll Type: {0}\nToll Amount: {1}",m_TollItem.Name,m_TollAmount);
				LabelTo( from, tolabel );
			}
			base.OnSingleClick( from );
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			list.Add(1060658,"{0}\t{1}","Toll Type",m_TollItem);
			list.Add(1060659,"{0}\t{1}","Toll Amount",m_TollAmount);
		}

		public bool IsActive()
		{
			return ( TargetMap != null && TargetMap != Map.Internal && Target != new Point3D( 0,0,0 ) && m_TollItem != null && m_TollAmount > 0 );
		}

		public override void UseGate( Mobile m )
		{
			if ( IsActive() )
			{
				if (m.Backpack.ConsumeTotal(m_TollItem,m_TollAmount,true))
				{
					Party p = Party.Get( m );

					if ( p != null )
					{
						for ( int i = 0; i < p.Members.Count; ++i )
						{
							PartyMemberInfo pmi = (PartyMemberInfo)p.Members[i];
							Mobile member = pmi.Mobile;

							if ( member != m && member.Map == this.Map && member.InRange( this, 3 ) )
							{
								member.CloseGump( typeof( PartyGump ) );
								member.SendGump( new PartyGump( m, member, this ) );
							}
						}
					}

					Teleport( m );
				}
				else
					m.SendMessage("You lack the required toll in your bag to travel forth.");
			}
			else
				m.SendMessage( "This moongate does not seem to go anywhere." );
		}

		public void Teleport( Mobile m )
		{
			BaseCreature.TeleportPets( m, Target, TargetMap);

			m.Map = TargetMap;
			m.Location = Target;

			m.PlaySound( 0x1FE );
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version

			writer.Write( m_TollItem.ToString() );
			writer.Write( m_TollAmount );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			m_TollItem = ScriptCompiler.FindTypeByFullName(reader.ReadString());
			m_TollAmount = reader.ReadInt();
		}
	}

	public class PartyGump : Gump
	{
		private Mobile m_Leader;
		private Mobile m_Member;
		private PartyTollGate m_Gate;

		public PartyGump( Mobile leader, Mobile member, PartyTollGate gate) : base( 150, 50 )
		{
			m_Leader = leader;
			m_Member = member;
			m_Gate = gate;

			Closable = false;

			AddPage( 0 );

			AddImage( 0, 0, 3600 );

			AddImageTiled( 0, 14, 15, 200, 3603 );
			AddImageTiled( 380, 14, 14, 200, 3605 );
			AddImage( 0, 201, 3606 );
			AddImageTiled( 15, 201, 370, 16, 3607 );
			AddImageTiled( 15, 0, 370, 16, 3601 );
			AddImage( 380, 0, 3602 );
			AddImage( 380, 201, 3608 );
			AddImageTiled( 15, 15, 365, 190, 2624 );

			AddRadio( 30, 140, 9727, 9730, true, 1 );
			AddHtmlLocalized( 65, 145, 300, 25, 1050050, 0x7FFF, false, false ); // Yes, let's go!

			AddRadio( 30, 175, 9727, 9730, false, 0 );
			AddLabel( 65, 178, 1161, "I will stay here" );

			AddLabel( 30, 20, 2006, "Your leader is offering to take you with them" );

			AddLabel( 30, 105, 2006, "Will you travel through this moongate with your leader?" );

			AddImage( 65, 72, 5605 );

			AddImageTiled( 80, 90, 200, 1, 9107 );
			AddImageTiled( 95, 92, 200, 1, 9157 );

			AddLabel( 90, 70, 1645, leader.Name );

			AddButton( 290, 175, 247, 248, 2, GumpButtonType.Reply, 0 );

			AddImageTiled( 15, 14, 365, 1, 9107 );
			AddImageTiled( 380, 14, 1, 190, 9105 );
			AddImageTiled( 15, 205, 365, 1, 9107 );
			AddImageTiled( 15, 14, 1, 190, 9105 );
			AddImageTiled( 0, 0, 395, 1, 9157 );
			AddImageTiled( 394, 0, 1, 217, 9155 );
			AddImageTiled( 0, 216, 395, 1, 9157 );
			AddImageTiled( 0, 0, 1, 217, 9155 );
		}

		public override void OnResponse( NetState sender, RelayInfo info )
		{
			if ( info.ButtonID == 2 && info.IsSwitched( 1 ) )
			{
				if ( m_Member.InRange( m_Gate, 3 ) )
				{
					m_Leader.SendMessage( "{0} has accepted your offer and followed you through the moongate", m_Member.Name );

					BaseCreature.TeleportPets( m_Member, m_Leader.Location, m_Leader.Map );

					m_Member.Map = m_Leader.Map;
					m_Member.Location = m_Leader.Location;

					m_Member.PlaySound( 0x1FE );
				}
				else
				{
					m_Member.SendMessage( "The offer was declined" );
				}
			}
			else
			{
				m_Member.SendMessage( "You have declined the offer!" );
				m_Leader.SendMessage( "{0} has declined your offer to travel through the moongate with you!", m_Member.Name );
			}
		}
	}
}