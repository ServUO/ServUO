namespace Server.Items
{
    public class FireflyGarland : BaseLight, IFlipable
    {
        private string _DisplayName;

        [CommandProperty(AccessLevel.GameMaster)]
        public string DisplayName { get => _DisplayName; set { _DisplayName = value; InvalidateProperties(); } }

        public override int LabelNumber => 1124978; // Firefly Garland

        public override int LitItemID => ItemID == 0x9FFA ? 0x9FFB : 0xA000;
        public override int UnlitItemID => ItemID == 0x9FFB ? 0x9FFA : 0x9FFF;

        public int NorthID => Burning ? 0x9FFB : 0x9FFA;
        public int WestID => Burning ? 0xA000 : 0x9FFF;

        [Constructable]
        public FireflyGarland()
            : base(0x9FFA)
        {
            _DisplayName = _Names[Utility.Random(_Names.Length)];
            Light = LightType.Circle150;
            Burning = false;
        }

        public FireflyGarland(Serial serial)
            : base(serial)
        {
        }

        public void OnFlip(Mobile m)
        {
            if (ItemID == NorthID)
                ItemID = WestID;
            else if (ItemID == WestID)
                ItemID = NorthID;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (!string.IsNullOrEmpty(_DisplayName))
            {
                list.Add(1157970, _DisplayName); // <BASEFONT COLOR=#2DDC1B>Hand Strung by ~1_NAME~<BASEFONT COLOR=#FFFFFF>
            }
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
            reader.ReadInt();

            _DisplayName = reader.ReadString();
        }

        private static readonly string[] _Names =
        {
            "Inebriated Ice Fiends", "Goofy Gazers", "Yappy Yamandons", "Jolly Jukas", "Rambunctious Ratmen", "Spunky Skeletons", "Venerable Vollems", "Quirky Kappas"
        };
    }
}
