using System;

namespace Server.Items
{
    public class TreatForDrithen : Item
    {
        [Constructable]
        public TreatForDrithen()
            : base(0x21B)
        {
            this.LootType = LootType.Blessed;
            this.Weight = 2;
            this.Hue = 0x489;
        }

        public TreatForDrithen(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1074517;
            }
        }// Special Treat for Drithen
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