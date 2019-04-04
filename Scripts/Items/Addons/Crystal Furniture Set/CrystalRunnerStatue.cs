using System;
using Server.Gumps;

namespace Server.Items
{
    public class CrystalRunnerStatueAddon : BaseAddon
    {
        [Constructable]
        public CrystalRunnerStatueAddon(DirectionType type)
        {
            switch (type)
            {
                case DirectionType.South:
                    AddComponent(new LocalizedAddonComponent(0x35FD, 1076670), 0, 0, 0);
                    break;
                case DirectionType.East:
                    AddComponent(new LocalizedAddonComponent(0x35FC, 1076670), 0, 0, 0);
                    break;
            }
        }

        public CrystalRunnerStatueAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed { get { return new CrystalRunnerStatueDeed(); } }

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

    public class CrystalRunnerStatueDeed : BaseAddonDeed, IRewardOption
    {
        public override int LabelNumber { get { return 1076670; } } // Crystal Runner Statue

        public override bool ExcludeDeedHue { get { return true; } }

        public override BaseAddon Addon { get { return new CrystalRunnerStatueAddon(_Direction); } }

        private DirectionType _Direction;

        [Constructable]
        public CrystalRunnerStatueDeed()
            : base()
        {
            LootType = LootType.Blessed;
            Hue = 1173;
        }

        public CrystalRunnerStatueDeed(Serial serial)
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
                from.CloseGump(typeof(RewardOptionGump));
                from.SendGump(new RewardOptionGump(this, 1076725)); // Please select your crystal runner statue position
            }
            else
            {
                from.SendLocalizedMessage(1062334); // This item must be in your backpack to be used.
            }
        }

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
