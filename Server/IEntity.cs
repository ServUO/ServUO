#region Header
// **********
// ServUO - IEntity.cs
// **********
#endregion

#region References
using System;
#endregion

namespace Server
{
	public interface IEntity : IPoint3D, IComparable, IComparable<IEntity>
	{
		Serial Serial { get; }
        Point3D Location { get; set; }
		Map Map { get; }

		bool Deleted { get; }
        bool NoMoveHS { get; set; }

		void Delete();
		void ProcessDelta();
	}

	public class Entity : IEntity, IComparable<Entity>
	{
		public int CompareTo(IEntity other)
		{
			if (other == null)
			{
				return -1;
			}

			return m_Serial.CompareTo(other.Serial);
		}

		public int CompareTo(Entity other)
		{
			return CompareTo((IEntity)other);
		}

		public int CompareTo(object other)
		{
			if (other == null || other is IEntity)
			{
				return CompareTo((IEntity)other);
			}

			throw new ArgumentException();
		}

		private Serial m_Serial;
		private Point3D m_Location;
		private readonly Map m_Map;

		private bool m_Deleted;

		public Entity(Serial serial, Point3D loc, Map map)
		{
			m_Serial = serial;
			m_Location = loc;
			m_Map = map;
		}

		public Serial Serial { get { return m_Serial; } }

        public Point3D Location { get { return m_Location; } set { } }

		public int X { get { return m_Location.X; } }

		public int Y { get { return m_Location.Y; } }

		public int Z { get { return m_Location.Z; } }

		public Map Map { get { return m_Map; } }

		public bool Deleted { get { return m_Deleted; } }

        public bool NoMoveHS { get; set; }

		public void Delete()
		{
			m_Deleted = true;
		}

		public void ProcessDelta()
		{ }
	}
}