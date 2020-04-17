using Server.Multis;
using Server.Prompts;
using Server.Regions;

namespace Server.Items
{
    public enum RecallRuneType
    {
        Normal,
        Shop,
        Ship
    }

    [Flipable(0x1f14, 0x1f15, 0x1f16, 0x1f17)]
    public class RecallRune : Item
    {
        public override int LabelNumber => Type == RecallRuneType.Normal ? 1060577 : Type == RecallRuneType.Shop ? 1151508 : 1149570;  // Recall Rune - Shop Recall Rune - Ship Recall Rune

        private const string RuneFormat = "a recall rune for {0}";
        private string m_Description;
        private bool m_Marked;
        private Map m_TargetMap;
        private BaseHouse m_House;
        private BaseGalleon m_Galleon;

        [CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
        public RecallRuneType Type { get; set; }

        [Constructable]
        public RecallRune()
            : base(0x1F14)
        {
            Weight = 1.0;
            Type = RecallRuneType.Normal;
            CalculateHue();
        }

        public RecallRune(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
        public BaseHouse House
        {
            get
            {
                if (m_House != null && m_House.Deleted)
                    House = null;

                return m_House;
            }
            set
            {
                m_House = value;

                if (value != null)
                {
                    Type = RecallRuneType.Shop;
                }

                CalculateHue();
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
        public BaseGalleon Galleon
        {
            get
            {
                if (m_Galleon != null && m_Galleon.Deleted)
                    Galleon = null;

                return m_Galleon;
            }
            set
            {
                m_Galleon = value;

                if (value != null)
                {
                    Type = RecallRuneType.Ship;
                }

                CalculateHue();
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
        public string Description
        {
            get { return m_Description; }
            set
            {
                m_Description = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
        public bool Marked
        {
            get { return m_Marked; }
            set
            {
                if (m_Marked != value)
                {
                    m_Marked = value;
                    CalculateHue();
                    InvalidateProperties();
                }
            }
        }

        [CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
        public Point3D Target { get; set; }

        [CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
        public Map TargetMap
        {
            get { return m_TargetMap; }
            set
            {
                if (m_TargetMap != value)
                {
                    m_TargetMap = value;
                    CalculateHue();
                    InvalidateProperties();
                }
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(2); // version

            writer.Write((int)Type);
            writer.Write(m_Galleon);
            writer.Write(m_House);
            writer.Write(m_Description);
            writer.Write(m_Marked);
            writer.Write(Target);
            writer.Write(m_TargetMap);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            switch (version)
            {
                case 2:
                    {
                        Type = (RecallRuneType)reader.ReadInt();
                        m_Galleon = reader.ReadItem() as BaseGalleon;
                        m_House = reader.ReadItem() as BaseHouse;
                        m_Description = reader.ReadString();
                        m_Marked = reader.ReadBool();
                        Target = reader.ReadPoint3D();
                        m_TargetMap = reader.ReadMap();

                        break;
                    }
                case 1:
                    {
                        m_House = reader.ReadItem() as BaseHouse;
                        goto case 0;
                    }
                case 0:
                    {
                        m_Description = reader.ReadString();
                        m_Marked = reader.ReadBool();
                        Target = reader.ReadPoint3D();
                        m_TargetMap = reader.ReadMap();

                        break;
                    }
            }
        }

        public void SetGalleon(BaseGalleon galleon)
        {
            m_Marked = true;
            Galleon = galleon;
        }

        public void Mark(Mobile m)
        {
            RecallRuneEmpty();

            m_Marked = true;

            bool setDesc = false;

            m_Galleon = BaseBoat.FindBoatAt(m) as BaseGalleon;

            if (m_Galleon != null)
            {
                Type = RecallRuneType.Ship;
            }
            else
            {
                m_House = BaseHouse.FindHouseAt(m);

                if (m_House == null)
                {
                    Target = m.Location;
                    m_TargetMap = m.Map;

                    Type = RecallRuneType.Normal;
                }
                else
                {
                    HouseSign sign = m_House.Sign;

                    if (sign != null)
                        m_Description = sign.Name;
                    else
                        m_Description = null;

                    if (m_Description == null || (m_Description = m_Description.Trim()).Length == 0)
                        m_Description = "an unnamed house";

                    setDesc = true;

                    int x = m_House.BanLocation.X;
                    int y = m_House.BanLocation.Y + 2;
                    int z = m_House.BanLocation.Z;

                    Map map = m_House.Map;

                    if (map != null && !map.CanFit(x, y, z, 16, false, false))
                        z = map.GetAverageZ(x, y);

                    Target = new Point3D(x, y, z);
                    m_TargetMap = map;

                    Type = RecallRuneType.Shop;
                }
            }

            if (!setDesc)
                m_Description = BaseRegion.GetRuneNameFor(Region.Find(Target, m_TargetMap));

            CalculateHue();
            InvalidateProperties();
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (m_Marked)
            {
                if (Type == RecallRuneType.Ship)
                {
                    if (Galleon != null)
                    {
                        if (Galleon.Owner != null)
                        {
                            list.Add(1149571, Galleon.Owner.Name); // Owner: ~1_NAME~
                        }
                        else
                        {
                            list.Add(1150535); // Unknown Owner
                        }

                        if (Galleon.ShipName != null)
                        {
                            list.Add(1149572, Galleon.ShipName); // Name: the ~1_NAME~
                        }
                        else
                        {
                            list.Add(1149573); // Name: the Unnamed Ship
                        }

                        if (Galleon.Map != Map.Internal && Galleon.Map != null)
                        {
                            list.Add(1149574, Galleon.Map.ToString()); // Location: ~1_FACET~
                        }
                        else
                        {
                            list.Add(1149574, "#1149575"); // Location: Dry Dock
                        }
                    }
                    else
                    {
                        list.Add(1150535); // Unknown Owner
                        list.Add(1149573); // Name: the Unnamed Ship
                        list.Add(1149574, "#1149575"); // Location: Dry Dock
                    }
                }
                else
                {
                    string desc;

                    if ((desc = m_Description) == null || (desc = desc.Trim()).Length == 0)
                        desc = "an unknown location";

                    if (m_TargetMap == Map.Tokuno)
                        list.Add((House != null ? 1063260 : 1063259), RuneFormat, desc); // ~1_val~ (Tokuno Islands)[(House)]
                    else if (m_TargetMap == Map.Malas)
                        list.Add((House != null ? 1062454 : 1060804), RuneFormat, desc); // ~1_val~ (Malas)[(House)]
                    else if (m_TargetMap == Map.Felucca)
                        list.Add((House != null ? 1062452 : 1060805), RuneFormat, desc); // ~1_val~ (Felucca)[(House)]
                    else if (m_TargetMap == Map.Trammel)
                        list.Add((House != null ? 1062453 : 1060806), RuneFormat, desc); // ~1_val~ (Trammel)[(House)]
                    else if (m_TargetMap == Map.TerMur)
                        list.Add((House != null ? 1113206 : 1113205), RuneFormat, desc); // ~1_val~ (Ter Mur)(House)
                    else
                        list.Add((House != null ? "{0} ({1})(House)" : "{0} ({1})"), string.Format(RuneFormat, desc), m_TargetMap);
                }
            }
        }

        public void RecallRuneEmpty()
        {
            m_Description = null;
            m_Galleon = null;
            m_House = null;
            Target = Point3D.Zero;
            m_TargetMap = null;
            Type = RecallRuneType.Normal;
            m_Marked = false;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (Type == RecallRuneType.Ship)
            {
                if (Galleon == null)
                {
                    RecallRuneEmpty();
                    CalculateHue();
                }

                InvalidateProperties();
                return;
            }

            int number;

            if (!IsChildOf(from.Backpack))
            {
                number = 1042001; // That must be in your pack for you to use it.
            }
            else if (House != null)
            {
                number = 1062399; // You cannot edit the description for this rune.
            }
            else if (m_Marked)
            {
                number = 0;

                from.Prompt = new RenamePrompt(this);
            }
            else
            {
                number = 501805; // That rune is not yet marked.
            }

            if (number > 0)
                from.SendLocalizedMessage(number);
        }

        private void CalculateHue()
        {
            int hue = 0;

            if (Type == RecallRuneType.Ship)
            {
                hue = 1151;
            }
            else
            {
                hue = CalculateHue(m_TargetMap, House, m_Marked);
            }

            Hue = hue;
        }

        public static int CalculateHue(Map map, BaseHouse house, bool mark)
        {
            int hue = 0;

            if (mark)
            {
                if (map == Map.Trammel)
                    hue = (house != null ? 0x47F : 50);
                else if (map == Map.Felucca)
                    hue = (house != null ? 0x66D : 0);
                else if (map == Map.Ilshenar)
                    hue = (house != null ? 0x55F : 1102);
                else if (map == Map.Malas)
                    hue = (house != null ? 0x55F : 1102);
                else if (map == Map.Tokuno)
                    hue = (house != null ? 0x1F14 : 1154);
                else if (map == Map.TerMur)
                    hue = 1162;
            }

            return hue;
        }

        private class RenamePrompt : Prompt
        {
            public override int MessageCliloc => 501804;
            private readonly RecallRune m_Rune;

            public RenamePrompt(RecallRune rune)
            {
                m_Rune = rune;
            }

            public override void OnResponse(Mobile from, string text)
            {
                if (m_Rune.House == null && m_Rune.Marked)
                {
                    m_Rune.Description = text;
                    from.SendLocalizedMessage(1010474); // The etching on the rune has been changed.
                }
            }
        }
    }
}
