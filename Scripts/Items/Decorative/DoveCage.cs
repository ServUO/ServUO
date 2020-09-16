namespace Server.Items
{
    [Flipable(0xA4B2, 0xA4B1)]
    public class DoveCage : Item, IDyable
    {
        public override int LabelNumber => 1126185;  // cage

        private string _DisplayName;

        [CommandProperty(AccessLevel.GameMaster)]
        public string DisplayName { get { return _DisplayName; } set { _DisplayName = value; InvalidateProperties(); } }

        [Constructable]
        public DoveCage()
            : base(0xA4B2)
        {
            _DisplayName = _Names[Utility.Random(_Names.Length)];
        }

        private static readonly string[] _Names =
        {
            "White-Winged", "Jobi", "Golden Heart", "White-Tipped", "Collared", "Common Ground", "Ruddy Quail", "Thick-Billed Ground", "Celebes Quail", "Dragon Turtle",
            "Crested Quail", "HeartWood", "Grey-Fronted", "Bronzewing", "Grey-Headed", "Brown Cuckoo", "Red Dragon Turtle", "Rock", "Namaqua", "Laughing", "Bar Shoulder",
            "Whistling", "Zenaida", "Red-Eyed", "Inca", "Eared", "Wood"
        };

        public bool Dye(Mobile from, DyeTub sender)
        {
            if (Deleted)
                return false;

            Hue = sender.DyedHue;

            return true;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (!string.IsNullOrEmpty(_DisplayName))
            {
                list.Add(1159261, _DisplayName); // A Cage With A Single ~1_WHERE~ Dove
            }
        }

        public DoveCage(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (!m.InRange(GetWorldLocation(), 3))
            {
                m.LocalOverheadMessage(Network.MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
            }
            else
            {
                Effects.PlaySound(m.Location, m.Map, 1663);
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
            int version = reader.ReadInt();

            _DisplayName = reader.ReadString();
        }
    }
}
