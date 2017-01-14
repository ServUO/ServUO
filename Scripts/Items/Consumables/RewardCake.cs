using System;
using Server.Network;

namespace Server.Items
{
    public class RewardCake : Item
    {
        [Constructable]
        public RewardCake()
            : base(0x9e9)
        {
            this.Stackable = false;
            this.Weight = 1.0;
            this.Hue = Utility.RandomList(0x135, 0xcd, 0x38, 0x3b, 0x42, 0x4f, 0x11e, 0x60, 0x317, 0x10, 0x136, 0x1f9, 0x1a, 0xeb, 0x86, 0x2e);
            this.LootType = LootType.Blessed;
        }

        public RewardCake(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1049786;
            }
        }// Happy Birthday!  ...
        public override bool DisplayLootType
        {
            get
            {
                return Core.ML;
            }
        }
        public override void OnDoubleClick(Mobile from)
        {
            if (! from.InRange(this.GetWorldLocation(), 1))
            {
                from.LocalOverheadMessage(MessageType.Regular, 906, 1019045); // I can't reach that.
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            this.LootType = LootType.Blessed;

            int version = reader.ReadInt();
        }
    }
}