using System;

namespace Server.Items
{
    public class PunchCard : BaseDecayingItem
    {
        [Constructable]
        public PunchCard() 
            : base(0x0FF4)
        {
            this.LootType = LootType.Regular;
            this.Hue = Utility.RandomNondyedHue();
            this.Weight = 2;
        }

        public override int Lifespan { get { return 21600; } }
        public override bool UseSeconds { get { return false; } }

        public override int LabelNumber { get { return 1153867; } } // Punch Card

        public PunchCard(Serial serial)
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
}