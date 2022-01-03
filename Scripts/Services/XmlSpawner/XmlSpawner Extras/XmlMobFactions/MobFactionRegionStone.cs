using System;
using Server;
using Server.Mobiles;
using Server.Items;
using Server.Regions;
using System.Collections;

namespace Server.Engines.XmlSpawner2
{

	public class MobFactionRegionStone : Item
	{

		public Map m_MobFactionMap;
		private string m_MobFactionRegionName;
		private MobFactionRegion m_MobFactionRegion;                    // MobFaction region
		private MusicName m_Music;
		private int m_MobFactionPriority;
		private Rectangle3D [] m_MobFactionArea;
		private string m_CopiedRegion;
		private XmlMobFactions.GroupTypes m_FactionType;
		private int m_FactionLevel;
		private Point3D m_EjectLocation;
		private Map m_EjectMap;

		[CommandProperty(AccessLevel.GameMaster)]
		public Point3D EjectLocation
		{
			get { return m_EjectLocation; }
			set
			{
				m_EjectLocation = value;
				if (m_MobFactionRegion != null)
				{
					m_MobFactionRegion.EjectLocation = m_EjectLocation;
				}
				//RefreshRegions();
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public Map EjectMap
		{
			get { return m_EjectMap; }
			set
			{
				m_EjectMap = value;
				if (m_MobFactionRegion != null)
				{
					m_MobFactionRegion.EjectMap = m_EjectMap;
				}
				//RefreshRegions();
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public Rectangle3D[] MobFactionArea
		{
			get { return m_MobFactionArea; }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public string MobFactionRegionName
		{
			get { return m_MobFactionRegionName; }
			set
			{
				m_MobFactionRegionName = value;

				RefreshRegions();
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public MobFactionRegion MobFactionRegion
		{
			get { return m_MobFactionRegion; }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public Map MobFactionMap
		{
			get { return m_MobFactionMap; }
			set
			{
				m_MobFactionMap = value;

				RefreshRegions();
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public MusicName MobFactionMusic
		{
			get { return m_Music; }
			set
			{
				m_Music = value;
				if (m_MobFactionRegion != null)
				{
					m_MobFactionRegion.Unregister();
					m_MobFactionRegion.Music = m_Music;
					m_MobFactionRegion.Register();
				}
				//RefreshRegions();
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int MobFactionPriority
		{
			get { return m_MobFactionPriority; }
			set
			{
				m_MobFactionPriority = value;

				RefreshRegions();
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public XmlMobFactions.GroupTypes FactionType
		{
			get { return m_FactionType; }
			set
			{
				m_FactionType = value;
				if (m_MobFactionRegion != null)
				{
					m_MobFactionRegion.FactionType = m_FactionType;
				}

				//RefreshRegions();
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int FactionLevel
		{
			get { return m_FactionLevel; }
			set
			{
				m_FactionLevel = value;
				if (m_MobFactionRegion != null)
				{
					m_MobFactionRegion.FactionLevel = m_FactionLevel;
				}

				//RefreshRegions();
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public string CopyRegion
		{
			get
			{
				return m_CopiedRegion;
			}
			set
			{
				if (value == null)
				{
					m_CopiedRegion = null;
				}
				else
				{

					// find the named region
					//Region r = FindRegion(value);
					Region r = this.Map.Regions[value];

					if (r != null)
					{


						m_CopiedRegion = value;

						// copy the coords, map, and music from that region
						m_MobFactionMap = r.Map;
						m_Music = r.Music;
						m_MobFactionPriority = r.Priority;
						m_MobFactionArea = r.Area;


						RefreshRegions();

					}
				}
			}
		}

		public override void OnLocationChange(Point3D oldLocation)
		{
			base.OnLocationChange(oldLocation);

			// assign the eject location to the new location
			EjectLocation = Location;

		}

		public override void OnMapChange()
		{
			base.OnMapChange();

			// assign the eject map to the new map
			EjectMap = Map;
		}

		public void RefreshRegions()
		{
			if (m_MobFactionRegion != null)
			{
				m_MobFactionRegion.Unregister();
			}
			// define a new one based on the named region
			m_MobFactionRegion = new MobFactionRegion(m_MobFactionRegionName, m_MobFactionMap, m_MobFactionPriority, m_MobFactionArea);
			// update the new region properties with current values
			m_MobFactionRegion.Music = m_Music;
			m_MobFactionRegion.EjectLocation = m_EjectLocation;
			m_MobFactionRegion.EjectMap = m_EjectMap;
			m_MobFactionRegion.FactionType = m_FactionType;
			m_MobFactionRegion.FactionLevel = m_FactionLevel;

			m_MobFactionRegion.Register();
		}

		[Constructable]
		public MobFactionRegionStone()
			: base(0x161D)
		{
			Visible = false;
			Movable = false;
			Name = "MobFaction Region Controller";

			m_MobFactionRegionName = "MobFaction Game Region";

			m_MobFactionPriority = 0x90;             // high priority

			m_MobFactionMap = this.Map;

		}

		public MobFactionRegionStone(Serial serial)
			: base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write((int)1); // version

			// version 1
			writer.Write(m_EjectLocation);
			if (m_EjectMap != null)
			{
				writer.Write(m_EjectMap.ToString());
			}
			else
			{
				writer.Write(String.Empty);
			}

			writer.Write(m_FactionType.ToString());

			writer.Write(m_FactionLevel);
			

			// version 0
			writer.Write((int)m_Music);
			writer.Write(m_MobFactionPriority);
			// removed in version 1
			//writer.Write(m_MobFactionArea);
			writer.Write(m_MobFactionRegionName);
			if (m_MobFactionMap != null)
			{
				writer.Write(m_MobFactionMap.Name);
			} else
			{
				writer.Write(String.Empty);
			}
			writer.Write(m_CopiedRegion);

			// do the coord list
			if (m_MobFactionRegion != null && m_MobFactionRegion.Area != null && m_MobFactionRegion.Area.Length > 0)
			{
				writer.Write(m_MobFactionRegion.Area.Length);

				for (int i = 0; i < m_MobFactionRegion.Area.Length; i++)
				{				
					writer.Write(m_MobFactionRegion.Area[i].Start);
					writer.Write(m_MobFactionRegion.Area[i].End);
				}
			}
			else
			{
				writer.Write((int)0);
			}
		}

		public override void GetProperties(ObjectPropertyList list)
		{
			base.GetProperties(list);

			list.Add(1062613, m_MobFactionRegionName);
		}



		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();

			m_MobFactionArea = new Rectangle3D[0];

			switch (version)
			{
				case 1:
					{
						m_EjectLocation = reader.ReadPoint3D();
						string ejectmap = reader.ReadString();
						try
						{
							m_EjectMap = Map.Parse(ejectmap);

						}
						catch { }

						string factiontype = reader.ReadString();
							try
						{
							m_FactionType = (XmlMobFactions.GroupTypes)Enum.Parse(typeof(XmlMobFactions.GroupTypes), factiontype);
						} 
						catch{}
						m_FactionLevel = reader.ReadInt();

						goto case 0;
					}
				case 0:
					{
						m_Music = (MusicName)reader.ReadInt();
						m_MobFactionPriority = reader.ReadInt();
						if (version < 1)
						{
							// old region area
							 reader.ReadRect2D();
						}
						m_MobFactionRegionName = reader.ReadString();
						string mapname = reader.ReadString();
						try
						{
							m_MobFactionMap = Map.Parse(mapname);
						}
						catch { }
						m_CopiedRegion = reader.ReadString();
						// do the coord list
						int count = reader.ReadInt();
						if (count > 0)
						{
							// the old version used 2D rectangles for the region area.  The new version uses 3D
							if (version < 1)
							{
								Rectangle2D[] area = new Rectangle2D[count];
								for (int i = 0; i < count; i++)
								{
									area[i] = reader.ReadRect2D();
								}
								m_MobFactionArea = Region.ConvertTo3D(area);
							}
							else
							{
								m_MobFactionArea = new Rectangle3D[count];
								for (int i = 0; i < count; i++)
								{
									m_MobFactionArea[i] = new Rectangle3D(reader.ReadPoint3D(), reader.ReadPoint3D());
								}
							}
						}

						break;
					}
			}

			// refresh the region
			Timer.DelayCall(TimeSpan.Zero, new TimerCallback(RefreshRegions));
		}

		public static Region FindRegion(string name)
		{
			if (Region.Regions == null) return null;

			foreach (Region region in Region.Regions)
			{
				if (string.Compare(region.Name, name, true) == 0)
				{
					return region;
				}
			}

			return null;
		}

		public override void OnDoubleClick(Mobile m)
		{
			if (m != null && m.AccessLevel >= AccessLevel.GameMaster)
			{
				m.SendMessage("Define the MobFaction area");
				DefineMobFactionArea(m);
			}
		}


		public void DefineMobFactionArea(Mobile m)
		{
			BoundingBoxPicker.Begin(m, new BoundingBoxCallback(MobFactionRegionArea_Callback), this);
		}


		private static void MobFactionRegionArea_Callback(Mobile from, Map map, Point3D start, Point3D end, object state)
		{
			// assign these coords to the region
			MobFactionRegionStone s = state as MobFactionRegionStone;

			if (s != null && from != null)
			{
				s.m_MobFactionArea = new Rectangle3D[1];
				//s.m_MobFactionArea[0] = new Rectangle3D(start,end);
				s.m_MobFactionArea[0] = Region.ConvertTo3D( new Rectangle2D(start.X, start.Y, end.X - start.X + 1, end.Y - start.Y + 1) );
				s.m_MobFactionMap = map;

				s.CopyRegion = null;

				s.RefreshRegions();

			}
		}

		public override void OnDelete()
		{
			if (m_MobFactionRegion != null)
				m_MobFactionRegion.Unregister();

			base.OnDelete();
		}
	}
}
