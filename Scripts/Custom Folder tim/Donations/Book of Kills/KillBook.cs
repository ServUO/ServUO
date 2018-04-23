/*----------------------------------------------------------------------------------------------------
*Script created on 25-July for Runuo
*Author plus
*
*This kill book works only if it is in your backpack. When you double click it
*is when it truely becomes yours.
*
*If you don't want to use the point system you can change the bool value Pointsys in game.
*
*If you DON'T HAVE a point system - On line 65 and 95 is where you need to comment out the point references.
*They are marked so you shouldn't get lost
*
*And set m_Pointsys to false.
*
*-----------------------------------------------------------------------------------------------------
*/

using System;
using Server;
using Server.Gumps;
using Server.Network;
using Server.Items;
using Server.Mobiles;
using System.Collections;

namespace Server.Items
{
	public class KillBook : Item
	{

//	************************************ User Configuration **********************************

		private static bool m_Pointsys = true; //Set this to false if you aren't using nox's point system.

//	*******************************************************************************************


		private ArrayList m_Entries;
		private Mobile m_BookOwner;
		private int m_TotKills;
		private int m_TotDeaths;

		[CommandProperty( AccessLevel.GameMaster )]
		public Mobile BookOwner	{ get{ return m_BookOwner; }set{ m_BookOwner = value; }}

		public static bool Pointsys{ get{ return m_Pointsys; } set{ m_Pointsys = value; }}

		public int TotKills{ get{ return m_TotKills; }set{ m_TotKills = value; }}

		public int TotDeaths{ get{ return m_TotDeaths; }set{ m_TotDeaths = value;}}

		public ArrayList Entries { get{return m_Entries; }}

		public override bool DisplayLootType{ get{ return false; } }

		[Constructable]
		public KillBook ( ) : base ( 0x2253 )
		{
			Name = "Book Of Kills";
			Movable = true;
			Hue = 0x4D7;
			m_Entries = new ArrayList();
			LootType = LootType.Blessed;
		}

		public override void OnDoubleClick( Mobile from )
		{
			from.CloseGump( typeof( KillGump  ) );
			from.CloseGump( typeof( KillIndex ) );

			if (from.InRange( this.GetWorldLocation(), 1 ))
			{
				if (from != null && this.BookOwner != null )
				{
					from.SendGump( new KillIndex( from, this, m_Entries ) );
				}
				else
				{
					this.m_BookOwner = from;
					from.SendGump( new KillIndex( from, this, m_Entries ) );
				}

				if ( from != m_BookOwner && this.BookOwner != null)
					from.SendMessage("This is not your book it will not work for you!");
			}
			else
				from.SendLocalizedMessage( 500446 ); // That is too far away.
		}

		public KillBook ( Serial serial ) : base ( serial )
		{
		}

		public override void OnSingleClick(Mobile from)
		{
			if (m_BookOwner == null)
				base.OnSingleClick(from);
			else
				LabelTo(from, String.Format("{0}'s {1}", m_BookOwner.Name, Name));
		}

		public override void Serialize ( GenericWriter writer)
		{
			base.Serialize ( writer );

			writer.Write ( (int) 0);

			writer.Write( m_Entries.Count );

			for ( int i = 0; i < m_Entries.Count; ++i )
				((DeathEntry)m_Entries[i]).Serialize( writer );

			writer.Write ( (int) m_TotKills);
			writer.Write ( (int) m_TotDeaths);
			writer.Write ( (Mobile) m_BookOwner);
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize ( reader );

			int version = reader.ReadInt();

			int count = reader.ReadInt();

			m_Entries = new ArrayList( count );

			for ( int i = 0; i < count; ++i )
				m_Entries.Add( new DeathEntry( reader ) );

			m_TotKills = reader.ReadInt();
			m_TotDeaths = reader.ReadInt();
			m_BookOwner = reader.ReadMobile();
		}

		public DeathEntry AddEntry( string name, int deaths )
		{
			foreach( DeathEntry o in m_Entries )

			if (o.Name == name)
			{
				o.Deaths += deaths;
				return o;
			}

			DeathEntry be = new DeathEntry( name, deaths );

			m_Entries.Add( be );
			return be;
		}

		public void AddEntry( DeathEntry be )
		{
			m_Entries.Add( be );
		}
	}

	public class DeathEntry : IComparable
	{
		private string m_Name;
		private int m_Deaths;

		public string Name{ get{ return m_Name; } }

		public int Deaths{ get{ return m_Deaths; } set{ m_Deaths = value; }	}

		public DeathEntry( string name, int deaths )
		{
			m_Name   = name;
			m_Deaths = deaths;
		}

		public int CompareTo(object o) //Default sorting: Kills descending, Name ascending
		{
			if (!(o is DeathEntry)) throw new ArgumentException();

			if (m_Deaths.CompareTo(((DeathEntry)o).Deaths) != 0)
				return -1 * m_Deaths.CompareTo(((DeathEntry)o).Deaths); //Kills descending
			else
				return m_Name.CompareTo(((DeathEntry)o).Name); //Name ascending
		}

		public void Serialize( GenericWriter writer )
		{
			writer.Write( (byte) 0 ); // version

			writer.Write( (string) m_Name );

			writer.Write( (int) m_Deaths );
		}

		public DeathEntry( GenericReader reader )
		{
			int version = reader.ReadByte();

			m_Name = reader.ReadString();
			m_Deaths = reader.ReadInt();
		}
	}
}