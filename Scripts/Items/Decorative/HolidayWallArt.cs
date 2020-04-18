namespace Server.Items
{
    public class BaseHolidayWallArt : BaseLight, IFlipable
    {
        public override int LabelNumber => 1126181;  // glass tree

        private string _DisplayName;

        [CommandProperty(AccessLevel.GameMaster)]
        public string DisplayName { get { return _DisplayName; } set { _DisplayName = value; InvalidateProperties(); } }

        public override int LitItemID { get; }
        public override int UnlitItemID { get; }

        public virtual int NorthID { get; }
        public virtual int WestID { get; }

        [Constructable]
        public BaseHolidayWallArt(int ItemID)
            : base(ItemID)
        {
            _DisplayName = _Names[Utility.Random(_Names.Length)];
            Burning = false;
            Light = LightType.Circle225;
            Weight = 1.0;
        }

        public void OnFlip(Mobile from)
        {
            if (ItemID == NorthID)
                ItemID = WestID;
            else if (ItemID == WestID)
                ItemID = NorthID;
        }

        private static readonly string[] _Names =
        {
            "Minoc", "Britain", "Heartwood", "Empath Abbey", "The Lycaeum", "New Haven", "New Magincia", "Eodon", "Luna", "Delucia", "Buccaneer's Den", "Trinsic",
            "Wind", "Jhelom", "Zento", "Nujel'm", "Papua", "Moonglow", "Minoc", "Skara Brae"
        };

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (!string.IsNullOrEmpty(_DisplayName))
            {
                list.Add(1159260, _DisplayName); // Crafted By A Glassblower From ~1_WHERE~
            }
        }

        public BaseHolidayWallArt(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write(_DisplayName);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            _DisplayName = reader.ReadString();
        }
    }

    public class HolidayWallArt1 : BaseHolidayWallArt
    {
        public override int LitItemID => ItemID == 0xA4B3 ? 0xA4B4 : 0xA4B6;
        public override int UnlitItemID => ItemID == 0xA4B4 ? 0xA4B3 : 0xA4B5;

        public override int NorthID => Burning ? 0xA4B4 : 0xA4B3;
        public override int WestID => Burning ? 0xA4B6 : 0xA4B5;

        [Constructable]
        public HolidayWallArt1()
            : base(0xA4B3)
        {
        }

        public HolidayWallArt1(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class HolidayWallArt2 : BaseHolidayWallArt
    {
        public override int LitItemID => ItemID == 0xA4B7 ? 0xA4B8 : 0xA4BA;
        public override int UnlitItemID => ItemID == 0xA4B8 ? 0xA4B7 : 0xA4B9;

        public override int NorthID => Burning ? 0xA4B8 : 0xA4B7;
        public override int WestID => Burning ? 0xA4BA : 0xA4B9;

        [Constructable]
        public HolidayWallArt2()
            : base(0xA4B7)
        {
        }

        public HolidayWallArt2(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class HolidayWallArt3 : BaseHolidayWallArt
    {
        public override int LitItemID => ItemID == 0xA4AD ? 0xA4AE : 0xA4B0;
        public override int UnlitItemID => ItemID == 0xA4AE ? 0xA4AD : 0xA4AF;

        public override int NorthID => Burning ? 0xA4AE : 0xA4AD;
        public override int WestID => Burning ? 0xA4B0 : 0xA4AF;

        [Constructable]
        public HolidayWallArt3()
            : base(0xA4AD)
        {
        }

        public HolidayWallArt3(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
