using System;
using Server;
using Server.Gumps;

namespace Server.Items
{
    public enum SquaredDoorMatType
    {
        South = 0,
        East = 1,
    }

    public class SquaredDoorMatAddon : BaseAddon
    {
        public override BaseAddonDeed Deed { get { return new SquaredDoorMatDeed(); } }

        private SquaredDoorMatType m_SquaredDoorMatType;

        [Constructable]
        public SquaredDoorMatAddon(SquaredDoorMatType type)
        {
            m_SquaredDoorMatType = type;

            switch (type)
            {
                case SquaredDoorMatType.South:
                    AddComponent(new AddonComponent(0x47DA), 0, 0, 0);
                    break;
                case SquaredDoorMatType.East:
                    AddComponent(new AddonComponent(0x47DB), 0, 0, 0);
                    break;

            }
        }

        public SquaredDoorMatAddon(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class SquaredDoorMatDeed : BaseAddonDeed, IRewardOption
    {
        public override BaseAddon Addon { get { return new SquaredDoorMatAddon(m_SquaredDoorMatType); } }

        private SquaredDoorMatType m_SquaredDoorMatType;

        public override int LabelNumber { get { return 1151806; } } // squared door mat deed

        [Constructable]
        public SquaredDoorMatDeed()
        {
            LootType = LootType.Blessed;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                from.CloseGump(typeof(RewardOptionGump));
                from.SendGump(new RewardOptionGump(this));
            }
            else
                from.SendLocalizedMessage(1062334); // This item must be in your backpack to be used.       	
        }

        public SquaredDoorMatDeed(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }

        public void GetOptions(RewardOptionList list)
        {
            list.Add((int)SquaredDoorMatType.South, 1151815);
            list.Add((int)SquaredDoorMatType.East, 1151816);
        }


        public void OnOptionSelected(Mobile from, int choice)
        {
            m_SquaredDoorMatType = (SquaredDoorMatType)choice;

            if (!Deleted)
                base.OnDoubleClick(from);
        }
    }
}
