namespace Server.Items
{
    [Flipable(0xA35D, 0xA35E)]
    public class CopperSunflower : Item
    {
        private string _DisplayName;

        [CommandProperty(AccessLevel.GameMaster)]
        public string DisplayName { get { return _DisplayName; } set { _DisplayName = value; InvalidateProperties(); } }

        public override int LabelNumber => 1159149;  // Copper Sunflower

        [Constructable]
        public CopperSunflower()
            : base(0xA35D)
        {
            _DisplayName = _Names[Utility.Random(_Names.Length)];
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (!string.IsNullOrEmpty(_DisplayName))
            {
                list.Add(1159150, _DisplayName); // <BASEFONT COLOR=#FFD24D>Cast from Flowers Grown in The Warm Sun of ~1_NAME~<BASEFONT COLOR=#FFFFFF>
            }

            if (Hue == 2951)
                list.Add(1076187); // Antique
        }

        public CopperSunflower(Serial serial)
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

        private static readonly string[] _Names =
        {
            "Trinsic", "Jhelom", "Vesper", "Ocllo", "Yew", "Britain", "Minoc", "Moonglow", "Skara Brae", "Delucia"
        };
    }
}
