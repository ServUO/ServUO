using Server.Gumps;

namespace Server.Items
{
    public enum MetalLadderType
    {
        SouthCastle = 0,
        EastCastle = 1,
        NorthCastle = 2,
        WestCastle = 3,
        South = 4,
        East = 5,
        North = 6,
        West = 7
    }

    public class MetalLadderAddon : BaseAddon, IDyable
    {
        public override BaseAddonDeed Deed => new MetalLadderDeed();

        [Constructable]
        public MetalLadderAddon(MetalLadderType type)
        {
            switch (type)
            {
                case MetalLadderType.SouthCastle:
                    AddComponent(new LocalizedAddonComponent(0x3F28, 1076791), 0, 1, 28);
                    AddComponent(new LocalizedAddonComponent(0xA559, 1076791), 0, 2, 20);
                    AddComponent(new AddonComponent(0xA557), 0, 0, 0);
                    break;
                case MetalLadderType.EastCastle:
                    AddComponent(new LocalizedAddonComponent(0x3F28, 1076791), 1, 0, 28);
                    AddComponent(new LocalizedAddonComponent(0xA55A, 1076791), 2, 0, 20);
                    AddComponent(new AddonComponent(0xA558), 0, 0, 0);
                    break;
                case MetalLadderType.NorthCastle:
                    AddComponent(new LocalizedAddonComponent(0x3F28, 1076791), 0, -1, 28);
                    AddComponent(new LocalizedAddonComponent(0x3DB6, 1076791), 0, -2, 20);
                    AddComponent(new LocalizedAddonComponent(0xA55C, 1076791), 0, 0, 0);
                    break;
                case MetalLadderType.WestCastle:
                    AddComponent(new LocalizedAddonComponent(0x3F28, 1076791), -1, 0, 28);
                    AddComponent(new LocalizedAddonComponent(0x3DB7, 1076791), -2, 0, 20);
                    AddComponent(new LocalizedAddonComponent(0xA55B, 1076791), 0, 0, 0);
                    break;
                case MetalLadderType.South:
                    AddComponent(new LocalizedAddonComponent(0xA557, 1076287), 0, 0, 0);
                    break;
                case MetalLadderType.East:
                    AddComponent(new LocalizedAddonComponent(0xA558, 1076287), 0, 0, 0);
                    break;
                case MetalLadderType.North:
                    AddComponent(new LocalizedAddonComponent(0xA55C, 1076287), 0, 0, 0);
                    break;
                case MetalLadderType.West:
                    AddComponent(new LocalizedAddonComponent(0xA55B, 1076287), 0, 0, 0);
                    break;
            }
        }

        public virtual bool Dye(Mobile from, DyeTub sender)
        {
            if (Deleted)
                return false;

            Hue = sender.DyedHue;
            return true;
        }

        public MetalLadderAddon(Serial serial)
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

    public class MetalLadderDeed : BaseAddonDeed, IRewardOption
    {
        public override BaseAddon Addon => new MetalLadderAddon(_Direction);
        public override int LabelNumber => 1159445;  // metal ladder

        private MetalLadderType _Direction;

        [Constructable]
        public MetalLadderDeed()
        {
			LootType = LootType.Blessed;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                from.CloseGump(typeof(AddonOptionGump));
                from.SendGump(new AddonOptionGump(this, 1154194, 300, 260)); // Choose a Facing:
            }
            else
            {
                from.SendLocalizedMessage(1062334); // This item must be in your backpack to be used.
            }
        }

        public void GetOptions(RewardOptionList list)
        {
            list.Add((int)MetalLadderType.SouthCastle, 1076794);
            list.Add((int)MetalLadderType.EastCastle, 1076795);
            list.Add((int)MetalLadderType.NorthCastle, 1076792);
            list.Add((int)MetalLadderType.WestCastle, 1076793);
            list.Add((int)MetalLadderType.South, 1075386);
            list.Add((int)MetalLadderType.East, 1075387);
            list.Add((int)MetalLadderType.North, 1075389);
            list.Add((int)MetalLadderType.West, 1075390);
        }

        public void OnOptionSelected(Mobile from, int choice)
        {
            _Direction = (MetalLadderType)choice;

            if (!Deleted)
                base.OnDoubleClick(from);
        }

        public MetalLadderDeed(Serial serial)
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
