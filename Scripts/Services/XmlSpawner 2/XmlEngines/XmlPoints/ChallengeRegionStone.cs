using System;

namespace Server.Engines.XmlSpawner2
{
    public class ChallengeRegionStone : Item
    {
        public Map m_ChallengeMap;
        private string m_ChallengeRegionName;
        private ChallengeGameRegion m_ChallengeRegion;// challenge region
        private MusicName m_Music;
        private int m_Priority;
        private Rectangle3D[] m_ChallengeArea;
        private string m_CopiedRegion;
        private bool m_Disable;
        [Constructable]
        public ChallengeRegionStone()
            : base(0xED4)
        {
            this.Visible = false;
            this.Movable = false;
            this.Name = "Challenge Region Stone";

            this.ChallengeRegionName = "Challenge Game Region";

            this.ChallengePriority = 0x90;             // high priority

            this.m_ChallengeMap = this.Map;
        }

        public ChallengeRegionStone(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Rectangle3D[] ChallengeArea
        {
            get
            {
                return this.m_ChallengeArea;
            }
            set
            {
                this.m_ChallengeArea = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool DisableGuards
        {
            get
            {
                return this.m_Disable;
            }
            set 
            { 
                this.m_Disable = value;
                if (this.m_ChallengeRegion != null)
                {
                    this.m_ChallengeRegion.Unregister();
                    this.m_ChallengeRegion.Disabled = this.m_Disable;
                    this.m_ChallengeRegion.Register();
                }
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public string ChallengeRegionName
        {
            get
            {
                return this.m_ChallengeRegionName;
            }
            set
            { 
                this.m_ChallengeRegionName = value; 

                this.RefreshRegions();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public ChallengeGameRegion ChallengeRegion
        {
            get
            {
                return this.m_ChallengeRegion;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public Map ChallengeMap
        {
            get
            {
                return this.m_ChallengeMap;
            }
            set
            { 
                this.m_ChallengeMap = value; 

                this.RefreshRegions();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public MusicName ChallengeMusic
        {
            get
            {
                return this.m_Music;
            }
            set
            { 
                this.m_Music = value;
                if (this.m_ChallengeRegion != null)
                {
                    this.m_ChallengeRegion.Unregister();
                    this.m_ChallengeRegion.Music = this.m_Music;
                    this.m_ChallengeRegion.Register();
                }
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int ChallengePriority
        {
            get
            {
                return this.m_Priority;
            }
            set
            {
                this.m_Priority = value;
                
                this.RefreshRegions();
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
                    Region r = this.Map.Regions[value];
                    
                    if (r != null)
                    {
                        this.m_CopiedRegion = value;

                        // copy the coords, map, and music from that region
                        this.m_ChallengeMap = r.Map;
                        this.m_Music = r.Music;
                        this.m_Priority = r.Priority;
                        this.m_ChallengeArea = r.Area;

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

        public void RefreshRegions()
        {
            if (this.m_ChallengeRegion != null)
            {
                this.m_ChallengeRegion.Unregister();
            }

            if (this.m_ChallengeMap == null || this.m_ChallengeArea == null)
                return;

            // define a new one based on the named region
            this.m_ChallengeRegion = new ChallengeGameRegion(this.m_ChallengeRegionName, this.m_ChallengeMap, this.m_Priority, this.m_ChallengeArea);
            // update the new region properties with current values
            this.m_ChallengeRegion.Music = this.m_Music;
            this.m_ChallengeRegion.Disabled = this.m_Disable;

            this.m_ChallengeRegion.Register();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)2); // version
            // version 2
            // updated for RunUO 2.0 region changes in 3D region coordinates
            // version 1
            writer.Write(this.m_Disable);
            // version 0
            writer.Write((int)this.m_Music);
            writer.Write(this.m_Priority);
            // removed in version 2
            //writer.Write(m_ChallengeArea);

            writer.Write(this.m_ChallengeRegionName);
            if (this.m_ChallengeMap != null)
            {
                writer.Write(this.m_ChallengeMap.Name);
            }
            else
            {
                writer.Write(String.Empty);
            }
            writer.Write(this.m_CopiedRegion);

            // do the coord list
            if (this.m_ChallengeRegion != null && this.m_ChallengeRegion.Area != null && this.m_ChallengeRegion.Area.Length > 0)
            {
                writer.Write(this.m_ChallengeRegion.Area.Length);

                for (int i = 0; i < this.m_ChallengeRegion.Area.Length; i++)
                {
                    writer.Write(this.m_ChallengeRegion.Area[i].Start);
                    writer.Write(this.m_ChallengeRegion.Area[i].End);
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
            
            list.Add(1062613, this.m_ChallengeRegionName);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            this.m_ChallengeArea = new Rectangle3D[0];

            switch ( version )
            {
                case 2:
                case 1:
                    {
                        this.m_Disable = reader.ReadBool();
                        goto case 0;
                    }
                case 0:
                    {
                        this.m_Music = (MusicName)reader.ReadInt();
                        this.m_Priority = reader.ReadInt();
                        if (version < 2)
                        {
                            // old region area
                            reader.ReadRect2D();
                        }
                        this.m_ChallengeRegionName = reader.ReadString();
                        string mapname = reader.ReadString();
                        try
                        {
                            this.m_ChallengeMap = Map.Parse(mapname);
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
                            if (version < 2)
                            {
                                Rectangle2D[] area = new Rectangle2D[count];
                                for (int i = 0; i < count; i++)
                                {
                                    area[i] = reader.ReadRect2D();
                                }
                                this.m_ChallengeArea = Region.ConvertTo3D(area);
                            }
                            else
                            {
                                this.m_ChallengeArea = new Rectangle3D[count];
                                for (int i = 0; i < count; i++)
                                {
                                    this.m_ChallengeArea[i] = new Rectangle3D(reader.ReadPoint3D(), reader.ReadPoint3D());
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
                m.SendMessage("Define the Challenge area");
                this.DefineChallengeArea(m);
            }
        }

        public void DefineChallengeArea(Mobile m)
        {
            BoundingBoxPicker.Begin(m, new BoundingBoxCallback(ChallengeRegionArea_Callback), this);
        }

        public override void OnDelete()
        {
            if (this.m_ChallengeRegion != null)
                this.m_ChallengeRegion.Unregister();

            base.OnDelete();
        }

        private static void ChallengeRegionArea_Callback(Mobile from, Map map, Point3D start, Point3D end, object state)
        {
            // assign these coords to the region
            ChallengeRegionStone s = state as ChallengeRegionStone;

            if (s != null && from != null)
            {
                s.m_ChallengeArea = new Rectangle3D[1];
                s.m_ChallengeArea[0] = Region.ConvertTo3D(new Rectangle2D(start.X, start.Y, end.X - start.X + 1, end.Y - start.Y + 1));
                s.m_ChallengeMap = map;

                s.CopyRegion = null;

                s.RefreshRegions();
            }
        }
    }
}