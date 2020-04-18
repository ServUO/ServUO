namespace Server.Items
{
    public class JollyHolidayTreeAddon : BaseAddon
    {
        public override BaseAddonDeed Deed => new JollyHolidayTreeDeed(DisplayName);
        public override bool ForceShowProperties => true;

        private string _DisplayName;

        [CommandProperty(AccessLevel.GameMaster)]
        public string DisplayName { get { return _DisplayName; } set { _DisplayName = value; InvalidateProperties(); } }

        [Constructable]
        public JollyHolidayTreeAddon(string name)
        {
            DisplayName = name;

            AddComponent(new JollyHolidayTreeComponent(0xA4D0, 1124426), 0, 0, 0);
        }

        private class JollyHolidayTreeComponent : LocalizedAddonComponent, IDyable
        {
            public JollyHolidayTreeComponent(int id, int labelnumber)
                : base(id, labelnumber)
            {
            }

            public bool Dye(Mobile from, DyeTub sender)
            {
                if (Deleted)
                    return false;

                if (Addon != null)
                    Addon.Hue = sender.DyedHue;

                return true;
            }

            public override void GetProperties(ObjectPropertyList list)
            {
                base.GetProperties(list);

                string name = ((JollyHolidayTreeAddon)Addon).DisplayName;

                if (!string.IsNullOrEmpty(name))
                {
                    list.Add(1159271, name); // <BASEFONT COLOR=#FFD24D>Jolly Holiday Tree from ~1_NAME~<BASEFONT COLOR=#FFFFFF>
                }
            }

            public JollyHolidayTreeComponent(Serial serial)
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

        public JollyHolidayTreeAddon(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version

            writer.Write(_DisplayName);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            _DisplayName = reader.ReadString();
        }
    }

    public class JollyHolidayTreeDeed : BaseAddonDeed
    {
        public override BaseAddon Addon => new JollyHolidayTreeAddon(DisplayName);
        public override int LabelNumber => 1159256;  // Jolly Holiday Tree

        private string _DisplayName;

        [CommandProperty(AccessLevel.GameMaster)]
        public string DisplayName { get { return _DisplayName; } set { _DisplayName = value; InvalidateProperties(); } }

        [Constructable]
        public JollyHolidayTreeDeed()
            : this(null)
        {
        }

        [Constructable]
        public JollyHolidayTreeDeed(string name)
        {
            LootType = LootType.Blessed;

            if (name == null)
                _DisplayName = _Names[Utility.Random(_Names.Length)];
            else
                _DisplayName = name;
        }

        public static string[] _Names =
        {
            "Cursed Powder Obsidian", "Lover Noggin Artemis", "Cursed Tooth Aeon", "Short Noggin Takako", "Stink Hook Ashmedia", "Iron Eye Greg", "Mad Back Riccia",
            "Mad Blade Vereor", "Bloody Ear Aumakua", "Long Powder Jim", "Lazy Tongue Silver Fox", "Lazy Pants Hanarin", "Fat Hook Bonnie", "Short Hook Michael", "Bonny Eye Elizabella",
            "Glass Blade Artemis", "Stink Strap Raine", "Bloody Tongue Aumakua", "Bloody Tooth Asiantam", "Short Blade Topaz", "Lazy Beard Raine", "Drunk Nail Takako", "Rude Powder Obsidian",
            "Peg Mouth Thrixx", "Iron Hook Stephen", "Lost Nail Tori", "Lone Patch Michael", "Burnin Nail Aumakua"
        };

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (!string.IsNullOrEmpty(_DisplayName))
            {
                list.Add(1159271, _DisplayName); // <BASEFONT COLOR=#FFD24D>Jolly Holiday Tree from ~1_NAME~<BASEFONT COLOR=#FFFFFF>
            }
        }

        public JollyHolidayTreeDeed(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version

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
