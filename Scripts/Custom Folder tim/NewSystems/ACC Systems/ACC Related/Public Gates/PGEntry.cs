using System;
using System.Collections.Generic;

namespace Server.ACC.PG
{
	public enum EntryFlag
	{
		None      = 0x00000000,
		Generate  = 0x00000001,
		StaffOnly = 0x00000002,
		Reds      = 0x00000004,
		Young     = 0x00000008,
		Charge    = 0x00000010,
	}

	#region PGCategory Class
	public class PGCategory
	{
		private string    m_Name;
		private List<PGLocation> m_Locations;
		private EntryFlag m_Flags;
		private int       m_Cost;

		public string Name{ get{ return m_Name; } set{ m_Name = value; } }
		public List<PGLocation> Locations{ get{ return m_Locations; } set{ m_Locations = value; } }
		public EntryFlag Flags{ get{ return m_Flags; } set{ m_Flags = value; } }
		public int Cost{ get{ return m_Cost; } set{ m_Cost = value; } }

		public bool GetFlag( EntryFlag flag ){ return( (m_Flags & flag) != 0 ); }

		public PGCategory( string name, EntryFlag flags ) : this( name, flags, 0 )
		{
		}

		public PGCategory( string name, EntryFlag flags, int cost )
		{
			m_Name      = name;
			m_Locations = new List<PGLocation>();
			m_Flags     = flags;
			m_Cost      = cost;
		}

		public PGCategory( GenericReader reader )
		{
			Deserialize( reader );
		}

		public void Serialize( GenericWriter writer )
		{
			writer.Write( 0 ); //version

			writer.Write( m_Name );

            writer.Write( m_Locations.Count );
            IEnumerator<PGLocation> Locs = m_Locations.GetEnumerator();
            while(Locs.MoveNext())
                Locs.Current.Serialize(writer);

			writer.Write( (int)m_Flags );
			writer.Write( (int)m_Cost );
		}

		public void Deserialize( GenericReader reader )
		{
			int version = reader.ReadInt();

			m_Name = reader.ReadString();

			m_Locations = new List<PGLocation>();

            for (int i = reader.ReadInt(); i >0; i--)
			{
				m_Locations.Add( new PGLocation( reader ) );
			}

			m_Flags = (EntryFlag)reader.ReadInt();
			m_Cost = reader.ReadInt();
		}
	}
	#endregion //PGCategory Class

	#region PGLocation Class
	public class PGLocation
	{
		private string    m_Name;
		private EntryFlag m_Flags;
		private Point3D   m_Location;
		private Map       m_Map;
		private int       m_Hue;
		private int       m_Cost;

		public string    Name{      get{ return m_Name; }      set{ m_Name = value; } }
		public EntryFlag Flags{     get{ return m_Flags; }     set{ m_Flags = value; } }
		public Point3D   Location{  get{ return m_Location; }  set{ m_Location = value; } }
		public Map       Map{       get{ return m_Map; }       set{ m_Map = value; } }
		public int       Hue{       get{ return m_Hue; }       set{ m_Hue = value; } }
		public int       Cost{      get{ return m_Cost; }      set{ m_Cost = value; } }

		public bool GetFlag( EntryFlag flag ){ return( (m_Flags & flag) != 0 ); }

		public PGLocation( string name, EntryFlag flags, Point3D location, Map map, int hue ) : this( name, flags, location, map, hue, 0 )
		{
		}

		public PGLocation( string name, EntryFlag flags, Point3D location, Map map, int hue, int cost )
		{
			m_Name = name;
			m_Flags = flags;
			m_Location = location;
			m_Map = map;
			m_Hue = hue;
			m_Cost = cost;
		}

		public PGLocation( GenericReader reader )
		{
			Deserialize( reader );
		}

		public void Serialize( GenericWriter writer )
		{
			writer.Write( (int)0 ); //version

			writer.Write( (string)m_Name );
			writer.Write( (int)m_Flags );
			writer.Write( m_Location );
			writer.Write( m_Map );
			writer.Write( m_Hue );
			writer.Write( m_Cost );
		}

		public void Deserialize( GenericReader reader )
		{
			int version = reader.ReadInt();

			m_Name = reader.ReadString();
			m_Flags = (EntryFlag)reader.ReadInt();
			m_Location = reader.ReadPoint3D();
			m_Map = reader.ReadMap();
			m_Hue = reader.ReadInt();
			m_Cost = reader.ReadInt();
		}
	}
	#endregion //PGLocation Class
}