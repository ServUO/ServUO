using System;

namespace Server.Engines.XmlSpawner2
{
    public class MobFactionRegionStone : Item
    {
        public Map m_MobFactionMap;
        private string m_MobFactionRegionName;
        private MobFactionRegion m_MobFactionRegion;// MobFaction region
        private MusicName m_Music;
        private int m_MobFactionPriority;
        private Rectangle3D[] m_MobFactionArea;
        private string m_CopiedRegion;
        private XmlMobFactions.GroupTypes m_FactionType;
        private int m_FactionLevel;
        private Point3D m_EjectLocation;
        private Map m_EjectMap;
        [Constructable]
        public MobFactionRegionStone()
            : base(0x161D)
        {
            this.Visible = false;
            this.Movable = false;
            this.Name = "MobFaction Region Controller";

            this.m_MobFactionRegionName = "MobFaction Game Region";

            this.m_MobFactionPriority = 0x90;             // high priority

            this.m_MobFactionMap = this.Map;
        }

        public MobFactionRegionStone(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D EjectLocation
        {
            get
            {
                return this.m_EjectLocation;
            }
            set
            {
                this.m_EjectLocation = value;
                if (this.m_MobFactionRegion != null)
                {
                    this.m_MobFactionRegion.EjectLocation = this.m_EjectLocation;
                }
                //RefreshRegions();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public Map EjectMap
        {
            get
            {
                return this.m_EjectMap;
            }
            set
            {
                this.m_EjectMap = value;
                if (this.m_MobFactionRegion != null)
                {
                    this.m_MobFactionRegion.EjectMap = this.m_EjectMap;
                }
                //RefreshRegions();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public Rectangle3D[] MobFactionArea
        {
            get
            {
                return this.m_MobFactionArea;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public string MobFactionRegionName
        {
            get
            {
                return this.m_MobFactionRegionName;
            }
            set
            {
                this.m_MobFactionRegionName = value;

                this.RefreshRegions();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public MobFactionRegion MobFactionRegion
        {
            get
            {
                return this.m_MobFactionRegion;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public Map MobFactionMap
        {
            get
            {
                return this.m_MobFactionMap;
            }
            set
            {
                this.m_MobFactionMap = value;

                this.RefreshRegions();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public MusicName MobFactionMusic
        {
            get
            {
                return this.m_Music;
            }
            set
            {
                this.m_Music = value;
                if (this.m_MobFactionRegion != null)
                {
                    this.m_MobFactionRegion.Unregister();
                    this.m_MobFactionRegion.Music = this.m_Music;
                    this.m_MobFactionRegion.Register();
                }
                //RefreshRegions();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int MobFactionPriority
        {
            get
            {
                return this.m_MobFactionPriority;
            }
            set
            {
                this.m_MobFactionPriority = value;

                this.RefreshRegions();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public XmlMobFactions.GroupTypes FactionType
        {
            get
            {
                return this.m_FactionType;
            }
            set
            {
                this.m_FactionType = value;
                if (this.m_MobFactionRegion != null)
                {
                    this.m_MobFactionRegion.FactionType = this.m_FactionType;
                }
                //RefreshRegions();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int FactionLevel
        {
            get
            {
                return this.m_FactionLevel;
            }
            set
            {
                this.m_FactionLevel = value;
                if (this.m_MobFactionRegion != null)
                {
                    this.m_MobFactionRegion.FactionLevel = this.m_FactionLevel;
                }
                //RefreshRegions();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public string CopyRegion
        {
            get
            {
                return this.m_CopiedRegion;
            }
            set
            {
                if (value == null)
                {
                    this.m_CopiedRegion = null;
                }
                else
                {
                    // find the named region
                    //Region r = FindRegion(value);
                    Region r = this.Map.Regions[value];

                    if (r != null)
                    {
                        this.m_CopiedRegion = value;

                        // copy the coords, map, and music from that region
                        this.m_MobFactionMap = r.Map;
                        this.m_Music = r.Music;
                        this.m_MobFactionPriority = r.Priority;
                        this.m_MobFactionArea = r.Area;

                        this.RefreshRegions();
                    }
                }
            }
        }
        public static Region FindRegion(string name)
        {
            if (Region.Regions == null)
                return null;

            foreach (Region region in Region.Regions)
            {
                if (string.Compare(region.Name, name, true) == 0)
                {
                    return region;
                }
            }

            return null;
        }

        public override void OnLocationChange(Point3D oldLocation)
        {
            base.OnLocationChange(oldLocation);

            // assign the eject location to the new location
            this.EjectLocation = this.Location;
        }

        public override void OnMapChange()
        {
            base.OnMapChange();

            // assign the eject map to the new map
            this.EjectMap = this.Map;
        }

        public void RefreshRegions()
        {
            if (this.m_MobFactionRegion != null)
            {
                this.m_MobFactionRegion.Unregister();
            }
            // define a new one based on the named region
            this.m_MobFactionRegion = new MobFactionRegion(this.m_MobFactionRegionName, this.m_MobFactionMap, this.m_MobFactionPriority, this.m_MobFactionArea);
            // update the new region properties with current values
            this.m_MobFactionRegion.Music = this.m_Music;
            this.m_MobFactionRegion.EjectLocation = this.m_EjectLocation;
            this.m_MobFactionRegion.EjectMap = this.m_EjectMap;
            this.m_MobFactionRegion.FactionType = this.m_FactionType;
            this.m_MobFactionRegion.FactionLevel = this.m_FactionLevel;

            this.m_MobFactionRegion.Register();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version

            // version 1
            writer.Write(this.m_EjectLocation);
            if (this.m_EjectMap != null)
            {
                writer.Write(this.m_EjectMap.ToString());
            }
            else
            {
                writer.Write(String.Empty);
            }

            writer.Write(this.m_FactionType.ToString());

            writer.Write(this.m_FactionLevel);
			
            // version 0
            writer.Write((int)this.m_Music);
            writer.Write(this.m_MobFactionPriority);
            // removed in version 1
            //writer.Write(m_MobFactionArea);
            writer.Write(this.m_MobFactionRegionName);
            if (this.m_MobFactionMap != null)
            {
                writer.Write(this.m_MobFactionMap.Name);
            }
            else
            {
                writer.Write(String.Empty);
            }
            writer.Write(this.m_CopiedRegion);

            // do the coord list
            if (this.m_MobFactionRegion != null && this.m_MobFactionRegion.Area != null && this.m_MobFactionRegion.Area.Length > 0)
            {
                writer.Write(this.m_MobFactionRegion.Area.Length);

                for (int i = 0; i < this.m_MobFactionRegion.Area.Length; i++)
                { 
                    writer.Write(this.m_MobFactionRegion.Area[i].Start);
                    writer.Write(this.m_MobFactionRegion.Area[i].End);
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

            list.Add(1062613, this.m_MobFactionRegionName);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            this.m_MobFactionArea = new Rectangle3D[0];

            switch (version)
            {
                case 1:
                    {
                        this.m_EjectLocation = reader.ReadPoint3D();
                        string ejectmap = reader.ReadString();
                        try
                        {
                            this.m_EjectMap = Map.Parse(ejectmap);
                        }
                        catch
                        {
                        }

                        string factiontype = reader.ReadString();
                        try
                        {
                            this.m_FactionType = (XmlMobFactions.GroupTypes)Enum.Parse(typeof(XmlMobFactions.GroupTypes), factiontype);
                        }
                        catch
                        {
                        }
                        this.m_FactionLevel = reader.ReadInt();

                        goto case 0;
                    }
                case 0:
                    {
                        this.m_Music = (MusicName)reader.ReadInt();
                        this.m_MobFactionPriority = reader.ReadInt();
                        if (version < 1)
                        {
                            // old region area
                            reader.ReadRect2D();
                        }
                        this.m_MobFactionRegionName = reader.ReadString();
                        string mapname = reader.ReadString();
                        try
                        {
                            this.m_MobFactionMap = Map.Parse(mapname);
                        }
                        catch
                        {
                        }
                        this.m_CopiedRegion = reader.ReadString();
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
                                this.m_MobFactionArea = Region.ConvertTo3D(area);
                            }
                            else
                            {
                                this.m_MobFactionArea = new Rectangle3D[count];
                                for (int i = 0; i < count; i++)
                                {
                                    this.m_MobFactionArea[i] = new Rectangle3D(reader.ReadPoint3D(), reader.ReadPoint3D());
                                }
                            }
                        }

                        break;
                    }
            }

            // refresh the region
            Timer.DelayCall(TimeSpan.Zero, new TimerCallback(RefreshRegions));
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (m != null && m.AccessLevel >= AccessLevel.GameMaster)
            {
                m.SendMessage("Define the MobFaction area");
                this.DefineMobFactionArea(m);
            }
        }

        public void DefineMobFactionArea(Mobile m)
        {
            BoundingBoxPicker.Begin(m, new BoundingBoxCallback(MobFactionRegionArea_Callback), this);
        }

        public override void OnDelete()
        {
            if (this.m_MobFactionRegion != null)
                this.m_MobFactionRegion.Unregister();

            base.OnDelete();
        }

        private static void MobFactionRegionArea_Callback(Mobile from, Map map, Point3D start, Point3D end, object state)
        {
            // assign these coords to the region
            MobFactionRegionStone s = state as MobFactionRegionStone;

            if (s != null && from != null)
            {
                s.m_MobFactionArea = new Rectangle3D[1];
                //s.m_MobFactionArea[0] = new Rectangle3D(start,end);
                s.m_MobFactionArea[0] = Region.ConvertTo3D(new Rectangle2D(start.X, start.Y, end.X - start.X + 1, end.Y - start.Y + 1));
                s.m_MobFactionMap = map;

                s.CopyRegion = null;

                s.RefreshRegions();
            }
        }
    }
}