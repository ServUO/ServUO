#region References
using System;
#endregion

namespace Server
{
	public interface IEntity : IPoint3D, IComparable, IComparable<IEntity>
	{
		Serial Serial { get; }

        Point3D Location { get; set; }
		Map Map { get; set; }

        bool NoMoveHS { get; set; }

		Direction Direction { get; set; }

		string Name { get; set; }

		int Hue { get; set; }

		bool Deleted { get; }

		void Delete();
		void ProcessDelta();
		void InvalidateProperties();
        void OnStatsQuery(Mobile m);
	}

	public class Entity : IEntity, IComparable<Entity>
	{
		public Serial Serial { get; private set; }

		public Point3D Location { get; set; }
		public Map Map { get; set; }

		public int X { get { return Location.X; } }
		public int Y { get { return Location.Y; } }
		public int Z { get { return Location.Z; } }

		public bool Deleted { get; private set; }

		public bool NoMoveHS { get; set; }

		Direction IEntity.Direction { get; set; }

		string IEntity.Name { get; set; }

		int IEntity.Hue { get; set; }

		public Entity(Serial serial, Point3D loc, Map map)
		{
			Serial = serial;
			Location = loc;
			Map = map;
		}

		public int CompareTo(IEntity other)
		{
			if (other == null)
			{
				return -1;
			}

			return Serial.CompareTo(other.Serial);
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

		public void Delete()
		{
			if (Deleted)
			{
				return;
			}

			Deleted = true;

			Location = Point3D.Zero;
			Map = null;
		}

		void IEntity.ProcessDelta()
		{ }

		void IEntity.InvalidateProperties()
		{ }

		void IEntity.OnStatsQuery(Mobile m)
		{ }
	}
}