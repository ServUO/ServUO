using System;
using Server.Gumps;

namespace Server.Items
{
    public class MarbleTableAddon : BaseAddon, IDyable
    {
        [Constructable]
        public MarbleTableAddon(DirectionType type)
        {
            switch (type)
            {
                case DirectionType.East:
                    AddComponent(new LocalizedAddonComponent(19635, 1154152), 1, 0, 0);
                    AddComponent(new LocalizedAddonComponent(19634, 1154152), 0, 0, 0);
                    AddComponent(new LocalizedAddonComponent(19633, 1154152), 0, 1, 0);
                    AddComponent(new LocalizedAddonComponent(19632, 1154152), 1, 1, 0);
                    break;
                case DirectionType.South:
                    AddComponent(new LocalizedAddonComponent(19631, 1154152), 1, 1, 0);
                    AddComponent(new LocalizedAddonComponent(19630, 1154152), 0, 1, 0);
                    AddComponent(new LocalizedAddonComponent(19629, 1154152), 1, 0, 0);
                    AddComponent(new LocalizedAddonComponent(19628, 1154152), 0, 0, 0);
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

        public MarbleTableAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed { get { return new MarbleTableDeed(); } }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class MarbleTableDeed : BaseAddonDeed, IRewardOption
    {
        public override int LabelNumber { get { return 1154152; } } // Marble Table

        private DirectionType _Direction;

        [Constructable]
        public MarbleTableDeed()
            : base()
        {
            LootType = LootType.Blessed;
        }

        public MarbleTableDeed(Serial serial)
            : base(serial)
        {
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

        public override BaseAddon Addon { get { return new MarbleTableAddon(_Direction); } }
                
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
