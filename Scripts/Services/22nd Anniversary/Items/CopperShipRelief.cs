using Server.Gumps;

namespace Server.Items
{
    public class CopperShipReliefAddon : BaseAddon
    {
        public override BaseAddonDeed Deed => new CopperShipReliefAddonDeed(DisplayName);

        private string _DisplayName;

        [CommandProperty(AccessLevel.GameMaster)]
        public string DisplayName { get { return _DisplayName; } set { _DisplayName = value; InvalidateProperties(); } }

        public AddonFacing Facing { get; set; }

        [Constructable]
        public CopperShipReliefAddon(AddonFacing facing, string name)
        {
            DisplayName = name;
            Facing = facing;

            switch (facing)
            {
                case AddonFacing.South:
                    AddComponent(new CopperShipReliefComponent(41954), -1, 0, 0);
                    AddComponent(new CopperShipReliefComponent(41953), 1, 0, 0);
                    break;
                case AddonFacing.East:
                    AddComponent(new CopperShipReliefComponent(41957), 0, 1, 0);
                    AddComponent(new CopperShipReliefComponent(41958), 0, -1, 0);
                    break;

            }
        }

        private class CopperShipReliefComponent : LocalizedAddonComponent
        {
            public override bool ForceShowProperties => true;

            public CopperShipReliefComponent(int id)
                : base(id, 1159148) // Copper Ship Relief
            {
                Weight = 0;
            }

            public override void GetProperties(ObjectPropertyList list)
            {
                base.GetProperties(list);

                string name = ((CopperShipReliefAddon)Addon).DisplayName;

                if (!string.IsNullOrEmpty(name))
                {
                    list.Add(1159152, name); // <BASEFONT COLOR=#FFD24D>Depicting the Maiden Voyage of ~1_NAME~<BASEFONT COLOR=#FFFFFF>
                }

                if (Hue == 2951)
                    list.Add(1076187); // Antique
            }

            public CopperShipReliefComponent(Serial serial)
                : base(serial)
            {
            }

            public override void Serialize(GenericWriter writer)
            {
                base.Serialize(writer);
                writer.WriteEncodedInt(0); // version
            }

            public override void Deserialize(GenericReader reader)
            {
                base.Deserialize(reader);
                int version = reader.ReadEncodedInt();
            }
        }

        public CopperShipReliefAddon(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(1); // version

            writer.Write(_DisplayName);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();

            if (version == 0)
            {
                _DisplayName = CopperShipReliefAddonDeed._Names[Utility.Random(CopperShipReliefAddonDeed._Names.Length)];
            }
            else
            {
                _DisplayName = reader.ReadString();
            }
        }
    }

    public class CopperShipReliefAddonDeed : BaseAddonDeed, IRewardOption
    {
        public override BaseAddon Addon => new CopperShipReliefAddon(Facing, DisplayName);

        private string _DisplayName;

        [CommandProperty(AccessLevel.GameMaster)]
        public string DisplayName { get { return _DisplayName; } set { _DisplayName = value; InvalidateProperties(); } }

        private AddonFacing Facing { get; set; }

        public override int LabelNumber => 1159148;  // Copper Ship Relief

        [Constructable]
        public CopperShipReliefAddonDeed()
            : this(null)
        {
        }

        [Constructable]
        public CopperShipReliefAddonDeed(string name)
        {
            LootType = LootType.Blessed;

            if (name == null)
                _DisplayName = _Names[Utility.Random(_Names.Length)];
            else
                _DisplayName = name;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                from.CloseGump(typeof(AddonOptionGump));
                from.SendGump(new AddonOptionGump(this));
            }
            else
                from.SendLocalizedMessage(1062334); // This item must be in your backpack to be used.       	
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (!string.IsNullOrEmpty(_DisplayName))
            {
                list.Add(1159152, _DisplayName); // <BASEFONT COLOR=#FFD24D>Depicting the Maiden Voyage of ~1_NAME~<BASEFONT COLOR=#FFFFFF>
            }

            if (Hue == 2951)
                list.Add(1076187); // Antique
        }

        public CopperShipReliefAddonDeed(Serial serial)
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

        public void GetOptions(RewardOptionList list)
        {
            list.Add((int)AddonFacing.South, 1075386);
            list.Add((int)AddonFacing.East, 1075387);
        }


        public void OnOptionSelected(Mobile from, int choice)
        {
            Facing = (AddonFacing)choice;

            if (!Deleted)
                base.OnDoubleClick(from);
        }

        public static string[] _Names =
        {
            "The HMS Cape", "The Mustang", "The Dragon's Breath", "The Crown Jewel", "The Empire", "The Scaly Eel", "The Spartan", "The Beast", "The Ararat", "The Arabella", "The Lusty Wench", "The Golden Ankh",
            "The Poseidon's Fury", "The Silver Hart"
        };
    }
}
