using System;
using Server.Multis;
using Server.Prompts;
using Server.Regions;

namespace Server.Items
{
    [FlipableAttribute(0x1f14, 0x1f15, 0x1f16, 0x1f17)]
    public class RecallRune : Item
    {
        private const string RuneFormat = "a recall rune for {0}";
        private string m_Description;
        private bool m_Marked;
        private Point3D m_Target;
        private Map m_TargetMap;
        private BaseHouse m_House;
        [Constructable]
        public RecallRune()
            : base(0x1F14)
        {
            this.Weight = 1.0;
            this.CalculateHue();
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
                if (this.m_House != null && this.m_House.Deleted)
                    this.House = null;

                return this.m_House;
            }
            set
            {
                this.m_House = value;
                this.CalculateHue();
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
        public string Description
        {
            get
            {
                return this.m_Description;
            }
            set
            {
                this.m_Description = value;
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
        public bool Marked
        {
            get
            {
                return this.m_Marked;
            }
            set
            {
                if (this.m_Marked != value)
                {
                    this.m_Marked = value;
                    this.CalculateHue();
                    this.InvalidateProperties();
                }
            }
        }
        [CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
        public Point3D Target
        {
            get
            {
                return this.m_Target;
            }
            set
            {
                this.m_Target = value;
            }
        }
        [CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
        public Map TargetMap
        {
            get
            {
                return this.m_TargetMap;
            }
            set
            {
                if (this.m_TargetMap != value)
                {
                    this.m_TargetMap = value;
                    this.CalculateHue();
                    this.InvalidateProperties();
                }
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            if (this.m_House != null && !this.m_House.Deleted)
            {
                writer.Write((int)1); // version

                writer.Write((Item)this.m_House);
            }
            else
            {
                writer.Write((int)0); // version
            }

            writer.Write((string)this.m_Description);
            writer.Write((bool)this.m_Marked);
            writer.Write((Point3D)this.m_Target);
            writer.Write((Map)this.m_TargetMap);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 1:
                    {
                        this.m_House = reader.ReadItem() as BaseHouse;
                        goto case 0;
                    }
                case 0:
                    {
                        this.m_Description = reader.ReadString();
                        this.m_Marked = reader.ReadBool();
                        this.m_Target = reader.ReadPoint3D();
                        this.m_TargetMap = reader.ReadMap();

                        this.CalculateHue();

                        break;
                    }
            }
        }

        public void Mark(Mobile m)
        {
            this.m_Marked = true;

            bool setDesc = false;
            if (Core.AOS)
            {
                this.m_House = BaseHouse.FindHouseAt(m);

                if (this.m_House == null)
                {
                    this.m_Target = m.Location;
                    this.m_TargetMap = m.Map;
                }
                else
                {
                    HouseSign sign = this.m_House.Sign;

                    if (sign != null)
                        this.m_Description = sign.Name;
                    else
                        this.m_Description = null;

                    if (this.m_Description == null || (this.m_Description = this.m_Description.Trim()).Length == 0)
                        this.m_Description = "an unnamed house";

                    setDesc = true;

                    int x = this.m_House.BanLocation.X;
                    int y = this.m_House.BanLocation.Y + 2;
                    int z = this.m_House.BanLocation.Z;

                    Map map = this.m_House.Map;

                    if (map != null && !map.CanFit(x, y, z, 16, false, false))
                        z = map.GetAverageZ(x, y);

                    this.m_Target = new Point3D(x, y, z);
                    this.m_TargetMap = map;
                }
            }
            else
            {
                this.m_House = null;
                this.m_Target = m.Location;
                this.m_TargetMap = m.Map;
            }

            if (!setDesc)
                this.m_Description = BaseRegion.GetRuneNameFor(Region.Find(this.m_Target, this.m_TargetMap));

            this.CalculateHue();
            this.InvalidateProperties();
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (this.m_Marked)
            {
                string desc;

                if ((desc = this.m_Description) == null || (desc = desc.Trim()).Length == 0)
                    desc = "an unknown location";

                if (this.m_TargetMap == Map.Tokuno)
                    list.Add((this.House != null ? 1063260 : 1063259), RuneFormat, desc); // ~1_val~ (Tokuno Islands)[(House)]
                else if (this.m_TargetMap == Map.Malas)
                    list.Add((this.House != null ? 1062454 : 1060804), RuneFormat, desc); // ~1_val~ (Malas)[(House)]
                else if (this.m_TargetMap == Map.Felucca)
                    list.Add((this.House != null ? 1062452 : 1060805), RuneFormat, desc); // ~1_val~ (Felucca)[(House)]
                else if (this.m_TargetMap == Map.Trammel)
                    list.Add((this.House != null ? 1062453 : 1060806), RuneFormat, desc); // ~1_val~ (Trammel)[(House)]
                else
                    list.Add((this.House != null ? "{0} ({1})(House)" : "{0} ({1})"), String.Format(RuneFormat, desc), this.m_TargetMap);
            }
        }

        public override void OnSingleClick(Mobile from)
        {
            if (this.m_Marked)
            {
                string desc;

                if ((desc = this.m_Description) == null || (desc = desc.Trim()).Length == 0)
                    desc = "an unknown location";

                if (this.m_TargetMap == Map.Tokuno)
                    this.LabelTo(from, (this.House != null ? 1063260 : 1063259), String.Format(RuneFormat, desc)); // ~1_val~ (Tokuno Islands)[(House)]
                else if (this.m_TargetMap == Map.Malas)
                    this.LabelTo(from, (this.House != null ? 1062454 : 1060804), String.Format(RuneFormat, desc)); // ~1_val~ (Malas)[(House)]
                else if (this.m_TargetMap == Map.Felucca)
                    this.LabelTo(from, (this.House != null ? 1062452 : 1060805), String.Format(RuneFormat, desc)); // ~1_val~ (Felucca)[(House)]
                else if (this.m_TargetMap == Map.Trammel)
                    this.LabelTo(from, (this.House != null ? 1062453 : 1060806), String.Format(RuneFormat, desc)); // ~1_val~ (Trammel)[(House)]
                else
                    this.LabelTo(from, (this.House != null ? "{0} ({1})(House)" : "{0} ({1})"), String.Format(RuneFormat, desc), this.m_TargetMap);
            }
            else
            {
                this.LabelTo(from, "an unmarked recall rune");
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            int number;

            if (!this.IsChildOf(from.Backpack))
            {
                number = 1042001; // That must be in your pack for you to use it.
            }
            else if (this.House != null)
            {
                number = 1062399; // You cannot edit the description for this rune.
            }
            else if (this.m_Marked)
            {
                number = 501804; // Please enter a description for this marked object.

                from.Prompt = new RenamePrompt(this);
            }
            else
            {
                number = 501805; // That rune is not yet marked.
            }

            from.SendLocalizedMessage(number);
        }

        private void CalculateHue()
        {
            if (!this.m_Marked)
                this.Hue = 0;
            else if (this.m_TargetMap == Map.Trammel)
                this.Hue = (this.House != null ? 0x47F : 50);
            else if (this.m_TargetMap == Map.Felucca)
                this.Hue = (this.House != null ? 0x66D : 0);
            else if (this.m_TargetMap == Map.Ilshenar)
                this.Hue = (this.House != null ? 0x55F : 1102);
            else if (this.m_TargetMap == Map.Malas)
                this.Hue = (this.House != null ? 0x55F : 1102);
            else if (this.m_TargetMap == Map.Tokuno)
                this.Hue = (this.House != null ? 0x47F : 1154);
        }

        private class RenamePrompt : Prompt
        {
            // Please enter a description for this marked object.
            public override int MessageCliloc { get { return 501804; } }
            private readonly RecallRune m_Rune;
            public RenamePrompt(RecallRune rune)
            {
                this.m_Rune = rune;
            }

            public override void OnResponse(Mobile from, string text)
            {
                if (this.m_Rune.House == null && this.m_Rune.Marked)
                {
                    this.m_Rune.Description = text;
                    from.SendLocalizedMessage(1010474); // The etching on the rune has been changed.
                }
            }
        }
    }
}