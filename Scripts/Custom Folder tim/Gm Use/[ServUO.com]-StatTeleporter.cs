using System;
using Server;
using Server.Mobiles;
using Server.Network;
using Server.Spells;

namespace Server.Items
{
	public class StatTeleporter : Teleporter
	{
		public enum Stat_Name
		{
			Int,
			Dex,
			Str
		}
		
		private Stat_Name m_Stat;
		private int m_Required;
		private string m_Message;

		[CommandProperty( AccessLevel.GameMaster )]
		public Stat_Name Stat
		{
			get{ return m_Stat; }
			set{ m_Stat = value; InvalidateProperties(); }
		}
		
		[CommandProperty( AccessLevel.GameMaster )]
		public int Required
		{
			get{ return m_Required; }
			set{ m_Required = value; InvalidateProperties(); }
		}
		
		[CommandProperty( AccessLevel.GameMaster )]
		public String Message
		{
			get{ return m_Message; }
			set{ m_Message = value; InvalidateProperties(); }
		}
		
		public override bool OnMoveOver( Mobile m )
		{
			if ( Active )
			{
				if ( !Creatures && !m.Player )
					return true;
				
				int p_Stat = 0;
				
				switch(m_Stat)
				{
					case(Stat_Name.Int):
					{
						p_Stat = m.Int;
						break;
					}
					case(Stat_Name.Dex):
					{
						p_Stat = m.Dex;
						break;
					}
					case(Stat_Name.Str):
					{
						p_Stat = m.Str;
						break;
					}
				}
				
				if ( p_Stat < m_Required )
				{
					SendMessage(m);
					return true;
				}
				
				StartTeleport( m );
				return false;
			}

			return true;
		}
		public void SendMessage(Mobile m)
		{
			if(m is PlayerMobile)
			{
				if( m_Message != null )
				{
					m.SendMessage(m_Message);
				}
				else
				{
					switch(m_Stat)
					{
						case(Stat_Name.Int):
						{
							m.SendMessage("You do not have enough intelligence.");
							break;
						}
						case(Stat_Name.Dex):
						{
							m.SendMessage("You do not have enough dexterity.");
							break;
						}
						case(Stat_Name.Str):
						{
							m.SendMessage("You do not have enough strength.");
							break;
						}
					}
				}
			}
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			string p_Stat;
			switch(m_Stat)
			{
				case(Stat_Name.Int):
				{
					p_Stat = "Intelligence";
					break;
				}
				case(Stat_Name.Dex):
				{
					p_Stat = "Dexterity";
					break;
				}
				case(Stat_Name.Str):
				{
					p_Stat = "Strength";
					break;
				}
				default:
				{
					p_Stat = "Dexterity";
					break;
				}
			}
			
			list.Add( 1060661, "{0}\t{1:F1}", p_Stat, m_Required );
		}

		[Constructable]
		public StatTeleporter()
		{
			m_Stat = Stat_Name.Dex;
		}

		public StatTeleporter( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version

			writer.Write( (int) m_Stat );
			writer.Write( (int) m_Required );
			writer.Write( (string) m_Message );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 0:
				{
					m_Stat = (Stat_Name)reader.ReadInt();
					m_Required = reader.ReadInt();
					m_Message = reader.ReadString();

					break;
				}
			}
		}
	}
}