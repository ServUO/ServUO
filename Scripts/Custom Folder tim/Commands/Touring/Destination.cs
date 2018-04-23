using System;
using System.Collections;

using Server;

namespace Server.Touring
{
	public struct Destination
	{
		private Map m_Map;
		private Point3D m_Location;

		private string
			m_Name,
			m_Description;

		private TimeSpan m_Delay;

		public Map Map { get { return m_Map; } }
		public Point3D Location { get { return m_Location; } }

		public string Name { get { return m_Name; } }
		public string Description { get { return m_Description; } }

		public TimeSpan Delay { get { return m_Delay; } }

		public Destination(Map map, Point3D location, string name, string description, TimeSpan delay)
		{
			m_Map = map;
			m_Location = location;
			m_Name = name;
			m_Description = description;
			m_Delay = delay;
		}

		public bool IsValid()
		{
			return !Server.Spells.SpellHelper.IsInvalid(m_Map, m_Location);
		}

		public void DoTeleport(Mobile m)
		{
			if (m != null && !m.Deleted)
			{
				m.Blessed = true;
				m.Freeze(m_Delay);
				m.MoveToWorld(m_Location, m_Map);
				Tour.DestinationChangedInvoke(new DestinationChangedEventArgs(this, m));
			}
		}
	}
}