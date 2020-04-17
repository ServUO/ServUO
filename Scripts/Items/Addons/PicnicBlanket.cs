using Server.Gumps;

namespace Server.Items
{
    public class PicnicBlanketAddon : BaseAddon
    {
        [Constructable]
        public PicnicBlanketAddon(DirectionType type)
        {
            switch (type)
            {
                case DirectionType.South:
                    AddComponent(new PicnicBlanketComponent(41168), 1, 1, 0);
                    AddComponent(new PicnicBlanketComponent(41169), 1, 0, 0);
                    AddComponent(new PicnicBlanketComponent(41170), 0, 1, 0);
                    AddComponent(new PicnicBlanketComponent(41171), 0, 0, 0);
                    AddComponent(new PicnicBlanketComponent(41172), -1, 1, 0);
                    AddComponent(new PicnicBlanketComponent(41173), -1, 0, 0);
                    break;
                case DirectionType.East:
                    AddComponent(new PicnicBlanketComponent(41163), 0, 1, 0);
                    AddComponent(new PicnicBlanketComponent(41162), 1, 1, 0);
                    AddComponent(new PicnicBlanketComponent(41165), 0, 0, 0);
                    AddComponent(new PicnicBlanketComponent(41164), 1, 0, 0);
                    AddComponent(new PicnicBlanketComponent(41166), 1, -1, 0);
                    AddComponent(new PicnicBlanketComponent(41167), 0, -1, 0);
                    break;
            }
        }

        public PicnicBlanketAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed => new PicnicBlanketDeed();

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

    public class PicnicBlanketComponent : LocalizedAddonComponent, IDyable
    {
        public PicnicBlanketComponent(int id)
            : base(id, 1125186) // blanket
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

        public PicnicBlanketComponent(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // Version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class PicnicBlanketDeed : BaseAddonDeed, IRewardOption
    {
        public override int LabelNumber => 1158332;  // picnic blanket

        public override BaseAddon Addon => new PicnicBlanketAddon(_Direction);

        private DirectionType _Direction;

        [Constructable]
        public PicnicBlanketDeed()
            : base()
        {
        }

        public PicnicBlanketDeed(Serial serial)
            : base(serial)
        {
        }

        public void GetOptions(RewardOptionList list)
        {
            list.Add((int)DirectionType.South, 1075386); // South
            list.Add((int)DirectionType.East, 1075387); // East
        }

        public void OnOptionSelected(Mobile from, int choice)
        {
            _Direction = (DirectionType)choice;

            if (!Deleted)
                base.OnDoubleClick(from);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                from.CloseGump(typeof(AddonOptionGump));
                from.SendGump(new AddonOptionGump(this, 1154194)); // Choose a Facing:
            }
            else
            {
                from.SendLocalizedMessage(1062334); // This item must be in your backpack to be used.
            }
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
