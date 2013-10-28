using System;

namespace Server.Items
{
    public class HolidayTimepiece : Clock
    {
        [Constructable]
        public HolidayTimepiece()
            : base(0x1086)
        {
            this.Weight = this.DefaultWeight;
            this.LootType = LootType.Blessed;
            this.Layer = Layer.Bracelet;
        }

        public HolidayTimepiece(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1041113;
            }
        }// a holiday timepiece
        public override double DefaultWeight
        {
            get
            {
                return 1.0;
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

            int version = reader.ReadInt();
        }
    }
}