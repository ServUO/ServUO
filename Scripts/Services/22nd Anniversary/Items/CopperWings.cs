namespace Server.Items
{
    [Flipable(0xA3DE, 0xA3DF)]
    public class CopperWings : Item
    {
        private string _DisplayName;

        [CommandProperty(AccessLevel.GameMaster)]
        public string DisplayName { get { return _DisplayName; } set { _DisplayName = value; InvalidateProperties(); } }

        public override int LabelNumber => 1159146;  // Copper Wings

        [Constructable]
        public CopperWings()
            : base(0xA3DE)
        {
            _DisplayName = _Names[Utility.Random(_Names.Length)];
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (!string.IsNullOrEmpty(_DisplayName))
            {
                list.Add(1159153, _DisplayName); // <BASEFONT COLOR=#FFD24D>Symbolizing Glory During the ~1_NAME~<BASEFONT COLOR=#FFFFFF>
            }

            if (Hue == 2951)
                list.Add(1076187); // Antique
        }

        public CopperWings(Serial serial)
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
            "Hook's Pirate War", "Endless Struggle Between Platinum And Crimson", "Ophidian War", "Battle Of The Bloody Plains", "Expedition Against Khal Ankur", "Evacuation Of Haven", "Defeat Of Virtuebane", "Siege Of Ver Lor Reg",
            "Assault On The Temple Of The Abyss", "Fall Of Trinsic", "Despise Onslaught"
        };
    }
}
