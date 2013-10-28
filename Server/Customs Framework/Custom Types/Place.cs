#region Header
// **********
// ServUO - Place.cs
// **********
#endregion

#region References
using CustomsFramework;
#endregion

namespace Server
{
	public class Place
	{
		private Map _Map;
		private Point3D _Location;

		public Place()
		{
			_Map = Map.Internal;
			_Location = new Point3D(0, 0, 0);
		}

		public Place(Map map, Point3D location)
		{
			_Map = map;
			_Location = location;
		}

		public Place(GenericReader reader)
		{
			Deserialize(reader);
		}

		[CommandProperty(AccessLevel.Decorator)]
		public Map Map { get { return _Map; } set { _Map = value; } }

		[CommandProperty(AccessLevel.Decorator)]
		public Point3D Location { get { return _Location; } set { _Location = value; } }

		public void Serialize(GenericWriter writer)
		{
			writer.WriteVersion(0);

			// Version 0
			writer.Write(_Map);
			writer.Write(_Location);
		}

		private void Deserialize(GenericReader reader)
		{
			int version = reader.ReadInt();

			switch (version)
			{
				case 0:
					{
						_Map = reader.ReadMap();
						_Location = reader.ReadPoint3D();
						break;
					}
			}
		}
	}
}