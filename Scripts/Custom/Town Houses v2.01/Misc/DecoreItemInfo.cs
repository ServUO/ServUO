using System;
using Server;

namespace Knives.TownHouses
{
	public class DecoreItemInfo
	{
		private string c_TypeString;
		private string c_Name;
		private int c_ItemID;
		private int c_Hue;
		private Point3D c_Location;
		private Map c_Map;

		public string TypeString{ get{ return c_TypeString; } }
		public string Name{ get{ return c_Name; } }
		public int ItemID{ get{ return c_ItemID; } }
		public int Hue{ get{ return c_Hue; } }
		public Point3D Location{ get{ return c_Location; } }
		public Map Map{ get{ return c_Map; } }

		public DecoreItemInfo()
		{
		}

		public DecoreItemInfo( string typestring, string name, int itemid, int hue, Point3D loc, Map map )
		{
			c_TypeString = typestring;
			c_ItemID = itemid;
			c_Location = loc;
			c_Map = map;
		}

		public void Save( GenericWriter writer )
		{
			writer.Write( (int)1 ); // Version

			// Version 1
			writer.Write( c_Hue );
			writer.Write( c_Name );

			writer.Write( c_TypeString );
			writer.Write( c_ItemID );
			writer.Write( c_Location );
			writer.Write( c_Map );
		}

		public void Load( GenericReader reader )
		{
			int version = reader.ReadInt();

			if ( version >= 1 )
			{
				c_Hue = reader.ReadInt();
				c_Name = reader.ReadString();
			}

			c_TypeString = reader.ReadString();
			c_ItemID = reader.ReadInt();
			c_Location = reader.ReadPoint3D();
			c_Map = reader.ReadMap();
		}
	}
}