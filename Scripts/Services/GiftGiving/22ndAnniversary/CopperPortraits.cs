namespace Server.Items
{
    [Flipable(0xA3E0, 0xA3E4)]
    public class CopperPortrait1 : Item
    {
        private string _DisplayName;

        [CommandProperty(AccessLevel.GameMaster)]
        public string DisplayName { get { return _DisplayName; } set { _DisplayName = value; InvalidateProperties(); } }

        public override int LabelNumber => 1159154;  // Copper Portrait

        [Constructable]
        public CopperPortrait1()
            : base(0xA3E0)
        {
            _DisplayName = _Names[Utility.Random(_Names.Length)];
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (!string.IsNullOrEmpty(_DisplayName))
            {
                list.Add(1159151, _DisplayName); // <BASEFONT COLOR=#FFD24D>Relief of ~1_NAME~<BASEFONT COLOR=#FFFFFF>
            }

            if (Hue == 2951)
                list.Add(1076187); // Antique
        }

        public CopperPortrait1(Serial serial)
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

        public static string[] _Names =
        {
            "Long Tooth Riccia", "Long Leg Topaz", "Glass Tongue Takako", "Iron Fist Riccia", "Fat Eye Takako", "Bloody Back Greg", "Cursed Powder Mercury", "Lone Tongue Erebus", "Mad Powder Sarah", "Long Beard Jim",
            "Lazy Eye Thrixx", "Cursed Patch Artemis", "Mad Back Aeon", "Glass Tooth Asiantam", "Iron Mouth Artemis", "Stink Back Elizabella", "Lost Blade Mercury", "Lazy Mouth Malachi", "Glass Back Nekomata",
            "Tooth Silver Fox"
        };
    }

    [Flipable(0xA3E3, 0xA3E7)]
    public class CopperPortrait2 : Item
    {
        private string _DisplayName;

        [CommandProperty(AccessLevel.GameMaster)]
        public string DisplayName { get { return _DisplayName; } set { _DisplayName = value; InvalidateProperties(); } }

        public override int LabelNumber => 1159154;  // Copper Portrait

        [Constructable]
        public CopperPortrait2()
            : base(0xA3E3)
        {
            _DisplayName = CopperPortrait1._Names[Utility.Random(CopperPortrait1._Names.Length)];
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (!string.IsNullOrEmpty(_DisplayName))
            {
                list.Add(1159151, _DisplayName); // <BASEFONT COLOR=#FFD24D>Relief of ~1_NAME~<BASEFONT COLOR=#FFFFFF>
            }

            if (Hue == 2951)
                list.Add(1076187); // Antique
        }

        public CopperPortrait2(Serial serial)
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
}
