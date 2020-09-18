namespace Server.Items
{
    [Flipable(0xA5CE, 0xA5CF, 0xA5D0, 0xA5D1)]
    public class SilverPlatedTome : Item
    {
        private string _DisplayName;

        [CommandProperty(AccessLevel.GameMaster)]
        public string DisplayName { get { return _DisplayName; } set { _DisplayName = value; InvalidateProperties(); } }

        public override int LabelNumber => 1159509;  // Silver Plated Tome

        [Constructable]
        public SilverPlatedTome()
            : base(0xA5CE)
        {
            Hue = Utility.RandomMinMax(8, 254);
            _DisplayName = string.Format("{0} of {1}", _Titles[Utility.Random(_Titles.Length)], _Names[Utility.Random(_Names.Length)]);
        }

        public SilverPlatedTome(Serial serial)
            : base(serial)
        {
        }

        public override void AddNameProperty(ObjectPropertyList list)
        {
            if (!string.IsNullOrEmpty(_DisplayName))
            {
                list.Add(1159515, _DisplayName); // The Tale of the ~1_VAL~
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

        private static readonly string[] _Titles =
        {
            "Knight", "Marquis", "Earl", "Baroness", "Viscount", "Marquise", "Viscountess", "Citizen", "Baron", "Baronetess", "Countess"
        };

        private static readonly string[] _Names =
        {
            "Trinsic", "Jhelom", "Vesper", "Ocllo", "Yew", "Britain", "Minoc", "Moonglow", "Skara Brae", "Delucia", "New Magincia"
        };
    }
}
